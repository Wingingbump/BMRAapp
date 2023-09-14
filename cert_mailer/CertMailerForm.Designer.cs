namespace cert_mailer
{
    partial class CertMailerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertMailerForm));
            CertBox = new ComboBox();
            RosterLabel = new Label();
            GradesLabel = new Label();
            Roster = new TextBox();
            Grades = new TextBox();
            Certs = new TextBox();
            CertsLabel = new Label();
            Browse1 = new Button();
            Browse2 = new Button();
            Browse3 = new Button();
            Submit = new Button();
            CerticateCheckBox = new CheckBox();
            numericUpDown = new NumericUpDown();
            Minpasslabel = new Label();
            progressBar1 = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)numericUpDown).BeginInit();
            SuspendLayout();
            // 
            // CertBox
            // 
            CertBox.Enabled = false;
            CertBox.FormattingEnabled = true;
            CertBox.Items.AddRange(new object[] { "Default Certificate", "SBA Certificate", "NOAA Certificate", "DOIU Certificate" });
            CertBox.Location = new Point(11, 222);
            CertBox.Name = "CertBox";
            CertBox.Size = new Size(121, 23);
            CertBox.TabIndex = 15;
            CertBox.Tag = "";
            CertBox.Text = "Select Certificate";
            // 
            // RosterLabel
            // 
            RosterLabel.AccessibleName = "BMRA Roster";
            RosterLabel.AutoSize = true;
            RosterLabel.BackColor = SystemColors.Control;
            RosterLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            RosterLabel.ForeColor = Color.Black;
            RosterLabel.Location = new Point(12, 9);
            RosterLabel.Margin = new Padding(4, 0, 4, 0);
            RosterLabel.Name = "RosterLabel";
            RosterLabel.Size = new Size(76, 15);
            RosterLabel.TabIndex = 1;
            RosterLabel.Text = "BMRA Roster";
            // 
            // GradesLabel
            // 
            GradesLabel.AutoSize = true;
            GradesLabel.BackColor = SystemColors.Control;
            GradesLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            GradesLabel.ForeColor = Color.Black;
            GradesLabel.Location = new Point(12, 53);
            GradesLabel.Margin = new Padding(4, 0, 4, 0);
            GradesLabel.Name = "GradesLabel";
            GradesLabel.Size = new Size(138, 15);
            GradesLabel.TabIndex = 2;
            GradesLabel.Text = "BMRA Roster and Grades";
            // 
            // Roster
            // 
            Roster.AllowDrop = true;
            Roster.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Roster.Location = new Point(12, 27);
            Roster.Margin = new Padding(4, 3, 4, 3);
            Roster.Name = "Roster";
            Roster.Size = new Size(254, 23);
            Roster.TabIndex = 3;
            // 
            // Grades
            // 
            Grades.AcceptsTab = true;
            Grades.AllowDrop = true;
            Grades.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Grades.Location = new Point(12, 71);
            Grades.Margin = new Padding(4, 3, 4, 3);
            Grades.Name = "Grades";
            Grades.Size = new Size(254, 23);
            Grades.TabIndex = 4;
            // 
            // Certs
            // 
            Certs.AllowDrop = true;
            Certs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Certs.Location = new Point(12, 115);
            Certs.Margin = new Padding(4, 3, 4, 3);
            Certs.Name = "Certs";
            Certs.Size = new Size(254, 23);
            Certs.TabIndex = 6;
            // 
            // CertsLabel
            // 
            CertsLabel.AutoSize = true;
            CertsLabel.BackColor = SystemColors.Control;
            CertsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CertsLabel.ForeColor = Color.Black;
            CertsLabel.Location = new Point(12, 97);
            CertsLabel.Margin = new Padding(4, 0, 4, 0);
            CertsLabel.Name = "CertsLabel";
            CertsLabel.Size = new Size(112, 15);
            CertsLabel.TabIndex = 5;
            CertsLabel.Text = "Certificate Directory";
            // 
            // Browse1
            // 
            Browse1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Browse1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Browse1.Location = new Point(278, 27);
            Browse1.Margin = new Padding(4, 3, 4, 3);
            Browse1.Name = "Browse1";
            Browse1.Size = new Size(79, 23);
            Browse1.TabIndex = 7;
            Browse1.Text = "Browse";
            Browse1.UseVisualStyleBackColor = true;
            Browse1.Click += Browse1_Click;
            // 
            // Browse2
            // 
            Browse2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Browse2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Browse2.Location = new Point(278, 71);
            Browse2.Margin = new Padding(4, 3, 4, 3);
            Browse2.Name = "Browse2";
            Browse2.Size = new Size(79, 23);
            Browse2.TabIndex = 8;
            Browse2.Text = "Browse";
            Browse2.UseVisualStyleBackColor = true;
            Browse2.Click += Browse2_Click;
            // 
            // Browse3
            // 
            Browse3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Browse3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Browse3.Location = new Point(278, 115);
            Browse3.Margin = new Padding(4, 3, 4, 3);
            Browse3.Name = "Browse3";
            Browse3.Size = new Size(79, 23);
            Browse3.TabIndex = 9;
            Browse3.Text = "Browse";
            Browse3.UseVisualStyleBackColor = true;
            Browse3.Click += Browse3_Click;
            // 
            // Submit
            // 
            Submit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Submit.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Submit.Location = new Point(278, 221);
            Submit.Margin = new Padding(4, 3, 4, 3);
            Submit.Name = "Submit";
            Submit.Size = new Size(79, 23);
            Submit.TabIndex = 10;
            Submit.Text = "Submit";
            Submit.UseVisualStyleBackColor = true;
            Submit.Click += Submit_Click;
            // 
            // CerticateCheckBox
            // 
            CerticateCheckBox.AutoSize = true;
            CerticateCheckBox.Location = new Point(12, 197);
            CerticateCheckBox.Name = "CerticateCheckBox";
            CerticateCheckBox.Size = new Size(122, 19);
            CerticateCheckBox.TabIndex = 11;
            CerticateCheckBox.Text = "Create Certificates";
            CerticateCheckBox.UseVisualStyleBackColor = true;
            CerticateCheckBox.CheckedChanged += CerticateCheckBox_CheckedChanged;
            // 
            // numericUpDown
            // 
            numericUpDown.Location = new Point(12, 159);
            numericUpDown.Name = "numericUpDown";
            numericUpDown.Size = new Size(120, 23);
            numericUpDown.TabIndex = 12;
            numericUpDown.ThousandsSeparator = true;
            numericUpDown.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numericUpDown.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // Minpasslabel
            // 
            Minpasslabel.AutoSize = true;
            Minpasslabel.BackColor = SystemColors.Control;
            Minpasslabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Minpasslabel.ForeColor = Color.Black;
            Minpasslabel.Location = new Point(12, 141);
            Minpasslabel.Margin = new Padding(4, 0, 4, 0);
            Minpasslabel.Name = "Minpasslabel";
            Minpasslabel.Size = new Size(137, 15);
            Minpasslabel.TabIndex = 13;
            Minpasslabel.Text = "Minimum Passing Grade";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(156, 228);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(107, 10);
            progressBar1.TabIndex = 16;
            progressBar1.Visible = false;
            // 
            // CertMailerForm
            // 
            AccessibleName = "Window Name";
            AccessibleRole = AccessibleRole.TitleBar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(369, 258);
            Controls.Add(progressBar1);
            Controls.Add(CertBox);
            Controls.Add(Minpasslabel);
            Controls.Add(numericUpDown);
            Controls.Add(CerticateCheckBox);
            Controls.Add(Submit);
            Controls.Add(Browse3);
            Controls.Add(Browse2);
            Controls.Add(Browse1);
            Controls.Add(Certs);
            Controls.Add(CertsLabel);
            Controls.Add(Grades);
            Controls.Add(Roster);
            Controls.Add(GradesLabel);
            Controls.Add(RosterLabel);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "CertMailerForm";
            RightToLeftLayout = true;
            Text = "Certificate Mailer";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label RosterLabel;
        private Label GradesLabel;
        private TextBox Roster;
        private TextBox Grades;
        private TextBox Certs;
        private Label CertsLabel;
        private Button Browse1;
        private Button Browse2;
        private Button Browse3;
        private Button Submit;
        private CheckBox CerticateCheckBox;
        private NumericUpDown numericUpDown;
        private Label Minpasslabel;
        private ComboBox CertBox;
        public ProgressBar progressBar1;
    }
}