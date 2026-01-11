using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class WikiSubmitPage : ContentPage
{
    public WikiSubmitPage(WikiSubmitViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is WikiSubmitViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}