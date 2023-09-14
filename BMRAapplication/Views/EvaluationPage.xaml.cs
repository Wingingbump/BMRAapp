using BMRAapplication.ViewModels;
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

public sealed partial class EvaluationPage : Page
{
    public string evalPath
    {
        get; set;
    }
    public string EOCPath
    {
        get; set;
    }

    public EvaluationViewModel ViewModel
    {
        get;
    }

    public EvaluationPage()
    {
        ViewModel = App.GetService<EvaluationViewModel>();
        evalPath = string.Empty;
        EOCPath = string.Empty; 
        InitializeComponent();
    }
    // Submit Button
    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        // Check if all fields are filled
        if (checkAllFields())
        {
            ShowErrorDialog("Please fill all Fields");
        }
        else
        {
            try
            {
                ProgressBar.Visibility = 0;
                ProgressBar.Value = 10;
                FileInfo evalFile = new FileInfo(evalPath);
                ComboBoxItem selectedComboBoxItem = OptionsComboBox.SelectedItem as ComboBoxItem;
                DateTimeOffset startSelectedDate = StartDatePicker.SelectedDate ?? DateTimeOffset.MinValue; // Assume MinValue if no date is selected
                DateTime startDate = startSelectedDate.DateTime;
                DateTimeOffset endSelectedDate = EndDatePicker.SelectedDate ?? DateTimeOffset.MinValue; // Assume MinValue if no date is selected
                DateTime endDate = endSelectedDate.DateTime;
                Evaluations evaluations = new Evaluations(evalFile,
                    EOCPath,
                    selectedComboBoxItem.Content.ToString(),
                    CourseCodeTextBox.Text,
                    startDate,
                    endDate,
                    InstructorTextBox.Text,
                    AgencyTextBox.Text,
                    CourseNameTextBox.Text,
                    AttendanceTextBox.Text,
                    CourseABVTextBox.Text);
                ProgressBar.Value = 100;
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
    }

    private bool checkAllFields()
    {
        if (evalPath == null || EOCPath == null
            || string.IsNullOrEmpty(CourseCodeTextBox.Text)
            || string.IsNullOrEmpty(InstructorTextBox.Text)
            || string.IsNullOrEmpty(AgencyTextBox.Text)
            || string.IsNullOrEmpty(CourseNameTextBox.Text)
            || StartDatePicker.SelectedDate == null
            || EndDatePicker.SelectedDate == null
            || string.IsNullOrEmpty(AttendanceTextBox.Text))
        {
            return true;
        }
        return false;
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
                    if (grid.Name == "EvalBox")
                    {
                        evalPath = file.Path;
                    }
                }
                else if (items[0] is StorageFolder folder)
                {
                    var textBlock = (TextBlock)grid.Children[0];
                    textBlock.Text = folder.DisplayName;
                    AdjustTextToFit(textBlock);
                    if (grid.Name == "EOCBox")
                    {
                        EOCPath = folder.Path;
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


    private void MinimumGradeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Get the entered text from the TextBox
        string enteredText = AttendanceTextBox.Text;

        // Check if the entered text is empty
        if (string.IsNullOrEmpty(enteredText))
        {
            return;
        }

        // Validate that the entered text is a valid integer
        if (!int.TryParse(enteredText, out int value))
        {
            // If the entered text is not a valid integer, remove the last entered character if the text box is not empty
            int caretIndex = AttendanceTextBox.SelectionStart;
            if (caretIndex > 0)
            {
                AttendanceTextBox.Text = enteredText.Remove(caretIndex - 1, 1);
                AttendanceTextBox.SelectionStart = caretIndex - 1;
            }
        }
        else
        {
            // Check if the entered value is within the desired range
            if (value < 0 || value > 100)
            {
                // If the entered value is outside the valid range, set it to the closest valid value
                int validValue = Math.Clamp(value, 0, 100);
                AttendanceTextBox.Text = validValue.ToString();
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
        bool isTextBoxEmpty = AttendanceTextBox.Text.Length == 0;

        // Allow the input of digits, control keys, and space bar if the text box is not empty
        if (!isDigit && !isControlKey && (!isSpaceBar || isTextBoxEmpty))
        {
            // Prevent the input of non-numeric characters and space bar by marking the event as handled
            e.Handled = true;
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
