# Glaze UI Implementation Guide

## Overview
This document provides detailed guidance on implementing the UI for the enhanced glaze management system in PottMaster, based on the updated database schema with comprehensive JSONB properties.

## Architecture Principles

### 1. Progressive Disclosure
- **Level 1 (List)**: Show only essential info (name, manufacturer, cone, type, quantity)
- **Level 2 (Quick View)**: Add appearance tags, food safety, favorite status
- **Level 3 (Detail Page)**: Full comprehensive data in collapsible sections

### 2. Mobile-First Design
- Touch-friendly tap targets (minimum 44x44 points)
- Swipe gestures for common actions (favorite, delete)
- Bottom sheets for quick actions
- Pull-to-refresh for data updates

### 3. Offline-First UX
- Show sync status indicators
- Allow full CRUD operations offline
- Visual feedback for pending changes
- Conflict resolution UI when needed

## UI Components

### 1. Glaze List Item (GlazeInventoryPage)

```xaml
<!-- Glaze List Item Template -->
<SwipeView>
    <SwipeView.RightItems>
        <SwipeItems>
            <SwipeItem Text="Delete" 
                       BackgroundColor="Red"
                       Command="{Binding DeleteCommand}"
                       CommandParameter="{Binding .}"/>
        </SwipeItems>
    </SwipeView.RightItems>
    
    <Border Style="{StaticResource CardBorderStyle}" Padding="16">
        <Grid ColumnDefinitions="Auto,*,Auto" RowDefinitions="Auto,Auto,Auto" ColumnSpacing="12">
            <!-- Favorite Star -->
            <Image Grid.Column="0" Grid.RowSpan="3"
                   Source="{Binding IsFavorite, Converter={StaticResource BoolToStarIconConverter}}"
                   WidthRequest="24" HeightRequest="24"
                   VerticalOptions="Start">
                <Image.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding ToggleFavoriteCommand}" 
                                         CommandParameter="{Binding .}"/>
                </Image.GestureRecognizers>
            </Image>
            
            <!-- Name & Manufacturer -->
            <Label Grid.Column="1" Grid.Row="0"
                   Text="{Binding Name}"
                   FontSize="16"
                   FontAttributes="Bold"/>
            <Label Grid.Column="1" Grid.Row="1"
                   Text="{Binding Manufacturer}"
                   FontSize="12"
                   Opacity="0.7"/>
            
            <!-- Tags -->
            <FlexLayout Grid.Column="1" Grid.Row="2"
                        BindableLayout.ItemsSource="{Binding QuickTags}"
                        Wrap="Wrap" 
                        AlignItems="Start">
                <BindableLayout.ItemTemplate>
                    <DataTemplate>
                        <Border Padding="6,3" Margin="0,4,4,0"
                                BackgroundColor="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}"
                                StrokeThickness="0">
                            <Border.StrokeShape>
                                <RoundRectangle CornerRadius="12"/>
                            </Border.StrokeShape>
                            <Label Text="{Binding .}" 
                                   FontSize="11"
                                   TextColor="White"/>
                        </Border>
                    </DataTemplate>
                </BindableLayout.ItemTemplate>
            </FlexLayout>
            
            <!-- Quantity & Type -->
            <VerticalStackLayout Grid.Column="2" Grid.RowSpan="3"
                                VerticalOptions="Center"
                                Spacing="4">
                <Label Text="{Binding Quantity}" 
                       FontSize="14"
                       HorizontalOptions="End"/>
                <Border Padding="8,4"
                        BackgroundColor="{Binding TypeId, Converter={StaticResource GlazeTypeIdToColorConverter}}"
                        StrokeThickness="0">
                    <Border.StrokeShape>
                        <RoundRectangle CornerRadius="8"/>
                    </Border.StrokeShape>
                    <Label Text="{Binding TypeName}" 
                           FontSize="10"
                           TextColor="White"
                           FontAttributes="Bold"/>
                </Border>
            </VerticalStackLayout>
        </Grid>
        
        <Border.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding ViewDetailsCommand}"
                                 CommandParameter="{Binding .}"/>
        </Border.GestureRecognizers>
    </Border>
</SwipeView>
```

