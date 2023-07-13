using cert_mailer;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace BMRAapplication.Views;

public sealed partial class MailerPage : Page
{
    public string rosterPath
    {
        get; set;
    }
    public string certPath
    {
        get; set;
    }
    public string gradesPath
    {
        get; set;
    }

    public MailerPage()
    {
        InitializeComponent();

        rosterPath = string.Empty;
        certPath = string.Empty;
        gradesPath = string.Empty;
    }

    private void Box_DragEnter(object sender, DragEventArgs e)
    {
        var grid = (Grid)sender;
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Drop file here";
            e.DragUIOverride.IsCaptionVisible = true;
            grid.Background = new SolidColorBrush(Colors.LightYellow);
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;
        }
    }

    private void Box_DragOver(object sender, DragEventArgs e)
    {
        var grid = (Grid)sender;
        if (!e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.None;
        }
    }

    private async void Box_Drop(object sender, DragEventArgs e)
    {
        var grid = (Grid)sender;
        grid.Background = ((SolidColorBrush)grid.Background).Color == Colors.LightYellow
            ? ((SolidColorBrush)grid.Background).Color == Colors.LightGray
                ? new SolidColorBrush(Colors.LightBlue)
                : new SolidColorBrush(Colors.LightGreen)
            : new SolidColorBrush(Colors.LightGray);

        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 1)
            {
                if (items[0] is StorageFile file)
                {
                    var textBlock = (TextBlock)grid.Children[0];
                    textBlock.Text = file.Name;
                    AdjustTextToFit(textBlock);
                    if (grid.Name == "RosterBox")
                    {
                        rosterPath = file.Path;
                    }
                    else if (grid.Name == "GradesBox")
                    {
                        gradesPath = file.Path;
                    }
                }
                else if (items[0] is StorageFolder folder)
                {
                    var textBlock = (TextBlock)grid.Children[0];
                    textBlock.Text = folder.DisplayName;
                    AdjustTextToFit(textBlock);
                    if (grid.Name == "CertBox")
                    {
                        certPath = folder.Path;
                    }
                }
            }
        }
    }

    private void AdjustTextToFit(TextBlock textBlock)
    {
        double maxWidth = textBlock.Width;
        double maxHeight = textBlock.Height;

        textBlock.MaxLines = 1;
        textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
    }

    private void CreateCertificatesCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        CertificatesComboBox.IsEnabled = true;
    }

    private void CreateCertificatesCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        CertificatesComboBox.IsEnabled = false;
    }

    private void MinimumGradeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Get the entered text from the TextBox
        string enteredText = MinimumGradeTextBox.Text;

        // Check if the entered text is empty
        if (string.IsNullOrEmpty(enteredText))
        {
            return;
        }

        // Validate that the entered text is a valid integer
        if (!int.TryParse(enteredText, out int value))
        {
            // If the entered text is not a valid integer, remove the last entered character if the text box is not empty
            int caretIndex = MinimumGradeTextBox.SelectionStart;
            if (caretIndex > 0)
            {
                MinimumGradeTextBox.Text = enteredText.Remove(caretIndex - 1, 1);
                MinimumGradeTextBox.SelectionStart = caretIndex - 1;
            }
        }
        else
        {
            // Check if the entered value is within the desired range
            if (value < 0 || value > 100)
            {
                // If the entered value is outside the valid range, set it to the closest valid value
                int validValue = Math.Clamp(value, 0, 100);
                MinimumGradeTextBox.Text = validValue.ToString();
            }
        }
    }

    private void MinimumGradeTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        // Get the pressed key
        var key = e.Key;

        // Check if the pressed key is a digit (0-9)
        bool isDigit = (key >= VirtualKey.Number0 && key <= VirtualKey.Number9) ||
                       (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9);

        // Check if the pressed key is a control key (e.g., Backspace, Delete, Arrow keys)
        bool isControlKey = key == VirtualKey.Back ||
                            key == VirtualKey.Delete ||
                            key == VirtualKey.Left ||
                            key == VirtualKey.Right ||
                            key == VirtualKey.Home ||
                            key == VirtualKey.End;

        // Check if the pressed key is the space bar
        bool isSpaceBar = key == VirtualKey.Space;

        // Check if the text box is empty
        bool isTextBoxEmpty = MinimumGradeTextBox.Text.Length == 0;

        // Allow the input of digits, control keys, and space bar if the text box is not empty
        if (!isDigit && !isControlKey && (!isSpaceBar || isTextBoxEmpty))
        {
            // Prevent the input of non-numeric characters and space bar by marking the event as handled
            e.Handled = true;
        }
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (certPath == string.Empty || rosterPath == string.Empty || gradesPath == string.Empty)
        {
            ShowErrorDialog("Error: Not all fields filled");
        }
        else
        {
            try
            {
                FileInfo rosterInfo = new FileInfo(rosterPath);
                FileInfo gradesInfo = new FileInfo(gradesPath);
                EnumCertificateType.CertificateType certificateType = EnumCertificateType.CertificateType.Default;
                ProgressBar.Visibility = 0;
                ProgressBar.Value = 5;
                if ((bool)CreateCertificatesCheckBox.IsChecked)
                {
                    var certType = CertificatesComboBox.SelectedItem;
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
                var minimumGrade = int.Parse(MinimumGradeTextBox.Text);
                DataRead reader = new DataRead(rosterInfo, gradesInfo, certPath, (bool)CreateCertificatesCheckBox.IsChecked, certificateType, (int)minimumGrade);
                ProgressBar.Value = 10;
                string courseName = reader.Course.CourseName;
                string courseId = reader.Course.CourseId;

                foreach (Student student in reader.Course.Students)
                {
                    if (student.Pass == true)
                    {
                        EmailBuilder message = new EmailBuilder(student.Email, courseName, courseId, student.Certification);
                        message.CreateDraft();
                        ProgressBar.Value += 2;
                    }
                }
                ProgressBar.Value = 100;
            }
            catch (Exception ex)
            {
                ShowErrorDialog("An error occured: " + ex.Message);
            }

        }
    }
    private async void ShowErrorDialog(string message)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = this.Content.XamlRoot,
            Title = "Error",
            Content = message,
            CloseButtonText = "OK"
        };

        await dialog.ShowAsync();
    }


}

