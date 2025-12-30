using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PottMaster.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private int count;

    [ObservableProperty]
    private string counterText = "Click me";

    [RelayCommand]
    private void OnCounterClicked()
    {
        Count++;
        CounterText = Count == 1 ? $"Clicked {Count} time" : $"Clicked {Count} times";
    }
}