using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace BlockchainAssignment.Wallet
{
    // Below is the code for creating transactions, and validating transactions
    public class Transaction
    {
        private string hash, signature;
        public string sender;
        public string recipient;
        public decimal amount;
        public decimal fee;
        private DateTime timestamp;
        public decimal reward;

        public string Hash { get { return hash; } }
        public string Signature { get { return signature; } }





        public Transaction(string privateKey, string SenderAddress, string RecipientAddress, decimal Amount, decimal Fee)
        {
            this.sender = SenderAddress;
            this.recipient = RecipientAddress;
            this.amount = Amount;
            this.fee = Fee;
            this.timestamp = DateTime.Now;
            this.hash = createHashTransaction();
            this.signature = Wallet.CreateSignature(sender, privateKey, hash);
        }
        public string createHashTransaction()
        {
            SHA256 hasher = SHA256Managed.Create();
            String input = timestamp.ToString("O") + sender + recipient + amount + fee;
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            String hash = string.Empty;
            foreach (byte x in hashByte)
                hash += String.Format("{0:x2}", x);
            this.hash = hash;
            return hash;
        }
        public override string ToString()
        {
            return $"Transaction: {hash}\nSender: {sender}\nRecipient: {recipient}\nAmount: {amount}\nFee: {fee}\nTimestamp: {timestamp}";
        }

       

       
    }
}