using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class GlazeDetailPage : ContentPage
{
	private readonly GlazeDetailViewModel _viewModel;

	public GlazeDetailPage(GlazeDetailViewModel viewModel)
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
