using EncryptedChat.Client.Common;
using EncryptedChat.Client.Common.Configuration;
using EncryptedChat.Client.Common.Crypto;
using EncryptedChat.Client.Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private HubConnection connection;
        private EncryptedCommunicationsManager communicationsManager;
        private ConfigurationManager<MainConfiguration> configurationManager;

        private User[] waitingUsers;
        private string username;
        private State state;
        private User otherUser;

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void LoadUsername()
        {
            this.username = this.configurationManager.Configuration.Username;

            Regex usernameRegex = new Regex(Constants.UsernameRegex);

            if (!string.IsNullOrWhiteSpace(this.username) && usernameRegex.IsMatch(this.username))
            {
                return;
            }

            frmRegister frmRegister = new frmRegister(Messages.UsernameInfo);
            if (frmRegister.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(frmRegister.Username) || !usernameRegex.IsMatch(frmRegister.Username))
                {
                    this.username = frmRegister.Username;
                    this.configurationManager.Configuration.Username = this.username;
                    this.configurationManager.SaveChanges();
                }
            }

            
        }

        private void LoadPrivateKey()
        {
            if (this.configurationManager.Configuration.PrivateKey == null)
            {
                richCommunication.AppendText(Messages.GeneratingKeyPair + "\n");

                this.communicationsManager.GenerateNewRsaKey();

                this.configurationManager.Configuration.PrivateKey =
                    this.communicationsManager.ExportOwnRsaKey(true);

                this.configurationManager.SaveChanges();
            }
            else
            {
                richCommunication.AppendText(Messages.LoadingPrivateKey + "\n");

                this.communicationsManager.ImportOwnRsaKey(this.configurationManager.Configuration.PrivateKey);
            }
        }

        private bool IsUserTrusted(User user)
        {
            if (!this.configurationManager.Configuration.TrustedUsers.ContainsKey(user.Username))
            {
                return false;
            }

            string keyHash = HashingUtil.GetSha256Hash(user.PublicKey);

            return this.configurationManager.Configuration.TrustedUsers[user.Username] == keyHash;
        }

        private void UpdateWaitingList(User[] users)
        {
            if (this.state != State.SelectingUser)
            {
                return;
            }

            Regex usernameRegex = new Regex(Constants.UsernameRegex);

            this.waitingUsers = users.Where(user =>
                    !string.IsNullOrWhiteSpace(user.Username) &&
                    usernameRegex.IsMatch(user.Username))
                .ToArray();

            int invalidUsernamesDifference = users.Length - this.waitingUsers.Length;

            if (this.waitingUsers.Length == 0)
            {
                Console.WriteLine(Messages.UserListNoUsers);
            }
            else
            {
                List<UserViewModel> userVms = new List<UserViewModel>();
                for (int i = 0; i < this.waitingUsers.Length; i++)
                {
                    string trustedBadge = this.IsUserTrusted(this.waitingUsers[i])
                        ? Messages.UserTrustedBadge
                        : Messages.UserNotTrustedBadge;

                    var displayName = string.Format(Messages.UserListItem, i + 1
                    , this.waitingUsers[i].Username
                    , trustedBadge);

                    userVms.Add(new UserViewModel
                    {
                        DisplayName = displayName,
                        UserObj = this.waitingUsers[i]
                    });
                }

                lstUsers.Items.Clear();
                lstUsers.DataSource = userVms;
                lstUsers.DisplayMember = "DisplayName";
                lstUsers.ValueMember = "UserObj";
            }

            if (invalidUsernamesDifference != 0)
            {
                richCommunication.AppendText(string.Format(Messages.UserListInvalidUsername, invalidUsernamesDifference,
                    invalidUsernamesDifference != 1 ? "s" : "") + "\n");
            }

            richCommunication.AppendText(Messages.UserListJoin + "\n");
        }

        private void Disconnect()
        {
            richCommunication.AppendText(Messages.Disconnected + "\n");

            this.state = State.Disconnected;
        }

        private void CreateChatWithUser(User user)
        {
            this.state = State.InChat;

            Regex usernameRegex = new Regex(Constants.UsernameRegex);

            if (string.IsNullOrWhiteSpace(user.Username) || !usernameRegex.IsMatch(user.Username))
            {
                richCommunication.AppendText(Messages.OtherUsernameInvalid + "\n");
                this.Disconnect();
                return;
            }

            this.otherUser = user;

            bool isTrusted = this.IsUserTrusted(this.otherUser);

            string trustedBadge = isTrusted
                ? Messages.UserTrustedBadge
                : Messages.UserNotTrustedBadge;

            richCommunication.AppendText(string.Format(Messages.ConnectedWithUser, user.Username, trustedBadge) + "\n");
            richCommunication.AppendText(string.Format(Messages.CurrentUserFingerprint, this.communicationsManager.GetOwnRsaFingerprint()) + "\n");

            if (!isTrusted)
            {
                richCommunication.AppendText(string.Format(Messages.OtherUserFingerprint, this.otherUser.Username,
                    this.communicationsManager.GetOtherRsaFingerprint()) + "\n");

                richCommunication.AppendText(new string('-', 30) + "\n");
                richCommunication.AppendText(Messages.UserNotTrustedMessage + "\n");
                richCommunication.AppendText(new string('-', 30) + "\n");
                btnTrust.Enabled = true;
            }
        }

        private void AcceptConnection(string aesKey, string otherUsername, string rsaKey, string signature)
        {
            if (this.state != State.Waiting)
            {
                return;
            }

            richCommunication.AppendText(Messages.InitialisingEncryptedConnection + "\n");

            this.communicationsManager.ImportOtherRsaKey(rsaKey);
            var signatureValid = this.communicationsManager.VerifySignature(aesKey, signature);
            if (!signatureValid)
            {
                richCommunication.AppendText(Messages.IncomingConnectionSignatureInvalid + "\n");
                this.Disconnect();
                return;
            }

            this.communicationsManager.ImportEncryptedAesKey(aesKey);

            var user = new User
            {
                Username = otherUsername,
                PublicKey = rsaKey
            };

            this.CreateChatWithUser(user);
        }

        private void NewMessage(string encryptedMessage, string messageUsername)
        {
            if (this.state != State.InChat)
            {
                return;
            }

            string decryptedMessage = this.communicationsManager.DecryptMessage(encryptedMessage);

            richCommunication.AppendText(string.Format(Messages.MessageFormat, messageUsername, decryptedMessage) + "\n");
        }

        private async Task SetUpConnection()
        {
            richCommunication.AppendText(
                string.Format(Messages.ConnectingToServer, this.configurationManager.Configuration.ServerUrl)
                + "\n");

            this.connection = new HubConnectionBuilder()
                .WithUrl(this.configurationManager.Configuration.ServerUrl)
                .Build();

            this.connection.On<User[]>(nameof(this.UpdateWaitingList), this.UpdateWaitingList);
            this.connection.On<string, string, string, string>(nameof(this.AcceptConnection), this.AcceptConnection);
            this.connection.On<string, string>(nameof(this.NewMessage), this.NewMessage);
            this.connection.On(nameof(this.Disconnect), this.Disconnect);

            await this.connection.StartAsync();

            richCommunication.AppendText(Messages.Connected + "\n");
        }

        private async Task StartReadingInput()
        {

        }

        private void LoadConfiguration()
        {
            richCommunication.AppendText(Messages.LoadingConfiguration + "\n");

            this.configurationManager = new ConfigurationManager<MainConfiguration>(Constants.ConfigurationFilePath);

            this.LoadUsername();

            this.communicationsManager = new EncryptedCommunicationsManager();

            this.LoadPrivateKey();
        }

        public async Task Setup()
        {
            this.LoadConfiguration();

            await this.SetUpConnection();

            this.state = State.SelectingUser;

            await this.StartReadingInput();
        }

        private async Task JoinAsWaitingUser()
        {
            string pubKey = this.communicationsManager.ExportOwnRsaKey();

            richCommunication.AppendText(Messages.SendingKeyToServer + "\n");

            this.state = State.Waiting;

            await this.connection.InvokeCoreAsync("RegisterAsWaiting", new object[]
            {
                this.username, pubKey
            });


            richCommunication.AppendText(Messages.WaitingForUser + "\n");
        }

        private async Task ConnectWithUser(User selectedUser)
        {
            richCommunication.AppendText(Messages.GeneratingSessionKey + "\n");

            this.communicationsManager.ImportOtherRsaKey(selectedUser.PublicKey);
            string aesKey = this.communicationsManager.GenerateEncryptedAesKey();
            string key = this.communicationsManager.ExportOwnRsaKey();
            string signature = this.communicationsManager.SignData(aesKey);

            richCommunication.AppendText(Messages.InitialisingEncryptedConnection + "\n");

            await this.connection.InvokeCoreAsync("ConnectToUser", new object[]
            {
                this.username, selectedUser.Id, aesKey, key, signature
            });

            this.CreateChatWithUser(selectedUser);
        }

        private async Task UserSelect(User input)
        {
            if (this.waitingUsers == null)
            {
                return;
            }

            await this.ConnectWithUser(input);
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            var input = richMessage.Text;
            if (input == Commands.ExitCommand ||
                this.connection.State == HubConnectionState.Disconnected ||
                this.state == State.Disconnected)
            {
                richCommunication.AppendText(Messages.Disconnected + "\n");

                if (this.connection.State != HubConnectionState.Disconnected)
                {
                    await this.connection.StopAsync();
                }

                return;
            }

            switch (this.state)
            {
                case State.InChat:
                    if (input == Commands.TrustCommand)
                    {
                        this.TrustCurrentUser();
                        break;
                    }

                    await this.SendMessage(input);
                    break;
            }
        }

        private void TrustCurrentUser()
        {
            bool result = this.TrustUser(this.otherUser);

            Console.WriteLine(result ? Messages.UserTrusted : Messages.CouldNotTrustUser);
            btnTrust.Enabled = false;
        }


        private async void btnJoinWait_Click(object sender, EventArgs e)
        {
            await this.JoinAsWaitingUser();
        }

        private async void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            if (this.state == State.SelectingUser)
            {
                User inputUser = (User)lstUsers.SelectedValue;
                await this.UserSelect(inputUser);
            }
        }

        private void btnTrust_Click(object sender, EventArgs e)
        {
            if (this.state == State.InChat)
            {
                this.TrustCurrentUser();
            }
        }
    }
}
