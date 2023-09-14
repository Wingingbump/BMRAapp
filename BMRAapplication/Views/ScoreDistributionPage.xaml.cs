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

public sealed partial class ScoreDistributionPage : Page
{
    public string rosterPath
    {
        get; set;
    }
    public string gradesPath
    {
        get; set;
    }

    public ScoreDistributionPage()
    {
        InitializeComponent();

        rosterPath = string.Empty;
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

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (rosterPath == string.Empty || gradesPath == string.Empty)
        {
            ShowErrorDialog("Error: Not all fields filled");
        }
        else
        {
            try
            {
                FileInfo rosterInfo = new FileInfo(rosterPath);
                FileInfo gradesInfo = new FileInfo(gradesPath);
                ProgressBar.Visibility = 0;
                ProgressBar.Value = 10;
                ScoreDistribution scoreDistribution = new ScoreDistribution(gradesInfo, rosterInfo);
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

