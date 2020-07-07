namespace EncryptedChat.Client.Common.Crypto
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;

    public class RsaCryptographyManager
    {
        private RSA rsa;

        public string EncryptData(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (this.rsa == null)
            {
                throw new InvalidOperationException();
            }

            return Convert.ToBase64String(
                this.rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256)
            );
        }

        public byte[] DecryptDataAsByteArray(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (this.rsa == null)
            {
                throw new InvalidOperationException();
            }

            return this.rsa.Decrypt(Convert.FromBase64String(data),
                RSAEncryptionPadding.OaepSHA256);
        }

        public string SignData(string data)
        {
            if (this.rsa == null)
            {
                throw new InvalidOperationException();
            }

            var signature = this.rsa.SignData(Encoding.UTF8.GetBytes(data),
                HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

            return Convert.ToBase64String(signature);
        }

        public bool VerifySignature(string data, string signature)
        {
            if (this.rsa == null)
            {
                throw new InvalidOperationException();
            }

            var isValid = this.rsa.VerifyData(Encoding.UTF8.GetBytes(data),
                Convert.FromBase64String(signature),
                HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

            return isValid;
        }

        public void GenerateNewKey(int keySize = 4096)
        {
            this.rsa = RSA.Create(keySize);
        }

        public string ExportKeyAsXml(bool includePrivateParams = false)
        {
            if (this.rsa == null)
            {
                throw new InvalidOperationException();
            }

            return ExportRsaParams(this.rsa, includePrivateParams);
        }

        public void LoadKeyFromXml(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                throw new ArgumentNullException(nameof(xmlString));
            }

            var rsaParameters = CreateRsaParamsFromXmlString(xmlString);

            this.rsa = RSA.Create(rsaParameters);
        }

        private static string ExportRsaParams(RSA rsa, bool includePrivateParameters)
        {
            var rsaParams = rsa.ExportParameters(includePrivateParameters);
            var sb = new StringBuilder();

            sb.Append("<RSAKeyValue>");
            sb.Append("<Modulus>" + Convert.ToBase64String(rsaParams.Modulus) + "</Modulus>");
            sb.Append("<Exponent>" + Convert.ToBase64String(rsaParams.Exponent) + "</Exponent>");

            if (includePrivateParameters)
            {
                sb.Append("<P>" + Convert.ToBase64String(rsaParams.P) + "</P>");
                sb.Append("<Q>" + Convert.ToBase64String(rsaParams.Q) + "</Q>");
                sb.Append("<DP>" + Convert.ToBase64String(rsaParams.DP) + "</DP>");
                sb.Append("<DQ>" + Convert.ToBase64String(rsaParams.DQ) + "</DQ>");
                sb.Append("<InverseQ>" + Convert.ToBase64String(rsaParams.InverseQ) + "</InverseQ>");
                sb.Append("<D>" + Convert.ToBase64String(rsaParams.D) + "</D>");
            }

            sb.Append("</RSAKeyValue>");

            return sb.ToString();
        }

        private static RSAParameters CreateRsaParamsFromXmlString(string xmlString)
        {
            var parameters = new RSAParameters();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus":
                            parameters.Modulus = Convert.FromBase64String(node.InnerText);

                            break;
                        case "Exponent":
                            parameters.Exponent = Convert.FromBase64String(node.InnerText);

                            break;
                        case "P":
                            parameters.P = Convert.FromBase64String(node.InnerText);

                            break;
                        case "Q":
                            parameters.Q = Convert.FromBase64String(node.InnerText);

                            break;
                        case "DP":
                            parameters.DP = Convert.FromBase64String(node.InnerText);

                            break;
                        case "DQ":
                            parameters.DQ = Convert.FromBase64String(node.InnerText);

                            break;
                        case "InverseQ":
                            parameters.InverseQ = Convert.FromBase64String(node.InnerText);

                            break;
                        case "D":
                            parameters.D = Convert.FromBase64String(node.InnerText);

                            break;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid XML RSA key.");
            }

            return parameters;
        }

        public string GetSha256Fingerprint()
        {
            if (this.rsa == null)
            {
                throw new InvalidOperationException();
            }

            var rsaParams = this.rsa.ExportParameters(false);

            byte[] hashBytes;
            using (SHA256 sha256 = new SHA256Managed())
            {
                hashBytes = sha256.ComputeHash(rsaParams.Modulus);
            }

            var sb = new StringBuilder(95);
            for (var i = 0; i < hashBytes.Length; i++)
            {
                byte b = hashBytes[i];
                sb.Append(b.ToString("X2"));

                if (i < hashBytes.Length - 1)
                {
                    sb.Append(':');
                }
            }

            return sb.ToString();
        }
    }
}