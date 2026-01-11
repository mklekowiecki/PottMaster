using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class WikiPage : ContentPage
{
	private readonly WikiViewModel _viewModel;

	public WikiPage(WikiViewModel viewModel)
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
