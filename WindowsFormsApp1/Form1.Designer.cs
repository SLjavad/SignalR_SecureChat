namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richCommunication = new System.Windows.Forms.RichTextBox();
            this.richMessage = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.btnTrust = new System.Windows.Forms.Button();
            this.btnJoinWait = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richCommunication
            // 
            this.richCommunication.Location = new System.Drawing.Point(193, 12);
            this.richCommunication.Name = "richCommunication";
            this.richCommunication.Size = new System.Drawing.Size(692, 357);
            this.richCommunication.TabIndex = 0;
            this.richCommunication.Text = "";
            // 
            // richMessage
            // 
            this.richMessage.Location = new System.Drawing.Point(193, 375);
            this.richMessage.Name = "richMessage";
            this.richMessage.Size = new System.Drawing.Size(692, 62);
            this.richMessage.TabIndex = 0;
            this.richMessage.Text = "";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(40, 347);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(112, 26);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lstUsers
            // 
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.ItemHeight = 15;
            this.lstUsers.Location = new System.Drawing.Point(12, 12);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(166, 319);
            this.lstUsers.TabIndex = 2;
            this.lstUsers.DoubleClick += new System.EventHandler(this.lstUsers_DoubleClick);
            // 
            // btnTrust
            // 
            this.btnTrust.Enabled = false;
            this.btnTrust.Location = new System.Drawing.Point(40, 379);
            this.btnTrust.Name = "btnTrust";
            this.btnTrust.Size = new System.Drawing.Size(112, 26);
            this.btnTrust.TabIndex = 1;
            this.btnTrust.Text = "Trust This User";
            this.btnTrust.UseVisualStyleBackColor = true;
            // 
            // btnJoinWait
            // 
            this.btnJoinWait.Enabled = false;
            this.btnJoinWait.Location = new System.Drawing.Point(40, 411);
            this.btnJoinWait.Name = "btnJoinWait";
            this.btnJoinWait.Size = new System.Drawing.Size(112, 26);
            this.btnJoinWait.TabIndex = 1;
            this.btnJoinWait.Text = "Join As Waiting";
            this.btnJoinWait.UseVisualStyleBackColor = true;
            this.btnJoinWait.Click += new System.EventHandler(this.btnJoinWait_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 449);
            this.Controls.Add(this.btnJoinWait);
            this.Controls.Add(this.btnTrust);
            this.Controls.Add(this.lstUsers);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.richMessage);
            this.Controls.Add(this.richCommunication);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richCommunication;
        private System.Windows.Forms.RichTextBox richMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Button btnTrust;
        private System.Windows.Forms.Button btnJoinWait;
    }
}

