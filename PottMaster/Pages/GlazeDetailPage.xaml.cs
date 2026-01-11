using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class GlazeDetailPage : ContentPage
{
	public GlazeDetailPage(GlazeDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
