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
            this.buttonValidationEvidence = new System.Windows.Forms.Button();
            this.buttonBenchmarkMining = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.POS = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            PublicKey = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.POS.SuspendLayout();
            this.SuspendLayout();
            // 
            // PublicKey
            // 
            PublicKey.AutoSize = true;
            PublicKey.Location = new System.Drawing.Point(343, 334);
            PublicKey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            PublicKey.Name = "PublicKey";
            PublicKey.Size = new System.Drawing.Size(57, 13);
            PublicKey.TabIndex = 8;
            PublicKey.Text = "Public Key";
            PublicKey.Click += new System.EventHandler(this.label1_Click);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(82, 460);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(43, 13);
            label1.TabIndex = 11;
            label1.Text = "Amount";
            label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(82, 483);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(25, 13);
            label2.TabIndex = 13;
            label2.Text = "Fee";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(657, 314);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 341);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 21);
            this.button1.TabIndex = 1;
            this.button1.Text = "Print";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(81, 342);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(52, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // GenerateWallet
            // 
            this.GenerateWallet.Location = new System.Drawing.Point(596, 331);
            this.GenerateWallet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GenerateWallet.Name = "GenerateWallet";
            this.GenerateWallet.Size = new System.Drawing.Size(74, 41);
            this.GenerateWallet.TabIndex = 4;
            this.GenerateWallet.Text = "Generate Wallet";
            this.GenerateWallet.UseVisualStyleBackColor = true;
            this.GenerateWallet.Click += new System.EventHandler(this.GenerateWallet_Click);
            // 
            // ValidateKey
            // 
            this.ValidateKey.Location = new System.Drawing.Point(588, 381);
            this.ValidateKey.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ValidateKey.Name = "ValidateKey";
            this.ValidateKey.Size = new System.Drawing.Size(82, 21);
            this.ValidateKey.TabIndex = 5;
            this.ValidateKey.Text = "Validate Keys";
            this.ValidateKey.UseVisualStyleBackColor = true;
            this.ValidateKey.Click += new System.EventHandler(this.ValidateKey_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(404, 331);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(188, 20);
            this.textBox2.TabIndex = 6;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(404, 355);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(188, 20);
            this.textBox3.TabIndex = 7;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // PrivateKey
            // 
            this.PrivateKey.AutoSize = true;
            this.PrivateKey.Location = new System.Drawing.Point(339, 359);
            this.PrivateKey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.PrivateKey.Name = "PrivateKey";
            this.PrivateKey.Size = new System.Drawing.Size(61, 13);
            this.PrivateKey.TabIndex = 9;
            this.PrivateKey.Text = "Private Key";
            this.PrivateKey.Click += new System.EventHandler(this.label2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(6, 450);
            this.button3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 43);
            this.button3.TabIndex = 10;
            this.button3.Text = "Create Transaction";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button_newTransaction);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(134, 456);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(46, 20);
            this.textBox4.TabIndex = 12;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged_1);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(134, 483);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(46, 20);
            this.textBox5.TabIndex = 14;
            this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(187, 483);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Receiver Key";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(262, 480);
            this.textBox6.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(356, 20);
            this.textBox6.TabIndex = 16;
            this.textBox6.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 368);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 36);
            this.button2.TabIndex = 17;
            this.button2.Text = "Create Block";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button_createBlock);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(82, 370);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(50, 34);
            this.button4.TabIndex = 18;
            this.button4.Text = "Read All";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button_readAll);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(588, 412);
            this.button5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(85, 19);
            this.button5.TabIndex = 19;
            this.button5.Text = "Read All Transactions";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.readAllTransactions);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(503, 381);
            this.button6.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(84, 23);
            this.button6.TabIndex = 20;
            this.button6.Text = "Check Balance";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.checkBalanceButton);
            // 
            // buttonValidateBlockchain
            // 
            this.buttonValidateBlockchain.Location = new System.Drawing.Point(6, 412);
            this.buttonValidateBlockchain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonValidateBlockchain.Name = "buttonValidateBlockchain";
            this.buttonValidateBlockchain.Size = new System.Drawing.Size(72, 34);
            this.buttonValidateBlockchain.TabIndex = 21;
            this.buttonValidateBlockchain.Text = "Validate Blockchain";
            this.buttonValidateBlockchain.UseVisualStyleBackColor = true;
            this.buttonValidateBlockchain.Click += new System.EventHandler(this.validateBlockchain_Click);
            // 
            // buttonValidationEvidence
            // 
            this.buttonValidationEvidence.Location = new System.Drawing.Point(404, 449);
            this.buttonValidationEvidence.Name = "buttonValidationEvidence";
            this.buttonValidationEvidence.Size = new System.Drawing.Size(90, 27);
            this.buttonValidationEvidence.TabIndex = 25;
            this.buttonValidationEvidence.Text = "Validation Evidence";
            this.buttonValidationEvidence.UseVisualStyleBackColor = true;
            this.buttonValidationEvidence.Click += new System.EventHandler(this.buttonValidationEvidence_Click);
            // 
            // buttonBenchmarkMining
            // 
            this.buttonBenchmarkMining.Location = new System.Drawing.Point(404, 381);
            this.buttonBenchmarkMining.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonBenchmarkMining.Name = "buttonBenchmarkMining";
            this.buttonBenchmarkMining.Size = new System.Drawing.Size(90, 34);
            this.buttonBenchmarkMining.TabIndex = 22;
            this.buttonBenchmarkMining.Text = "Benchmark Mining";
            this.buttonBenchmarkMining.UseVisualStyleBackColor = true;
            this.buttonBenchmarkMining.Click += new System.EventHandler(this.button_benchmarkMining_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(503, 412);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(81, 21);
            this.comboBox1.TabIndex = 23;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // POS
            // 
            this.POS.Controls.Add(this.comboBox2);
            this.POS.Controls.Add(this.textBox8);
            this.POS.Controls.Add(this.textBox7);
            this.POS.Controls.Add(this.button8);
            this.POS.Controls.Add(this.button7);
            this.POS.Controls.Add(this.label6);
            this.POS.Controls.Add(this.label5);
            this.POS.Controls.Add(this.label4);
            this.POS.Location = new System.Drawing.Point(138, 331);
            this.POS.Name = "POS";
            this.POS.Size = new System.Drawing.Size(200, 120);
            this.POS.TabIndex = 24;
            this.POS.TabStop = false;
            this.POS.Text = "Proof-Of-Stake";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Validator Key";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Consensus Mode";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Stake";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(6, 91);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(86, 23);
            this.button7.TabIndex = 3;
            this.button7.Text = "Add Validator";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(98, 91);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(92, 23);
            this.button8.TabIndex = 4;
            this.button8.Text = "Show Validators";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(92, 39);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(104, 20);
            this.textBox7.TabIndex = 5;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(94, 68);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(100, 20);
            this.textBox8.TabIndex = 6;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(92, 12);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(104, 21);
            this.comboBox2.TabIndex = 7;
            // 
            // BlockchainApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(681, 506);
            this.Controls.Add(this.POS);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.buttonValidationEvidence);
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
            this.Name = "BlockchainApp";
            this.Text = "Blockchain App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.POS.ResumeLayout(false);
            this.POS.PerformLayout();
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
        private System.Windows.Forms.Button buttonValidationEvidence;
        private System.Windows.Forms.Button buttonBenchmarkMining;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox POS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}

