using PottMaster.Services;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace PottMaster
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public static void SetServiceProvider(IServiceProvider services)
        {
            Services = services;
        }

        public App()
        {
            InitializeComponent();
        }
protected override Window CreateWindow(IActivationState? activationState)
{
    return new Window(new AppShell());
}

    }
}