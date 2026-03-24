using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlockchainAssignment.Wallet;

namespace BlockchainAssignment
{
    public partial class BlockchainApp : Form
    {
        private Blockchain blockchain;
        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            richTextBox1.Text = "New Blockchain initialised.";

        }

        private void updateText(string text)
        {
            richTextBox1.Text = text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = textBox1.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Block lastBlock = blockchain.GetLastBlock();
            richTextBox1.Text = blockchain.returnBlockchain(lastBlock);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void GenerateWallet_Click(object sender, EventArgs e)
        {
            String privateKey;
            Wallet.Wallet wallet = new Wallet.Wallet(out privateKey);
            richTextBox1.Text = "Public ID: " + wallet.publicID + "\nPrivate Key: " + privateKey;
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {
           
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void ValidateKey_Click(object sender, EventArgs e)
        {
            string publicId = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            string privateKey = textBox3.Text == null ? string.Empty : textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(publicId))
            {
                MessageBox.Show("Please enter a Public ID.", "Validation");
                return;
            }

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                MessageBox.Show("Please enter a Private Key.", "Validation");
                return;
            }

            bool valid;

            try
            {
                valid = Wallet.Wallet.ValidatePrivateKey(privateKey, publicId);
            }
            catch
            {
                MessageBox.Show("Invalid key format.", "Validation");
                return;
            }

            richTextBox1.Text = valid
                ? "Private key is valid for the provided Public ID."
                : "Private key is not valid for the provided Public ID.";
        }
        private void button_newTransaction(object sender, EventArgs e)
        {
            string publicId = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            string privateKey = textBox3.Text == null ? string.Empty : textBox3.Text.Trim();
            string recipient = textBox6.Text == null ? string.Empty : textBox6.Text.Trim();

            if (string.IsNullOrWhiteSpace(publicId))
            {
                MessageBox.Show("Please enter a Public ID (sender).", "Invalid Input");
                return;
            }

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                MessageBox.Show("Please enter a Private Key.", "Invalid Input");
                return;
            }

            if (string.IsNullOrWhiteSpace(recipient))
            {
                MessageBox.Show("Please enter a Receiver Key.", "Invalid Input");
                return;
            }

            if (!decimal.TryParse(textBox4.Text, out decimal amount))
            {
                MessageBox.Show("Please enter a valid number for Amount.", "Invalid Input");
                return;
            }

            if (!decimal.TryParse(textBox5.Text, out decimal fee))
            {
                MessageBox.Show("Please enter a valid number for Fee.", "Invalid Input");
                return;
            }

            if (amount <= 0)
            {
                MessageBox.Show("Amount must be greater than zero.", "Invalid Input");
                return;
            }

            if (fee < 0)
            {
                MessageBox.Show("Fee cannot be negative.", "Invalid Input");
                return;
            }

            Transaction newTransaction;
            try
            {
                newTransaction = new Transaction(privateKey, publicId, recipient, amount, fee);
            }
            catch
            {
                MessageBox.Show("Invalid key format.", "Error");
                return;
            }

            if (!newTransaction.isValid())
            {
                MessageBox.Show("Invalid transaction (check keys, amount, fee, and signature).", "Error");
                return;
            }

            blockchain.transactionPool.Add(newTransaction);
            richTextBox1.Text = newTransaction.ToString();
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_createBlock(object sender, EventArgs e)
        {
            List<Transaction> chosenTransactions = blockchain.getTransactionsForNextBlock();
            string minerAddress = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            Block newBlock = new Block(blockchain.GetLastBlock(), chosenTransactions, minerAddress);
            if (!blockchain.AddBlock(newBlock, out string failureMessage))
            {
                MessageBox.Show(failureMessage, "Error");
                return;
            }

            richTextBox1.Text = blockchain.returnBlockchain(newBlock.Index) + "\n";
        }

        private void button_readAll(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            for (int i = 0; i < blockchain.GetBlocks().Count; i++)
            {
                richTextBox1.Text += "  " + blockchain.returnBlockchain(blockchain.GetBlocks()[i].Index) + "\n" + "\n";
            }
        }

        private void readAllTransactions(object sender, EventArgs e)
        {
                        richTextBox1.Clear();
            List<Transaction> transactions = blockchain.GetPendingTransactions();
            foreach (Transaction t in transactions)
            {
                richTextBox1.Text += t.ToString() + "\n" + "\n";
            }   
        }

        private void validateBlockchain_Click(object sender, EventArgs e)
        {
            bool ok = blockchain.validateBlockchain(out string message);
            MessageBox.Show(message, ok ? "Blockchain valid" : "Blockchain invalid");
            richTextBox1.Text = message;
        }

        private void checkBalanceButton(object sender, EventArgs e)
        {
            updateText(blockchain.GetLastBlock().CheckBalance(blockchain.GetBlocks(), textBox2.Text).ToString());
        }
    }
}