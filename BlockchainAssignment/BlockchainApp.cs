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
        private bool _miningPolicyUiReady;

        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            richTextBox1.Text = "New Blockchain initialised.";
            _miningPolicyUiReady = false;
            comboBox1.Items.AddRange(Enum.GetNames(typeof(Blockchain.MiningPolicy)));
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.AddRange(new[] { "ProofOfWork", "ProofOfStake" });
            comboBox2.SelectedIndex = 0;
            _miningPolicyUiReady = true;
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

            string failureMessage;
            if (!blockchain.tryAddPendingTransaction(newTransaction, out failureMessage))
            {
                MessageBox.Show(failureMessage, "Error");
                return;
            }

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
            string minerAddress = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            Blockchain.MiningPolicy miningPolicy = (Blockchain.MiningPolicy)comboBox1.SelectedIndex;
            float difficulty = blockchain.getDifficultyForNextBlock();

            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() == "ProofOfStake")
            {
                try
                {
                    Validator selectedValidator = blockchain.SelectValidator();
                    if (selectedValidator == null)
                    {
                        MessageBox.Show("No validators registered.", "Proof of Stake");
                        return;
                    }

                    List<Transaction> posTransactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, selectedValidator.publicKey);

                    Block candidate = Block.CreateProofOfStakeCandidate(lastBlock, posTransactions, selectedValidator.publicKey, DateTime.Now, difficulty);
                    string failureMessage;
                    if (!blockchain.addBlock(candidate, out failureMessage))
                        MessageBox.Show(failureMessage, "Error");
                    else
                        SetRichText(blockchain.returnBlockchain(candidate.Index) + "\n", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Proof of Stake");
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(minerAddress))
            {
                MessageBox.Show("Please enter a Public ID (miner) for the reward transaction.", "Mining");
                return;
            }

            List<Transaction> chosenTransactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, minerAddress);

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

        private void buttonValidationEvidence_Click(object sender, EventArgs e)
        {
            Block lastBlock = blockchain.getLastBlock();
            string minerAddress = string.IsNullOrWhiteSpace(textBox2.Text) ? Transaction.miningRewardSenderID : textBox2.Text.Trim();
            Blockchain.MiningPolicy miningPolicy = (Blockchain.MiningPolicy)comboBox1.SelectedIndex;
            string preferentialMiner = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            List<Transaction> transactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, preferentialMiner);
            float difficulty = blockchain.getDifficultyForNextBlock();
            int chainBefore = blockchain.getBlocks().Count;
            int poolBefore = blockchain.getPendingTransactionsPool().Count;

            var report = new StringBuilder();
            report.AppendLine("Validation evidence checks (tampered blocks are rejected; rejected PoS may slash validator stake):");
            report.AppendLine();

            Block merkleCandidate = Block.CreateUnminedCandidate(lastBlock, transactions, minerAddress, DateTime.Now, difficulty);
            merkleCandidate.Mine(Environment.ProcessorCount, null);
            merkleCandidate.merikleRoot = "tampered";
            string message;
            bool accepted = blockchain.addBlock(merkleCandidate, out message);
            report.AppendLine("Tampered Merkle root: " + (accepted ? "accepted" : "rejected") + " - " + message);

            Block powCandidate = Block.CreateUnminedCandidate(lastBlock, transactions, minerAddress, DateTime.Now, difficulty);
            powCandidate.Hash = new string('F', 64);
            accepted = blockchain.addBlock(powCandidate, out message);
            report.AppendLine("Invalid proof-of-work hash: " + (accepted ? "accepted" : "rejected") + " - " + message);

            if (blockchain.getValidators().Count > 0)
            {
                Validator selected = blockchain.SelectValidator();
                if (selected != null)
                {
                    List<Transaction> posTransactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, selected.publicKey);
                    Block posCandidate = Block.CreateProofOfStakeCandidate(lastBlock, posTransactions, selected.publicKey, DateTime.Now, difficulty);
                    posCandidate.merikleRoot = "tampered";
                    accepted = blockchain.addBlock(posCandidate, out message);
                    report.AppendLine("Tampered proof-of-stake Merkle root: " + (accepted ? "accepted" : "rejected") + " - " + message);
                }
                else
                {
                    report.AppendLine("Tampered proof-of-stake Merkle root: skipped (could not select a validator).");
                }
            }
            else
            {
                report.AppendLine("Tampered proof-of-stake Merkle root: skipped (no validators registered).");
            }

            report.AppendLine();
            List<Validator> validatorsAfter = blockchain.getValidators();
            if (validatorsAfter.Count > 0)
                report.AppendLine("Validators now:\n"
                    + string.Join("\n", validatorsAfter.Select(v => "Validator: " + v.publicKey + ", Stake: " + v.stake + ", Blocks Forged: " + v.blocksForged + ", Penalties: " + v.penalties)));
            else
                report.AppendLine("Validators now: none");

            report.AppendLine();
            report.AppendLine("Chain blocks: " + chainBefore + " -> " + blockchain.getBlocks().Count);
            report.AppendLine("Pending pool: " + poolBefore + " -> " + blockchain.getPendingTransactionsPool().Count);

            string validationMessage;
            blockchain.validateBlockchain(out validationMessage);
            report.AppendLine("Current chain validation: " + validationMessage);

            SetRichText(report.ToString(), false);
        }

        private void checkBalanceButton(object sender, EventArgs e)
        {
            updateText(blockchain.getLastBlock().checkBalance(blockchain.getBlocks(), textBox2.Text).ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_miningPolicyUiReady || comboBox1.SelectedIndex < 0)
                return;

            Blockchain.MiningPolicy miningPolicy = (Blockchain.MiningPolicy)comboBox1.SelectedIndex;
            string minerAddress = textBox2.Text == null ? string.Empty : textBox2.Text.Trim();
            if (miningPolicy == Blockchain.MiningPolicy.AddressPreferential && string.IsNullOrWhiteSpace(minerAddress))
            {
                MessageBox.Show("Please enter a Public ID (miner) to preview address-preferential selection.", "Mining");
                return;
            }

            List<Transaction> transactions = blockchain.getTransactionsForNextBlock(blockchain.getTransactionsPerBlock(), miningPolicy, minerAddress);
            richTextBox1.Text = string.Join("\n", transactions.Select(t => t.ToString()));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            decimal stake;
            if (!decimal.TryParse(textBox8.Text, out stake))
            {
                MessageBox.Show("Please enter a valid number for Stake.", "Proof of Stake");
                return;
            }

            string errorMessage;
            blockchain.addValidator(textBox7.Text, stake, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Proof of Stake");
                return;
            }

            SetRichText("Validator added: " + textBox7.Text.Trim() + " with stake " + stake, false);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<Validator> validators = blockchain.getValidators();
            if (validators.Count == 0)
            {
                SetRichText("No validators registered.", false);
                return;
            }

            SetRichText(string.Join("\n", validators.Select(v => "Validator: " + v.publicKey + ", Stake: " + v.stake + ", Blocks Forged: " + v.blocksForged + ", Penalties: " + v.penalties)), false);
        }
    }
}
