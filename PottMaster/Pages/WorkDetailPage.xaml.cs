using PottMaster.ViewModels;

namespace PottMaster.Pages;

public partial class WorkDetailPage : ContentPage
{
    public WorkDetailPage(WorkDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
