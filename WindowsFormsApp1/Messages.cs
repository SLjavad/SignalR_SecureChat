﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsFormsApp1
{
    public static class Messages
    {
        public const string ConnectingToServer = "Connecting to {0}...";
        public const string Connected = "Connected.";
        public const string Disconnected = "Disconnected.";
        public const string UsernamePrompt = "Username: ";
        public const string InvalidUserIdSelectedError = "Please type a number from 0 to {0}";
        public const string GeneratingSessionKey = "Generating session key...";
        public const string InitialisingEncryptedConnection = "Initialising encrypted connection...";
        public const string IncomingConnectionSignatureInvalid = "Could not verify user identity.";
        public const string OtherUsernameInvalid = "The other user's username is invalid.";
        public const string ConnectedWithUser = "Connected with {0} - {1}!";
        public const string UserListHeader = "Users:";
        public const string UserListItem = "{0} - {1} {2}";
        public const string UserListInvalidUsername = "Not showing {0} user{1} with an invalid username.";
        public const string UserListJoin = "0 - join waiting list";
        public const string UserTrustedBadge = "[trusted]";
        public const string UserNotTrustedBadge = "[not trusted]";
        public const string UserListNoUsers = "None";
        public const string GeneratingKeyPair = "Generating keypair...";
        public const string LoadingPrivateKey = "Loading private key...";
        public const string SendingKeyToServer = "Sending public key to server...";
        public const string WaitingForUser = "Waiting for other user";
        public const string CurrentUserFingerprint = "Your fingerprint: {0}";
        public const string OtherUserFingerprint = "{0}'s fingerprint: {1}";
        public const string MessageFormat = "<{0}> {1}";
        public const string LoadingConfiguration = "Loading configuration...";
        public const string UserTrusted = "User trusted.";
        public const string CouldNotTrustUser = "Could not trust user";

        public const string UserNotTrustedMessage = "User not trusted. Verify key fingerprints and type "
                                                    + Commands.TrustCommand + " to trust user";

        public const string UsernameInfo =
            "Please choose a username. It must be between 3 and 20 characters long and " +
            "may contain uppercase and lowercase letters from the English alphabet, " +
            "digits, dots, and underscores. It may not begin or end with " +
            "a dot or an underscore, or contain two or more of them in a row.";
    }
}
