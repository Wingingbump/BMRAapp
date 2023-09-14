namespace cert_mailer
{
    public partial class MissingInfoForm : Form
    {
        public string MissingData { get; set; }
        public MissingInfoForm()
        {
            InitializeComponent();
            MissingData = "";
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Set the form border style to fixed size
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            MissingData = instructorNameTB.Text;
            this.Close();
        }

    }
}
