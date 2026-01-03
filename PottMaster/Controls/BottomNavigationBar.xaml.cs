using Microsoft.Maui.Controls;

namespace PottMaster.Controls;

public partial class BottomNavigationBar : ContentView
{
    private string _currentPage = "MainPage";

    public BottomNavigationBar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        UpdateActiveTab(_currentPage);
    }

    private async void OnMainTapped(object sender, EventArgs e)
    {
        await AnimateTap(MainBorder);
        _currentPage = "MainPage";
        UpdateActiveTab(_currentPage);
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnInventoryTapped(object sender, EventArgs e)
    {
        await AnimateTap(InventoryBorder);
        _currentPage = "GlazeInventoryPage";
        UpdateActiveTab(_currentPage);
        await Shell.Current.GoToAsync("//GlazeInventoryPage");
    }

    private async void OnWikiTapped(object sender, EventArgs e)
    {
        await AnimateTap(WikiBorder);
        _currentPage = "WikiPage";
        UpdateActiveTab(_currentPage);
        await Shell.Current.GoToAsync("//WikiPage");
    }

    private async void OnAnalyticsTapped(object sender, EventArgs e)
    {
        await AnimateTap(AnalyticsBorder);
        _currentPage = "AnalyticsPage";
        UpdateActiveTab(_currentPage);
        await Shell.Current.GoToAsync("//AnalyticsPage");
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await AnimateTap(ProfileBorder);
        _currentPage = "ProfilePage";
        UpdateActiveTab(_currentPage);
        await Shell.Current.GoToAsync("//ProfilePage");
    }

    private async Task AnimateTap(Border border)
    {
        await border.ScaleTo(0.95, 50, Easing.CubicOut);
        await border.ScaleTo(1.0, 50, Easing.CubicIn);
    }

    private void UpdateActiveTab(string pageName)
    {
        var primaryColor = Application.Current.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#D0BCFF")
            : Color.FromArgb("#6750A4");

        var primaryContainerColor = Application.Current.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#4F378B")
            : Color.FromArgb("#EADDFF");

        var onSurfaceVariantColor = Application.Current.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#CAC4D0")
            : Color.FromArgb("#49454F");

        ResetAllTabs(onSurfaceVariantColor);

        switch (pageName)
        {
            case "MainPage":
                SetActiveTabStyle(MainIconContainer, MainLabel, primaryContainerColor, primaryColor);
                break;
            case "GlazeInventoryPage":
                SetActiveTabStyle(InventoryIconContainer, InventoryLabel, primaryContainerColor, primaryColor);
                break;
            case "WikiPage":
                SetActiveTabStyle(WikiIconContainer, WikiLabel, primaryContainerColor, primaryColor);
                break;
            case "AnalyticsPage":
                SetActiveTabStyle(AnalyticsIconContainer, AnalyticsLabel, primaryContainerColor, primaryColor);
                break;
            case "ProfilePage":
                SetActiveTabStyle(ProfileIconContainer, ProfileLabel, primaryContainerColor, primaryColor);
                break;
        }
    }

    private void ResetAllTabs(Color defaultColor)
    {
        MainIconContainer.BackgroundColor = Colors.Transparent;
        MainLabel.TextColor = defaultColor;

        InventoryIconContainer.BackgroundColor = Colors.Transparent;
        InventoryLabel.TextColor = defaultColor;

        WikiIconContainer.BackgroundColor = Colors.Transparent;
        WikiLabel.TextColor = defaultColor;

        AnalyticsIconContainer.BackgroundColor = Colors.Transparent;
        AnalyticsLabel.TextColor = defaultColor;

        ProfileIconContainer.BackgroundColor = Colors.Transparent;
        ProfileLabel.TextColor = defaultColor;
    }

    private void SetActiveTabStyle(Border iconContainer, Label label, Color backgroundColor, Color textColor)
    {
        iconContainer.BackgroundColor = backgroundColor;
        label.TextColor = textColor;
    }
}
