using Microsoft.Extensions.DependencyInjection;
using PottMaster.Services;
using PottMaster.ViewModels;

namespace PottMaster
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = App.Services.GetRequiredService<MainViewModel>();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}
