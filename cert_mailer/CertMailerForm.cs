namespace cert_mailer
{
    public partial class CertMailerForm : Form
    {

        private string? rosterPath = null;
        private string? gradesPath = null;
        private string? certPath = null;
        private bool certEnabled = false;

        public CertMailerForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(380, 300);
            this.MaximumSize = new Size(800, 800);
        }

        private void Browse1_Click(object sender, EventArgs e)
        {
            // Show the file explorer
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                dialog.Title = "Select a BMRA Roster Excel";

                // Update the "Roster" text box with the selected excel path
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    rosterPath = dialog.FileName;
                    Roster.Text = Path.GetFileName(dialog.FileName);
                }
            }
        }

        private void Browse2_Click(object sender, EventArgs e)
        {
            // Show the file explorer
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                dialog.Title = "Select a BMRA Roster and Grades Excel";

                // Update the "Grades" text box with the selected excel path
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    gradesPath = dialog.FileName;
                    Grades.Text = Path.GetFileName(dialog.FileName);
                }
            }
        }


        private void Browse3_Click(object sender, EventArgs e)
        {
            // Show the file explorer and get the selected directory
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Certificate Directory";
                dialog.ShowNewFolderButton = false;
                DialogResult result = dialog.ShowDialog();

                // Update the "Cert" text box with the selected directory path
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    certPath = dialog.SelectedPath;
                    Certs.Text = Path.GetFileName(dialog.SelectedPath);
                    Certs.Text = certPath;
                }
            }
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            if (rosterPath == null || certPath == null || gradesPath == null)
            {
                MessageBox.Show("Error: Not all fields filled", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    FileInfo rosterInfo = new FileInfo(rosterPath);
                    FileInfo gradesInfo = new FileInfo(gradesPath);
                    EnumCertificateType.CertificateType certificateType = EnumCertificateType.CertificateType.None;
                    progressBar1.Visible = true;
                    progressBar1.Value = 5;
                    if (certEnabled)
                    {
                        string? certType = CertBox.SelectedItem.ToString();
                        switch (certType)
                        {
                            case "Default Certificate":
                                certificateType = EnumCertificateType.CertificateType.Default;
                                break;
                            case "SBA Certificate":
                                certificateType = EnumCertificateType.CertificateType.SBA;
                                break;
                            case "NOAA Certificate":
                                certificateType = EnumCertificateType.CertificateType.NOAA;
                                break;
                            case "DOIU Certificate":
                                certificateType = EnumCertificateType.CertificateType.DOIU;
                                break;
                        }
                    }
                    DataRead reader = new DataRead(rosterInfo, gradesInfo, certPath, certEnabled, certificateType, (int)numericUpDown.Value);
                    progressBar1.Value = 10;
                    string courseName = reader.Course.CourseName;
                    string courseId = reader.Course.CourseId;

                    foreach (Student student in reader.Course.Students)
                    {
                        if (student.Pass == true)
                        {
                            EmailBuilder message = new EmailBuilder(student.Email, courseName, courseId, student.Certification, student.Grade);
                            message.CreateDraft();
                            progressBar1.Value += 2;
                        }
                    }
                    progressBar1.Value = 100;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CerticateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            certEnabled = !certEnabled;
            // Hide or show the CertBox checkbox control based on the certEnabled variable
            CertBox.Enabled = certEnabled;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // do nothing
        }


    }
}