### 2. Glaze Detail Page

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:controls="clr-namespace:PottMaster.Controls"
             x:Class="PottMaster.Pages.GlazeDetailPage"
             Title="Glaze Details">
    <ScrollView>
        <VerticalStackLayout Spacing="16" Padding="16">
            <!-- Header -->
            <Grid ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
                <Label Grid.Column="0" Grid.Row="0"
                       Text="{Binding Glaze.Name}"
                       Style="{StaticResource HeadlineLabelStyle}"/>
                <Label Grid.Column="0" Grid.Row="1"
                       Text="{Binding Glaze.Manufacturer}"
                       Style="{StaticResource SubheadlineLabelStyle}"/>
                <Button Grid.Column="1" Grid.RowSpan="2"
                        Text="Edit"
                        Command="{Binding EditCommand}"
                        VerticalOptions="Center"/>
            </Grid>

            <!-- Quick Actions -->
            <HorizontalStackLayout Spacing="8">
                <Button Text="{Binding Glaze.IsFavorite, Converter={StaticResource BoolToFavoriteTextConverter}}"
                        Command="{Binding ToggleFavoriteCommand}"/>
                <Button Text="Use on Work"
                        Command="{Binding UseOnWorkCommand}"/>
                <Button Text="Share"
                        Command="{Binding ShareCommand}"/>
            </HorizontalStackLayout>

            <!-- Essential Info Card -->
            <Border Style="{StaticResource CardBorderStyle}">
                <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto,Auto,Auto" 
                      RowSpacing="12" Padding="16">
                    <Label Grid.Row="0" Grid.Column="0" Text="Type:" FontAttributes="Bold"/>
                    <Label Grid.Row="0" Grid.Column="1" Text="{Binding Glaze.TypeName}"/>
                    
                    <Label Grid.Row="1" Grid.Column="0" Text="Cone Rating:" FontAttributes="Bold"/>
                    <Label Grid.Row="1" Grid.Column="1" Text="{Binding Glaze.ConeRating}"/>
                    
                    <Label Grid.Row="2" Grid.Column="0" Text="Color:" FontAttributes="Bold"/>
                    <Label Grid.Row="2" Grid.Column="1" Text="{Binding Glaze.Color}"/>
                    
                    <Label Grid.Row="3" Grid.Column="0" Text="Quantity:" FontAttributes="Bold"/>
                    <Label Grid.Row="3" Grid.Column="1" Text="{Binding Glaze.Quantity}"/>
                    
                    <Label Grid.Row="4" Grid.Column="0" Text="Food Safe:" FontAttributes="Bold"/>
                    <Label Grid.Row="4" Grid.Column="1" 
                           Text="{Binding Glaze.FoodSafe, Converter={StaticResource FoodSafeToTextConverter}}"
                           TextColor="{Binding Glaze.FoodSafe, Converter={StaticResource FoodSafeToColorConverter}}"/>
                </Grid>
            </Border>

            <!-- Collapsible Sections -->
            <controls:CollapsibleSection Title="Firing Parameters" IsExpanded="True">
                <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto" RowSpacing="8">
                    <Label Grid.Row="0" Grid.Column="0" Text="Temperature Range:"/>
                    <Label Grid.Row="0" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Firing.TemperatureRange}"/>
                    
                    <Label Grid.Row="1" Grid.Column="0" Text="Atmosphere:"/>
                    <Label Grid.Row="1" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Firing.Atmosphere}"/>
                    
                    <Label Grid.Row="2" Grid.Column="0" Text="Curve Sensitivity:"/>
                    <Label Grid.Row="2" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Firing.CurveSensitivity}"/>
                </Grid>
            </controls:CollapsibleSection>

            <controls:CollapsibleSection Title="Appearance">
                <VerticalStackLayout Spacing="12">
                    <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto" RowSpacing="8">
                        <Label Grid.Row="0" Grid.Column="0" Text="Transparency:"/>
                        <Label Grid.Row="0" Grid.Column="1" 
                               Text="{Binding Glaze.Properties.Appearance.Transparency}"/>
                        
                        <Label Grid.Row="1" Grid.Column="0" Text="Finish:"/>
                        <Label Grid.Row="1" Grid.Column="1" 
                               Text="{Binding Glaze.Properties.Appearance.Finish}"/>
                    </Grid>
                    
                    <Label Text="Texture:" FontAttributes="Bold"/>
                    <FlexLayout BindableLayout.ItemsSource="{Binding Glaze.Properties.Appearance.Texture}"
                                Wrap="Wrap">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate>
                                <Border Style="{StaticResource TagStyle}">
                                    <Label Text="{Binding .}"/>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </FlexLayout>
                    
                    <Label Text="Special Effects:" FontAttributes="Bold"/>
                    <FlexLayout BindableLayout.ItemsSource="{Binding Glaze.Properties.Appearance.SpecialEffects}"
                                Wrap="Wrap">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate>
                                <Border Style="{StaticResource TagStyle}">
                                    <Label Text="{Binding .}"/>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </FlexLayout>
                </VerticalStackLayout>
            </controls:CollapsibleSection>

            <controls:CollapsibleSection Title="Behavior">
                <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto,Auto" RowSpacing="8">
                    <Label Grid.Row="0" Grid.Column="0" Text="Melt Fluidity:"/>
                    <Label Grid.Row="0" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Behavior.MeltFluidity}"/>
                    
                    <Label Grid.Row="1" Grid.Column="0" Text="Thickness Tolerance:"/>
                    <Label Grid.Row="1" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Behavior.ThicknessTolerance}"/>
                    
                    <Label Grid.Row="2" Grid.Column="0" Text="Color Stability:"/>
                    <Label Grid.Row="2" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Behavior.ColorStability}"/>
                    
                    <Label Grid.Row="3" Grid.Column="0" Text="Repeatability:"/>
                    <Label Grid.Row="3" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Behavior.Repeatability}"/>
                </Grid>
            </controls:CollapsibleSection>

            <controls:CollapsibleSection Title="Application">
                <VerticalStackLayout Spacing="12">
                    <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto" RowSpacing="8">
                        <Label Grid.Row="0" Grid.Column="0" Text="Form:"/>
                        <Label Grid.Row="0" Grid.Column="1" 
                               Text="{Binding Glaze.Properties.Application.Form}"/>
                        
                        <Label Grid.Row="1" Grid.Column="0" Text="Recommended Thickness:"/>
                        <Label Grid.Row="1" Grid.Column="1" 
                               Text="{Binding Glaze.Properties.Application.RecommendedThickness}"/>
                    </Grid>
                    
                    <Label Text="Application Methods:" FontAttributes="Bold"/>
                    <FlexLayout BindableLayout.ItemsSource="{Binding Glaze.Properties.Application.Methods}"
                                Wrap="Wrap">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate>
                                <Border Style="{StaticResource TagStyle}">
                                    <Label Text="{Binding .}"/>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </FlexLayout>
                    
                    <Label Text="Notes:" FontAttributes="Bold"
                           IsVisible="{Binding Glaze.Properties.Application.ApplicationNotes, Converter={StaticResource IsNotNullConverter}}"/>
                    <Label Text="{Binding Glaze.Properties.Application.ApplicationNotes}"/>
                </VerticalStackLayout>
            </controls:CollapsibleSection>

            <controls:CollapsibleSection Title="Clay Compatibility">
                <VerticalStackLayout Spacing="12">
                    <Grid ColumnDefinitions="*,*" RowSpacing="8">
                        <Label Grid.Row="0" Grid.Column="0" Text="Interaction:"/>
                        <Label Grid.Row="0" Grid.Column="1" 
                               Text="{Binding Glaze.Properties.ClayCompatibility.Interaction}"/>
                    </Grid>
                    
                    <Label Text="Best Suited For:" FontAttributes="Bold"/>
                    <FlexLayout BindableLayout.ItemsSource="{Binding Glaze.Properties.ClayCompatibility.BestSuited}"
                                Wrap="Wrap">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate>
                                <Border Style="{StaticResource TagStyle}">
                                    <Label Text="{Binding .}"/>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </FlexLayout>
                </VerticalStackLayout>
            </controls:CollapsibleSection>

            <controls:CollapsibleSection Title="Known Issues & Defects">
                <VerticalStackLayout Spacing="12">
                    <FlexLayout BindableLayout.ItemsSource="{Binding Glaze.Properties.Defects.KnownIssues}"
                                Wrap="Wrap">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate>
                                <Border Style="{StaticResource WarningTagStyle}">
                                    <Label Text="{Binding .}"/>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </FlexLayout>
                    
                    <Label Text="Mitigation:" FontAttributes="Bold"
                           IsVisible="{Binding Glaze.Properties.Defects.MitigationNotes, Converter={StaticResource IsNotNullConverter}}"/>
                    <Label Text="{Binding Glaze.Properties.Defects.MitigationNotes}"/>
                </VerticalStackLayout>
            </controls:CollapsibleSection>

            <controls:CollapsibleSection Title="Usage">
                <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto" RowSpacing="8">
                    <Label Grid.Row="0" Grid.Column="0" Text="Work Type:"/>
                    <Label Grid.Row="0" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Usage.WorkType}"/>
                    
                    <Label Grid.Row="1" Grid.Column="0" Text="Durability:"/>
                    <Label Grid.Row="1" Grid.Column="1" 
                           Text="{Binding Glaze.Properties.Usage.Durability}"/>
                </Grid>
            </controls:CollapsibleSection>

            <!-- Notes Section -->
            <Border Style="{StaticResource CardBorderStyle}"
                    IsVisible="{Binding Glaze.Notes, Converter={StaticResource IsNotNullConverter}}">
                <VerticalStackLayout Spacing="8" Padding="16">
                    <Label Text="Notes" FontAttributes="Bold" FontSize="16"/>
                    <Label Text="{Binding Glaze.Notes}" LineBreakMode="WordWrap"/>
                </VerticalStackLayout>
            </Border>

            <!-- Delete Button -->
            <Button Text="Delete Glaze"
                    Command="{Binding DeleteCommand}"
                    BackgroundColor="Red"
                    Margin="0,24,0,0"/>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

