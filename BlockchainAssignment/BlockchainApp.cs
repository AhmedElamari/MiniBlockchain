using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BlockchainAssignment.Wallet;

namespace BlockchainAssignment
{
    public partial class BlockchainApp : Form
    {
        private Blockchain blockchain;
        private volatile bool _backgroundWorkRunning;

        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            richTextBox1.Text = "New Blockchain initialised.";
            comboBox1.Items.AddRange(Enum.GetNames(typeof(Blockchain.MiningPolicy)));
            comboBox1.SelectedIndex = 0;
        }

        private void SetRichText(string text, bool append)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetRichText(text, append)));
                return;
            }

            if (append)
                richTextBox1.AppendText(text);
            else
                richTextBox1.Text = text;
        }

        private void SetMiningButtonsEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetMiningButtonsEnabled(enabled)));
                return;
            }

            button2.Enabled = enabled;
            buttonBenchmarkMining.Enabled = enabled;
            button3.Enabled = enabled;
        }

        private void updateText(string text)
        {
            SetRichText(text, false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetRichText(textBox1.Text, false);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

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
            SetRichText("Public ID: " + wallet.publicID + "\nPrivate Key: " + privateKey, false);
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

            SetRichText(valid
                ? "Private key is valid for the provided Public ID."
                : "Private key is not valid for the provided Public ID.", false);
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

            decimal amount;
            if (!decimal.TryParse(textBox4.Text, out amount))
            {
                MessageBox.Show("Please enter a valid number for Amount.", "Invalid Input");
                return;
            }

            decimal fee;
            if (!decimal.TryParse(textBox5.Text, out fee))
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

            blockchain.addPendingTransaction(newTransaction);
            SetRichText(newTransaction.ToString(), false);
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
            if (_backgroundWorkRunning)
                return;

            Block lastBlock = blockchain.getLastBlock();
            Blockchain.MiningPolicy miningPolicy = (Blockchain.MiningPolicy)comboBox1.SelectedIndex;
            List<Transaction> chosenTransactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, textBox2.Text);
            string minerAddress = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            if (string.IsNullOrWhiteSpace(minerAddress))
            {
                MessageBox.Show("Please enter a Public ID (miner) for the reward transaction.", "Mining");
                return;
            }

            float difficulty = blockchain.getDifficultyForNextBlock();

            _backgroundWorkRunning = true;
            SetMiningButtonsEnabled(false);

            var controller = new Thread(() =>
            {
                try
                {
                    Block candidate = Block.CreateUnminedCandidate(lastBlock, chosenTransactions, minerAddress, DateTime.Now, difficulty);
                    Block.MiningMessageCallback cb = message =>
                        BeginInvoke(new Action(() => SetRichText(message + Environment.NewLine, true)));
                    candidate.Mine(Environment.ProcessorCount, cb);

                    Invoke(new Action(() =>
                    {
                        try
                        {
                            string failureMessage;
                            if (!blockchain.addBlock(candidate, out failureMessage))
                                MessageBox.Show(failureMessage, "Error");
                            else
                                SetRichText(blockchain.returnBlockchain(candidate.Index) + "\n", false);
                        }
                        finally
                        {
                            _backgroundWorkRunning = false;
                            SetMiningButtonsEnabled(true);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(ex.Message, "Error");
                        _backgroundWorkRunning = false;
                        SetMiningButtonsEnabled(true);
                    }));
                }
            });
            controller.IsBackground = true;
            controller.Start();
        }

        private void button_benchmarkMining_Click(object sender, EventArgs e)
        {
            if (_backgroundWorkRunning)
                return;

            string minerAddress = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            if (string.IsNullOrWhiteSpace(minerAddress))
            {
                MessageBox.Show("Please enter a Public ID (miner) for the benchmark reward transaction.", "Benchmark");
                return;
            }

            Block lastBlock = blockchain.getLastBlock();
            int transactionsPerBlock = blockchain.getTransactionsPerBlock();
            List<Transaction> transactionsSnapshot = blockchain.getPendingTransactionsPool().Take(transactionsPerBlock).ToList();
            int chainBefore = blockchain.getBlocks().Count;
            int poolBefore = blockchain.getPendingTransactionsPool().Count;
            int workers = Environment.ProcessorCount;

            _backgroundWorkRunning = true;
            SetMiningButtonsEnabled(false);

            var benchThread = new Thread(() =>
            {
                try
                {
                    var report = new StringBuilder();
                    report.AppendLine("Benchmark: difficulty 5, " + workers + " logical processors");
                    report.AppendLine("Per sample: same chain head, tx snapshot, miner, timestamp; workers use disjoint nonce strides.");
                    report.AppendLine();

                    if (workers <= 1)
                    {
                        report.AppendLine("Note: single logical processor — parallel speedup is not meaningful here.");
                        report.AppendLine();
                    }

                    double[] singleMs = new double[5];
                    double[] parallelMs = new double[5];
                    Func<int, DateTime, double> timeMine = (workerCount, ts) =>
                    {
                        var block = Block.CreateUnminedCandidate(lastBlock, new List<Transaction>(transactionsSnapshot), minerAddress, ts, 5f);
                        var sw = Stopwatch.StartNew();
                        block.Mine(workerCount, null);
                        sw.Stop();
                        return sw.Elapsed.TotalMilliseconds;
                    };

                    for (int sample = 0; sample < 5; sample++)
                    {
                        DateTime sampleTimestamp = DateTime.UtcNow;
                        singleMs[sample] = timeMine(1, sampleTimestamp);
                        parallelMs[sample] = timeMine(workers, sampleTimestamp);
                        report.AppendLine("Sample " + (sample + 1) + ": 1 thread = " + singleMs[sample].ToString("F2") + " ms, " + workers + " threads = " + parallelMs[sample].ToString("F2") + " ms");
                    }

                    double avgSingle = singleMs.Average();
                    double avgParallel = parallelMs.Average();
                    double speedup = avgParallel > 0 ? avgSingle / avgParallel : 0;
                    double efficiency = workers > 0 ? speedup / workers : 0;

                    report.AppendLine();
                    report.AppendLine("Average 1-thread time: " + avgSingle.ToString("F2") + " ms");
                    report.AppendLine("Average parallel time: " + avgParallel.ToString("F2") + " ms");
                    report.AppendLine("Speedup (single / parallel): " + speedup.ToString("F3"));
                    report.AppendLine("Efficiency (speedup / worker count): " + efficiency.ToString("F3"));

                    int chainAfter = chainBefore;
                    int poolAfter = poolBefore;
                    Invoke(new Action(() =>
                    {
                        chainAfter = blockchain.getBlocks().Count;
                        poolAfter = blockchain.getPendingTransactionsPool().Count;
                    }));
                    report.AppendLine();
                    report.AppendLine("Chain blocks: " + chainBefore + " → " + chainAfter + " (no new blocks)");
                    report.AppendLine("Pending pool: " + poolBefore + " → " + poolAfter);

                    SetRichText(report.ToString(), false);
                }
                finally
                {
                    _backgroundWorkRunning = false;
                    SetMiningButtonsEnabled(true);
                }
            });
            benchThread.IsBackground = true;
            benchThread.Start();
        }

        private void button_readAll(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            for (int i = 0; i < blockchain.getBlocks().Count; i++)
            {
                richTextBox1.Text += "  " + blockchain.returnBlockchain(blockchain.getBlocks()[i].Index) + "\n" + "\n";
            }
        }

        private void readAllTransactions(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            List<Transaction> transactions = blockchain.getPendingTransactionsPool();
            foreach (Transaction t in transactions)
            {
                richTextBox1.Text += t.ToString() + "\n" + "\n";
            }
        }

        private void validateBlockchain_Click(object sender, EventArgs e)
        {
            string message;
            bool ok = blockchain.validateBlockchain(out message);
            MessageBox.Show(message, ok ? "Blockchain valid" : "Blockchain invalid");
            SetRichText(message, false);
        }

        private void checkBalanceButton(object sender, EventArgs e)
        {
            updateText(blockchain.getLastBlock().checkBalance(blockchain.getBlocks(), textBox2.Text).ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string minerAddress = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            if (string.IsNullOrWhiteSpace(minerAddress))
            {
                MessageBox.Show("Please enter a Public ID (miner) for the reward transaction.", "Mining");
                return;
            }
            Blockchain.MiningPolicy miningPolicy = (Blockchain.MiningPolicy)comboBox1.SelectedIndex;
            List<Transaction> transactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, minerAddress);
            richTextBox1.Text = string.Join("\n", transactions.Select(t => t.ToString()));
        }
    }
}
