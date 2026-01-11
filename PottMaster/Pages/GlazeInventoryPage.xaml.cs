using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class GlazeInventoryPage : ContentPage
{
	private readonly GlazeInventoryViewModel _viewModel;

	public GlazeInventoryPage(GlazeInventoryViewModel viewModel)
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