### 3. New/Edit Glaze Form (Tabbed Interface)

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="PottMaster.Pages.NewGlazePage"
             Title="New Glaze">
    <Grid RowDefinitions="Auto,*,Auto">
        <!-- Tab Bar -->
        <ScrollView Grid.Row="0" Orientation="Horizontal" 
                    HorizontalScrollBarVisibility="Never">
            <FlexLayout>
                <Button Text="Basic" 
                        Command="{Binding SelectTabCommand}" 
                        CommandParameter="basic"
                        Style="{Binding CurrentTab, Converter={StaticResource TabButtonStyleConverter}, ConverterParameter='basic'}"/>
                <Button Text="Firing" 
                        Command="{Binding SelectTabCommand}" 
                        CommandParameter="firing"
                        Style="{Binding CurrentTab, Converter={StaticResource TabButtonStyleConverter}, ConverterParameter='firing'}"/>
                <Button Text="Appearance" 
                        Command="{Binding SelectTabCommand}" 
                        CommandParameter="appearance"
                        Style="{Binding CurrentTab, Converter={StaticResource TabButtonStyleConverter}, ConverterParameter='appearance'}"/>
                <Button Text="Behavior" 
                        Command="{Binding SelectTabCommand}" 
                        CommandParameter="behavior"
                        Style="{Binding CurrentTab, Converter={StaticResource TabButtonStyleConverter}, ConverterParameter='behavior'}"/>
                <Button Text="Application" 
                        Command="{Binding SelectTabCommand}" 
                        CommandParameter="application"
                        Style="{Binding CurrentTab, Converter={StaticResource TabButtonStyleConverter}, ConverterParameter='application'}"/>
                <Button Text="Advanced" 
                        Command="{Binding SelectTabCommand}" 
                        CommandParameter="advanced"
                        Style="{Binding CurrentTab, Converter={StaticResource TabButtonStyleConverter}, ConverterParameter='advanced'}"/>
            </FlexLayout>
        </ScrollView>

        <!-- Tab Content -->
        <ScrollView Grid.Row="1">
            <!-- Basic Tab -->
            <VerticalStackLayout IsVisible="{Binding CurrentTab, Converter={StaticResource StringEqualConverter}, ConverterParameter='basic'}"
                                Spacing="16" Padding="16">
                <Entry Placeholder="Glaze Name*" 
                       Text="{Binding Name}"/>
                <Entry Placeholder="Manufacturer" 
                       Text="{Binding Manufacturer}"/>
                <DatePicker Date="{Binding BatchDate}" 
                           Format="D"/>
                <Picker Title="Glaze Type"
                        ItemsSource="{Binding GlazeTypes}"
                        ItemDisplayBinding="{Binding Name}"
                        SelectedItem="{Binding SelectedType}"/>
                <Entry Placeholder="Color" 
                       Text="{Binding Color}"/>
                <Entry Placeholder="Cone Rating (e.g., Cone 6)" 
                       Text="{Binding ConeRating}"/>
                <Entry Placeholder="Quantity (e.g., 500g, half jar)" 
                       Text="{Binding Quantity}"/>
                
                <Label Text="Food Safety" FontAttributes="Bold"/>
                <Grid ColumnDefinitions="*,*,*" ColumnSpacing="8">
                    <Button Grid.Column="0" Text="Not Tested"
                            Command="{Binding SetFoodSafeCommand}"
                            CommandParameter="{x:Null}"/>
                    <Button Grid.Column="1" Text="Safe"
                            Command="{Binding SetFoodSafeCommand}"
                            CommandParameter="True"/>
                    <Button Grid.Column="2" Text="Not Safe"
                            Command="{Binding SetFoodSafeCommand}"
                            CommandParameter="False"/>
                </Grid>
                
                <CheckBox IsChecked="{Binding IsFavorite}">
                    <CheckBox.Behaviors>
                        <toolkit:IconBehavior IconName="star" />
                    </CheckBox.Behaviors>
                </CheckBox>
                <Label Text="Mark as Favorite"/>
            </VerticalStackLayout>

            <!-- Firing Tab -->
            <VerticalStackLayout IsVisible="{Binding CurrentTab, Converter={StaticResource StringEqualConverter}, ConverterParameter='firing'}"
                                Spacing="16" Padding="16">
                <Label Text="Temperature Range" FontAttributes="Bold"/>
                <Grid ColumnDefinitions="*,Auto,*" ColumnSpacing="8">
                    <Entry Grid.Column="0" 
                           Placeholder="Min" 
                           Keyboard="Numeric"
                           Text="{Binding TemperatureMin}"/>
                    <Label Grid.Column="1" Text="-" VerticalOptions="Center"/>
                    <Entry Grid.Column="2" 
                           Placeholder="Max" 
                           Keyboard="Numeric"
                           Text="{Binding TemperatureMax}"/>
                </Grid>
                
                <Picker Title="Unit"
                        SelectedItem="{Binding TemperatureUnit}">
                    <Picker.ItemsSource>
                        <x:Array Type="{x:Type x:String}">
                            <x:String>C</x:String>
                            <x:String>F</x:String>
                        </x:Array>
                    </Picker.ItemsSource>
                </Picker>
                
                <Picker Title="Atmosphere"
                        SelectedItem="{Binding Atmosphere}">
                    <Picker.ItemsSource>
                        <x:Array Type="{x:Type x:String}">
                            <x:String>Oxidation</x:String>
                            <x:String>Reduction</x:String>
                        </x:Array>
                    </Picker.ItemsSource>
                </Picker>
                
                <Picker Title="Curve Sensitivity"
                        SelectedItem="{Binding CurveSensitivity}">
                    <Picker.ItemsSource>
                        <x:Array Type="{x:Type x:String}">
                            <x:String>Low</x:String>
                            <x:String>Medium</x:String>
                            <x:String>High</x:String>
                        </x:Array>
                    </Picker.ItemsSource>
                </Picker>
            </VerticalStackLayout>

            <!-- Appearance Tab -->
            <VerticalStackLayout IsVisible="{Binding CurrentTab, Converter={StaticResource StringEqualConverter}, ConverterParameter='appearance'}"
                                Spacing="16" Padding="16">
                <Picker Title="Transparency"
                        SelectedItem="{Binding Transparency}">
                    <Picker.ItemsSource>
                        <x:Array Type="{x:Type x:String}">
                            <x:String>Transparent</x:String>
                            <x:String>Semi-transparent</x:String>
                            <x:String>Opaque</x:String>
                        </x:Array>
                    </Picker.ItemsSource>
                </Picker>
                
                <Picker Title="Finish"
                        SelectedItem="{Binding Finish}">
                    <Picker.ItemsSource>
                        <x:Array Type="{x:Type x:String}">
                            <x:String>Gloss</x:String>
                            <x:String>Satin</x:String>
                            <x:String>Semi-matte</x:String>
                            <x:String>Matte</x:String>
                        </x:Array>
                    </Picker.ItemsSource>
                </Picker>
                
                <Label Text="Texture" FontAttributes="Bold"/>
                <!-- Multi-select for texture options -->
                <FlexLayout Wrap="Wrap">
                    <CheckBox />
                    <Label Text="Smooth"/>
                    <CheckBox />
                    <Label Text="Cratered"/>
                    <CheckBox />
                    <Label Text="Crystalline"/>
                    <CheckBox />
                    <Label Text="Crackle"/>
                </FlexLayout>
                
                <Label Text="Special Effects" FontAttributes="Bold"/>
                <!-- Multi-select for effects -->
            </VerticalStackLayout>

            <!-- Add similar layouts for Behavior, Application, and Advanced tabs -->
        </ScrollView>

        <!-- Action Buttons -->
        <Grid Grid.Row="2" ColumnDefinitions="*,*" Padding="16" ColumnSpacing="8">
            <Button Grid.Column="0" 
                    Text="Cancel" 
                    Command="{Binding CancelCommand}"/>
            <Button Grid.Column="1" 
                    Text="Save" 
                    Command="{Binding SaveCommand}"
                    IsEnabled="{Binding IsSaving, Converter={StaticResource InverseBoolConverter}}"/>
        </Grid>
    </Grid>
