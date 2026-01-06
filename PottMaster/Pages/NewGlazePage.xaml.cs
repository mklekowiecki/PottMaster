using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class NewGlazePage : ContentPage
{
	private readonly NewGlazeViewModel _viewModel;

	public NewGlazePage(NewGlazeViewModel viewModel)
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
