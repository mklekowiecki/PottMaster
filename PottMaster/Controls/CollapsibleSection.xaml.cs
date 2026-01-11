using CommunityToolkit.Mvvm.Input;

namespace PottMaster.Controls;

public partial class CollapsibleSection : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CollapsibleSection), string.Empty,
            propertyChanged: OnTitleChanged);

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(CollapsibleSection), false,
   propertyChanged: OnIsExpandedChanged);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(CollapsibleSection),
     propertyChanged: OnContentChanged);

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

        // Set initial values after InitializeComponent
        UpdateTitle();
        UpdateExpanded();
        UpdateContent();
    }

    private void Toggle()
    {
        IsExpanded = !IsExpanded;
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CollapsibleSection section)
        {
            section.UpdateTitle();
        }
    }

    private static void OnIsExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CollapsibleSection section)
        {
            section.UpdateExpanded();
        }
    }

    private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CollapsibleSection section)
        {
            section.UpdateContent();
        }
    }

    private void UpdateTitle()
    {
        if (TitleLabel != null)
        {
            TitleLabel.Text = Title;
        }
    }

    private void UpdateExpanded()
    {
        if (ExpandIconLabel != null)
        {
            ExpandIconLabel.Text = IsExpanded ? "−" : "+";
        }

        if (ContentPresenter != null)
        {
            ContentPresenter.IsVisible = IsExpanded;

            // Optional: Add animation here
            if (IsExpanded)
            {
                ContentPresenter.FadeTo(1, 200);
            }
            else
            {
                ContentPresenter.FadeTo(0, 100);
            }
        }
    }

    private void UpdateContent()
    {
        if (ContentPresenter != null && Content != null)
        {
            ContentPresenter.Content = Content;
        }
    }
}