</ContentPage>
```

## UI/UX Best Practices

### 1. **Visual Hierarchy**
- Name and manufacturer are most prominent
- Type and cone rating in colored badges
- Food safety with clear iconography
- Favorite star in prominent position

### 2. **Touch Interactions**
- Swipe-to-delete on list items
- Tap star to favorite
- Tap card to view details
- Long-press for quick actions menu

### 3. **Progressive Enhancement**
- Basic fields required, advanced optional
- Save with partial data allowed
- Clear indication of missing critical info

### 4. **Feedback & Validation**
- Real-time validation on required fields
- Toast messages for save confirmation
- Loading indicators during operations
- Sync status visible in list

### 5. **Search & Filter**
```xaml
<!-- Enhanced Search Bar -->
<Grid ColumnDefinitions="*,Auto" Padding="16">
    <SearchBar Grid.Column="0" 
               Placeholder="Search glazes..."
               Text="{Binding SearchText}"/>
    <Button Grid.Column="1" 
            Text="Filter"
            Command="{Binding ShowFilterCommand}"/>
</Grid>

<!-- Filter Options (Bottom Sheet) -->
<VerticalStackLayout>
    <CheckBox IsChecked="{Binding ShowFavoritesOnly}"/>
    <Label Text="Favorites Only"/>
    
    <Label Text="Glaze Type"/>
    <!-- Type checkboxes -->
    
    <Label Text="Food Safe"/>
    <!-- Food safe radio buttons -->
    
    <Label Text="Cone Rating"/>
    <!-- Cone rating multi-select -->
