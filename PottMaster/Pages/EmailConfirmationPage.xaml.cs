using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class EmailConfirmationPage : ContentPage
{
    private readonly EmailConfirmationViewModel _viewModel;

	public EmailConfirmationPage(EmailConfirmationViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.ConfirmEmailAsync();
    }
}