using CommunityToolkit.Mvvm.Input;

namespace PottMaster.Controls;

public partial class CollapsibleSection : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CollapsibleSection), string.Empty);

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(CollapsibleSection), false,
            propertyChanged: OnIsExpandedChanged);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(CollapsibleSection));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public new View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public IRelayCommand ToggleCommand { get; }

    public CollapsibleSection()
    {
        InitializeComponent();
        ToggleCommand = new RelayCommand(Toggle);
    }

    private void Toggle()
    {
        IsExpanded = !IsExpanded;
    }

    private static void OnIsExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CollapsibleSection section && newValue is bool isExpanded)
        {
            // Optional: Add animation here
            if (isExpanded)
            {
                section.Content?.FadeTo(1, 200);
            }
            else
            {
                section.Content?.FadeTo(0, 100);
            }
        }
    }
}
