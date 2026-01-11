using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class WikiDetailPage : ContentPage
{
    public WikiDetailPage(WikiDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}