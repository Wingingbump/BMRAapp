namespace cert_mailer
{
    partial class MissingInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MissingInfoForm));
            instructorLabel = new Label();
            instructorNameTB = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // instructorLabel
            // 
            instructorLabel.AutoSize = true;
            instructorLabel.Location = new Point(12, 9);
            instructorLabel.Name = "instructorLabel";
            instructorLabel.Size = new Size(123, 15);
            instructorLabel.TabIndex = 0;
            instructorLabel.Text = "Enter Instructor Name";
            // 
            // instructorNameTB
            // 
            instructorNameTB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            instructorNameTB.Location = new Point(12, 27);
            instructorNameTB.Name = "instructorNameTB";
            instructorNameTB.Size = new Size(254, 23);
            instructorNameTB.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(12, 56);
            button1.Name = "button1";
            button1.Size = new Size(106, 20);
            button1.TabIndex = 2;
            button1.Text = "Submit";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Submit_Click;
            // 
            // MissingInfoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(286, 86);
            Controls.Add(button1);
            Controls.Add(instructorNameTB);
            Controls.Add(instructorLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MissingInfoForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Missing Information";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label instructorLabel;
        private TextBox instructorNameTB;
        private Button button1;
    }
}