</VerticalStackLayout>
```

## Data Binding Strategy

### ViewModel Pattern
```csharp
// Flatten JSONB properties for easier binding
public partial class GlazeDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private LocalGlaze glaze;
    
    // Computed properties for display
    public string TemperatureRangeDisplay => 
        glaze.Properties.Firing?.TemperatureRange ?? "Not specified";
    
    public string AppearanceSummary =>
        $"{glaze.Properties.Appearance?.Transparency} • {glaze.Properties.Appearance?.Finish}";
}
```

## Performance Considerations

### 1. **List Virtualization**
- Use CollectionView with virtualization enabled
- Load glazes in pages (20-50 at a time)
- Cache type/category lookups

### 2. **JSONB Parsing**
- Parse JSONB only when detail view is opened
- Cache parsed properties in ViewModel
- Use lazy loading for complex sections

### 3. **Image Optimization**
- Future: Support glaze test tile photos
- Lazy load images in list
- Thumbnail generation for cards

## Accessibility

### 1. **Screen Reader Support**
```xaml
<Label Text="{Binding Name}"
       SemanticProperties.HeadingLevel="Level1"/>
<Image Source="star.png"
       SemanticProperties.Description="Favorite glaze"/>
```

### 2. **High Contrast**
- Support system theme (Light/Dark)
- Sufficient color contrast ratios
- Don't rely solely on color for info

### 3. **Font Scaling**
- Respect system font size settings
- Test with large text enabled

## Next Steps

1. Implement IGlazeService and repository
2. Create database migration for new glaze structure
3. Build out NewGlazePage with all tabs
4. Add advanced search/filter functionality
5. Implement glaze comparison feature
6. Add glaze usage history (which works used this glaze)
7. Export/Import glaze recipes

## Resources

- [.NET MAUI CollectionView](https://docs.microsoft.com/dotnet/maui/user-interface/controls/collectionview/)
- [.NET MAUI Data Binding](https://docs.microsoft.com/dotnet/maui/fundamentals/data-binding/)
- [Material Design for Mobile](https://material.io/design/platform-guidance/android-bars.html)
