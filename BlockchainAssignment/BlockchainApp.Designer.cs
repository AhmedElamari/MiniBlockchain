namespace BlockchainAssignment
{
    partial class BlockchainApp
    {
      
        private System.ComponentModel.IContainer components = null;

       
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

     

        
        private void InitializeComponent()
        {
            System.Windows.Forms.Label PublicKey;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.GenerateWallet = new System.Windows.Forms.Button();
            this.ValidateKey = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.PrivateKey = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.buttonValidateBlockchain = new System.Windows.Forms.Button();
            this.buttonBenchmarkMining = new System.Windows.Forms.Button();
            PublicKey = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PublicKey
            // 
            PublicKey.AutoSize = true;
            PublicKey.Location = new System.Drawing.Point(663, 639);
            PublicKey.Name = "PublicKey";
            PublicKey.Size = new System.Drawing.Size(114, 25);
            PublicKey.TabIndex = 8;
            PublicKey.Text = "Public Key";
            PublicKey.Click += new System.EventHandler(this.label1_Click);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(173, 830);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(85, 25);
            label1.TabIndex = 11;
            label1.Text = "Amount";
            label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(173, 875);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(49, 25);
            label2.TabIndex = 13;
            label2.Text = "Fee";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.richTextBox1.Location = new System.Drawing.Point(24, 23);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1310, 600);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 636);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 41);
            this.button1.TabIndex = 1;
            this.button1.Text = "Print";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(162, 641);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 31);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // GenerateWallet
            // 
            this.GenerateWallet.Location = new System.Drawing.Point(1186, 636);
            this.GenerateWallet.Name = "GenerateWallet";
            this.GenerateWallet.Size = new System.Drawing.Size(148, 78);
            this.GenerateWallet.TabIndex = 4;
            this.GenerateWallet.Text = "Generate Wallet";
            this.GenerateWallet.UseVisualStyleBackColor = true;
            this.GenerateWallet.Click += new System.EventHandler(this.GenerateWallet_Click);
            // 
            // ValidateKey
            // 
            this.ValidateKey.Location = new System.Drawing.Point(1169, 732);
            this.ValidateKey.Name = "ValidateKey";
            this.ValidateKey.Size = new System.Drawing.Size(165, 41);
            this.ValidateKey.TabIndex = 5;
            this.ValidateKey.Text = "Validate Keys";
            this.ValidateKey.UseVisualStyleBackColor = true;
            this.ValidateKey.Click += new System.EventHandler(this.ValidateKey_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(783, 636);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(371, 31);
            this.textBox2.TabIndex = 6;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(783, 683);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(371, 31);
            this.textBox3.TabIndex = 7;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // PrivateKey
            // 
            this.PrivateKey.AutoSize = true;
            this.PrivateKey.Location = new System.Drawing.Point(655, 686);
            this.PrivateKey.Name = "PrivateKey";
            this.PrivateKey.Size = new System.Drawing.Size(122, 25);
            this.PrivateKey.TabIndex = 9;
            this.PrivateKey.Text = "Private Key";
            this.PrivateKey.Click += new System.EventHandler(this.label2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 830);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(143, 83);
            this.button3.TabIndex = 10;
            this.button3.Text = "Create Transaction";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button_newTransaction);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(276, 824);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(87, 31);
            this.textBox4.TabIndex = 12;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged_1);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(276, 875);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(87, 31);
            this.textBox5.TabIndex = 14;
            this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(369, 878);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 25);
            this.label3.TabIndex = 15;
            this.label3.Text = "Receiver Key";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(515, 878);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(709, 31);
            this.textBox6.TabIndex = 16;
            this.textBox6.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 686);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 70);
            this.button2.TabIndex = 17;
            this.button2.Text = "Create Block";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button_createBlock);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(162, 691);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 65);
            this.button4.TabIndex = 18;
            this.button4.Text = "Read All";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button_readAll);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1169, 792);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(165, 30);
            this.button5.TabIndex = 19;
            this.button5.Text = "Read All Transactions";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.readAllTransactions);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(996, 732);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(168, 44);
            this.button6.TabIndex = 20;
            this.button6.Text = "Check Balance";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.checkBalanceButton);
            // 
            // buttonValidateBlockchain
            // 
            this.buttonValidateBlockchain.Location = new System.Drawing.Point(13, 762);
            this.buttonValidateBlockchain.Name = "buttonValidateBlockchain";
            this.buttonValidateBlockchain.Size = new System.Drawing.Size(143, 65);
            this.buttonValidateBlockchain.TabIndex = 21;
            this.buttonValidateBlockchain.Text = "Validate Blockchain";
            this.buttonValidateBlockchain.UseVisualStyleBackColor = true;
            this.buttonValidateBlockchain.Click += new System.EventHandler(this.validateBlockchain_Click);
            // 
            // buttonBenchmarkMining
            // 
            this.buttonBenchmarkMining.Location = new System.Drawing.Point(801, 722);
            this.buttonBenchmarkMining.Name = "buttonBenchmarkMining";
            this.buttonBenchmarkMining.Size = new System.Drawing.Size(180, 65);
            this.buttonBenchmarkMining.TabIndex = 22;
            this.buttonBenchmarkMining.Text = "Benchmark Mining";
            this.buttonBenchmarkMining.UseVisualStyleBackColor = true;
            this.buttonBenchmarkMining.Click += new System.EventHandler(this.button_benchmarkMining_Click);
            // 
            // BlockchainApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1362, 925);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.buttonBenchmarkMining);
            this.Controls.Add(this.buttonValidateBlockchain);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(label2);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.PrivateKey);
            this.Controls.Add(PublicKey);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ValidateKey);
            this.Controls.Add(this.GenerateWallet);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "BlockchainApp";
            this.Text = "Blockchain App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button GenerateWallet;
        private System.Windows.Forms.Button ValidateKey;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label PrivateKey;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button buttonValidateBlockchain;
        private System.Windows.Forms.Button buttonBenchmarkMining;
    }
}

