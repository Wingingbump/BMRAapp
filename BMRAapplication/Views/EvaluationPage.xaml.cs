using BMRAapplication.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BMRAapplication.Views;

public sealed partial class EvaluationPage : Page
{
    public EvaluationViewModel ViewModel
    {
        get;
    }

    public EvaluationPage()
    {
        ViewModel = App.GetService<EvaluationViewModel>();
        InitializeComponent();
    }
}
