using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class NewWorkPage : ContentPage
{
    private readonly NewWorkViewModel _viewModel;

    public NewWorkPage(NewWorkViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
