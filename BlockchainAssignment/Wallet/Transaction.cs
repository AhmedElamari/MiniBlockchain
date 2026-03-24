using System;
using System.Text;
using System.Security.Cryptography;

namespace BlockchainAssignment.Wallet
{
    public class Transaction
    {
        public const string miningRewardSenderID = "Mine Rewards";
        private string hash, signature;
        public string sender;
        public string recipient;
        public decimal amount;
        public decimal fee;
        private DateTime timestamp;
        public decimal reward;

        public string Hash { get { return hash; } }
        public string Signature { get { return signature; } }

        public Transaction(string privateKey, string senderAddress, string recipientAddress, decimal amount, decimal fee)
            : this(privateKey, senderAddress, recipientAddress, amount, fee, DateTime.Now)
        {
        }

        public Transaction(string privateKey, string senderAddress, string recipientAddress, decimal amount, decimal fee, DateTime timestamp)
        {
            this.sender = senderAddress;
            this.recipient = recipientAddress;
            this.amount = amount;
            this.fee = fee;
            this.timestamp = timestamp;
            this.hash = createHashTransaction();
            this.signature = Wallet.createSignature(sender, privateKey, hash);
        }

        public string createHashTransaction()
        {
            SHA256 hasher = SHA256Managed.Create();
            String input = timestamp.ToString("O") + sender + recipient + amount + fee;
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            String hash = string.Empty;
            foreach (byte x in hashByte)
                hash += String.Format("{0:x2}", x);
            return hash;
        }

        public bool isValid()
        {
            if (string.IsNullOrWhiteSpace(signature) || signature == "null")
                return false;

            if (sender == miningRewardSenderID)
            {
                if (fee != 0)
                    return false;
                if (string.IsNullOrWhiteSpace(recipient))
                    return false;
                if (amount <= 0)
                    return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(recipient))
                    return false;
                if (amount <= 0)
                    return false;
                if (fee < 0)
                    return false;
            }

            if (string.IsNullOrEmpty(hash))
                return false;

            String storedHash = hash;
            this.hash = createHashTransaction();
            if (storedHash != hash)
                return false;

            return Wallet.ValidateSignature(sender, hash, signature);
        }

        public override string ToString()
        {
            return "Transaction: " + hash + "\nSender: " + sender + "\nRecipient: " + recipient + "\nAmount: " + amount + "\nFee: " + fee + "\nTimestamp: " + timestamp;
        }
    }
}
