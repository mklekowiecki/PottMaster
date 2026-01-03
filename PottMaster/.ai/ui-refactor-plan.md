# UI Refactoring Plan – PottMaster (.NET MAUI)

## Executive Summary

This document outlines a comprehensive plan to refactor the PottMaster UI to create a consistent, modern, and Material Design 3-inspired design system similar to Google's Reply app. The refactor focuses on establishing reusable styles, standardizing spacing and typography, and ensuring all pages follow the same visual language.

---

## 1. Current State Analysis

### 1.1 Existing Pages
- ? **LoginPage**: Basic implementation with HeadingLabel and SubheadingLabel styles
- ? **SignupPage**: Similar to LoginPage, basic styling
- ? **MainPage**: Work list with cards, uses Border components
- ? **NewWorkPage**: Complex form with photos, picker, slider, and sections
- ? **WorkDetailPage**: Detailed view with grid layout and info cards
- ? **Profile/Settings**: User profile with logout functionality
- ? **GlazeInventoryPage**: Not yet implemented
- ? **WikiPage**: Not yet implemented
- ? **AnalyticsPage**: Not yet implemented

### 1.2 Current Style Resources
- **Colors.xaml**: Basic color palette with Primary, Secondary, Gray scales
- **Styles.xaml**: Default MAUI styles with some customization
  - ModernEntry style for entries
  - HeadingLabel and SubheadingLabel for text
  - Button styles with rounded corners
  - Limited custom styles for complex scenarios

### 1.3 Issues Identified
1. **Inconsistent spacing**: Different pages use different padding/margin values
2. **Typography inconsistency**: Font sizes vary across similar elements
3. **Color usage**: Not following a consistent theme hierarchy
4. **Border/Card styling**: Different corner radius and padding values
5. **Button variations**: Inconsistent height, corner radius, and font sizes
6. **Missing component styles**: No reusable card, section header, or info row styles
7. **Dark mode support**: Incomplete theme support across pages
8. **Layout patterns**: No standardized section/group layout pattern

---

## 2. Design System Foundation

### 2.1 Material Design 3 Principles (Reply App Style)
- **Clean & Modern**: Subtle shadows, generous whitespace
- **Typography-focused**: Clear hierarchy with varied font sizes
- **Intuitive Navigation**: Bottom nav bar with clear icons
- **Rounded corners**: Consistent use of rounded rectangles
- **Color harmony**: Limited palette with strong primary color
- **Elevation**: Subtle shadows for cards and floating elements
- **Responsive**: Adapts to different screen sizes

### 2.2 Design Tokens to Define

#### Spacing Scale
```
XXSmall: 4
XSmall: 8
Small: 12
Medium: 16
Large: 20
XLarge: 24
XXLarge: 32
Huge: 48
```

#### Corner Radius Scale
```
None: 0
XSmall: 4
Small: 8
Medium: 12
Large: 16
XLarge: 20
Circle: 9999 (for fully rounded)
```

#### Typography Scale
```
Caption: 12
Body: 14
BodyLarge: 16
Subtitle: 18
Title: 20
Headline: 24
DisplaySmall: 28
DisplayMedium: 32
DisplayLarge: 36
```

#### Shadow/Elevation
```
Level0: No shadow
Level1: Subtle shadow for cards
Level2: Medium shadow for floating elements
Level3: Strong shadow for modals/dialogs
```

### 2.3 Updated Color Palette (Material Design 3)

**Surface Colors**
- Surface: White (Light) / #1C1B1F (Dark)
- SurfaceVariant: #F3F0F4 (Light) / #49454F (Dark)
- SurfaceContainer: #F7F2FA (Light) / #1E1A20 (Dark)

**Primary Colors**
- Primary: #6750A4 (Purple - Material You inspired)
- OnPrimary: White
- PrimaryContainer: #EADDFF
- OnPrimaryContainer: #21005D

**Secondary Colors**
- Secondary: #625B71
- OnSecondary: White
- SecondaryContainer: #E8DEF8
- OnSecondaryContainer: #1D192B

**Tertiary Colors**
- Tertiary: #7D5260
- OnTertiary: White
- TertiaryContainer: #FFD8E4
- OnTertiaryContainer: #31111D

**Error Colors**
- Error: #B3261E
- OnError: White
- ErrorContainer: #F9DEDC
- OnErrorContainer: #410E0B

**Status Colors**
- Success: #4CAF50
- Warning: #FF9800
- Info: #2196F3

---

## 3. Common Component Styles

### 3.1 Card Styles
```xml
<!-- Standard Card -->
<Style x:Key="CardStyle" TargetType="Border">
    <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Surface}, Dark={StaticResource SurfaceDark}}" />
    <Setter Property="StrokeThickness" Value="0"/>
    <Setter Property="Padding" Value="16"/>
    <Setter Property="Margin" Value="0,0,0,12"/>
    <Setter Property="StrokeShape">
        <RoundRectangle CornerRadius="12"/>
    </Setter>
    <Setter Property="Shadow">
        <Shadow Brush="{AppThemeBinding Light=#40000000, Dark=#40000000}"
                Offset="0,2"
                Radius="8"
                Opacity="0.15"/>
    </Setter>
</Style>

<!-- Elevated Card -->
<Style x:Key="ElevatedCardStyle" TargetType="Border" BasedOn="{StaticResource CardStyle}">
    <Setter Property="Shadow">
        <Shadow Brush="{AppThemeBinding Light=#40000000, Dark=#40000000}"
                Offset="0,4"
                Radius="12"
                Opacity="0.25"/>
    </Setter>
</Style>

<!-- Info Card (colored background) -->
<Style x:Key="InfoCardStyle" TargetType="Border">
    <Setter Property="BackgroundColor" Value="{StaticResource PrimaryContainer}" />
    <Setter Property="StrokeThickness" Value="0"/>
    <Setter Property="Padding" Value="16"/>
    <Setter Property="Margin" Value="0,0,0,12"/>
    <Setter Property="StrokeShape">
        <RoundRectangle CornerRadius="12"/>
    </Setter>
</Style>
```

### 3.2 Typography Styles
```xml
<!-- Display Styles -->
<Style x:Key="DisplayLarge" TargetType="Label">
    <Setter Property="FontSize" Value="36" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<Style x:Key="DisplayMedium" TargetType="Label">
    <Setter Property="FontSize" Value="32" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<!-- Headline Styles -->
<Style x:Key="HeadlineLarge" TargetType="Label">
    <Setter Property="FontSize" Value="24" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<Style x:Key="HeadlineMedium" TargetType="Label">
    <Setter Property="FontSize" Value="20" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<!-- Title Styles -->
<Style x:Key="TitleLarge" TargetType="Label">
    <Setter Property="FontSize" Value="18" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<Style x:Key="TitleMedium" TargetType="Label">
    <Setter Property="FontSize" Value="16" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<!-- Body Styles -->
<Style x:Key="BodyLarge" TargetType="Label">
    <Setter Property="FontSize" Value="16" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<Style x:Key="BodyMedium" TargetType="Label">
    <Setter Property="FontSize" Value="14" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<!-- Label Styles -->
<Style x:Key="LabelLarge" TargetType="Label">
    <Setter Property="FontSize" Value="14" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
</Style>

<Style x:Key="LabelMedium" TargetType="Label">
    <Setter Property="FontSize" Value="12" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurfaceVariant}, Dark={StaticResource OnSurfaceVariantDark}}" />
</Style>

<!-- Caption -->
<Style x:Key="Caption" TargetType="Label">
    <Setter Property="FontSize" Value="12" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurfaceVariant}, Dark={StaticResource OnSurfaceVariantDark}}" />
    <Setter Property="Opacity" Value="0.7" />
</Style>
```

### 3.3 Button Styles
```xml
<!-- Primary Button -->
<Style x:Key="PrimaryButton" TargetType="Button">
    <Setter Property="BackgroundColor" Value="{StaticResource Primary}" />
    <Setter Property="TextColor" Value="{StaticResource OnPrimary}" />
    <Setter Property="FontSize" Value="16" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="CornerRadius" Value="20" />
    <Setter Property="HeightRequest" Value="48" />
    <Setter Property="Padding" Value="24,12" />
</Style>

<!-- Secondary Button (Outlined) -->
<Style x:Key="SecondaryButton" TargetType="Button">
    <Setter Property="BackgroundColor" Value="Transparent" />
    <Setter Property="TextColor" Value="{StaticResource Primary}" />
    <Setter Property="BorderColor" Value="{StaticResource Primary}" />
    <Setter Property="BorderWidth" Value="1" />
    <Setter Property="FontSize" Value="16" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="CornerRadius" Value="20" />
    <Setter Property="HeightRequest" Value="48" />
    <Setter Property="Padding" Value="24,12" />
</Style>

<!-- Text Button -->
<Style x:Key="TextButton" TargetType="Button">
    <Setter Property="BackgroundColor" Value="Transparent" />
    <Setter Property="TextColor" Value="{StaticResource Primary}" />
    <Setter Property="FontSize" Value="14" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="BorderWidth" Value="0" />
    <Setter Property="Padding" Value="16,8" />
</Style>

<!-- FAB (Floating Action Button) -->
<Style x:Key="FABButton" TargetType="Button">
    <Setter Property="BackgroundColor" Value="{StaticResource Primary}" />
    <Setter Property="TextColor" Value="{StaticResource OnPrimary}" />
    <Setter Property="FontSize" Value="18" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="CornerRadius" Value="28" />
    <Setter Property="HeightRequest" Value="56" />
    <Setter Property="WidthRequest" Value="56" />
    <Setter Property="Shadow">
        <Shadow Brush="#40000000" Offset="0,4" Radius="12" Opacity="0.3"/>
    </Setter>
</Style>
```

### 3.4 Input Field Styles
```xml
<!-- Primary Entry -->
<Style x:Key="PrimaryEntry" TargetType="Entry">
    <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource SurfaceVariant}, Dark={StaticResource SurfaceVariantDark}}" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
    <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light={StaticResource OnSurfaceVariant}, Dark={StaticResource OnSurfaceVariantDark}}" />
    <Setter Property="FontSize" Value="16" />
    <Setter Property="HeightRequest" Value="56" />
    <Setter Property="Padding" Value="16,0" />
</Style>

<!-- Picker Style -->
<Style x:Key="PrimaryPicker" TargetType="Picker">
    <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource SurfaceVariant}, Dark={StaticResource SurfaceVariantDark}}" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
    <Setter Property="FontSize" Value="16" />
    <Setter Property="HeightRequest" Value="56" />
</Style>
```

### 3.5 Section/Group Styles
```xml
<!-- Section Container -->
<Style x:Key="SectionContainer" TargetType="VerticalStackLayout">
    <Setter Property="Spacing" Value="12" />
    <Setter Property="Margin" Value="0,0,0,24" />
</Style>

<!-- Section Header -->
<Style x:Key="SectionHeader" TargetType="Label">
    <Setter Property="FontSize" Value="18" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource OnSurface}, Dark={StaticResource OnSurfaceDark}}" />
    <Setter Property="Margin" Value="0,0,0,8" />
</Style>
```

---

## 4. Page-by-Page Refactoring Plan

### 4.1 LoginPage Refactor

**Priority**: High (First Impression)

**Changes**:
1. Update heading to use `DisplayMedium` style
2. Update subheading to use `TitleLarge` style
3. Change entries to use `PrimaryEntry` style
4. Update primary button to use `PrimaryButton` style
5. Update secondary button to use `SecondaryButton` style
6. Standardize spacing to 24dp between sections
7. Add subtle logo/branding area at top
8. Improve error message display with colored container

**Layout Pattern**:
```
- Logo/Brand Area (optional)
- Display Heading
- Subtitle
- [24dp spacing]
- Email Entry
- [12dp spacing]
- Password Entry
- [8dp spacing]
- Error Message (if visible)
- [24dp spacing]
- Login Button
- [16dp spacing]
- Activity Indicator
- [32dp spacing]
- "No Account" text
- [12dp spacing]
- Signup Button
```

### 4.2 SignupPage Refactor

**Priority**: High

**Changes**:
1. Match LoginPage styling exactly
2. Use consistent typography hierarchy
3. Same button and entry styles
4. Add password strength indicator (future enhancement)
5. Add terms/privacy checkbox (future enhancement)

**Layout Pattern**: Similar to LoginPage

### 4.3 MainPage Refactor

**Priority**: High (Core Functionality)

**Changes**:
1. Update work cards to use `CardStyle`
2. Improve empty state with illustration (optional) and better text hierarchy
3. Update FAB to use `FABButton` style
4. Standardize card content layout:
   - Photo thumbnail (80x80) with rounded corners
   - Title with `TitleMedium` style
   - Subtitle with `BodyMedium` style
   - Status badge with colored background
   - Time remaining with `Caption` style
5. Add subtle dividers between cards or use consistent card margins
6. Improve pull-to-refresh visual feedback

**Work Card Layout**:
```
Border (CardStyle)
??? Grid (3 columns: 80, *, Auto)
    ??? Photo Thumbnail (rounded)
    ??? Info Column
    ?   ??? Work Code (TitleMedium)
    ?   ??? Category (BodyMedium)
    ?   ??? Status Badge (colored)
    ?   ??? Time Remaining (Caption)
    ??? Chevron Icon
```

### 4.4 NewWorkPage Refactor

**Priority**: High (Core Functionality)

**Changes**:
1. Group form into clear sections using `SectionContainer`
2. Use `SectionHeader` for each section title
3. Update photo area to use `CardStyle` with better placeholder
4. Use `PrimaryButton` and `SecondaryButton` for photo actions
5. Update picker to use `PrimaryPicker` style
6. Update entry to use `PrimaryEntry` style
7. Improve slider visual with better labels and styling
8. Update drying time estimate card to use `InfoCardStyle`
9. Update save button to use `PrimaryButton` with extended style
10. Add visual feedback during save operation

**Sections**:
1. Photo Section
   - Section Header
   - Photo Preview/Placeholder Card
   - Action Buttons (Take/Pick)
2. Details Section
   - Category Picker
   - Work Code Entry
3. Wall Thickness Section
   - Header with current value
   - Slider
   - Labels
   - Estimate Card
4. Actions Section
   - Save Button
   - Loading Indicator

### 4.5 WorkDetailPage Refactor

**Priority**: High (Core Functionality)

**Changes**:
1. Update photo gallery to use better styling
2. Replace placeholder border with `CardStyle`
3. Update work code card to use `ElevatedCardStyle` with Primary background
4. Convert info grid items to use consistent `CardStyle`
5. Update drying timer card to use `InfoCardStyle` with appropriate status color
6. Use consistent typography throughout
7. Improve button styling with `PrimaryButton` and `SecondaryButton`
8. Better visual hierarchy with spacing

**Layout Structure**:
```
- Photo Gallery (horizontal scroll)
- Work Code Card (elevated, primary color)
- Info Grid (2 columns)
  - Category Card
  - Status Card
  - Wall Thickness Card
  - Sync Status Card
  - Created Date Card
  - Updated Date Card
- Drying Timer Card (colored based on status)
- Action Buttons
```

### 4.6 Profile/Settings Page (New)

**Priority**: Medium

**Status**: ? **COMPLETED**

**Implementation Details**:
- **ProfileViewModel.cs**: Full MVVM with CommunityToolkit.Mvvm
  - Properties: UserEmail, UserInitials, IsLoading, AppVersion
  - LogoutCommand with confirmation dialog
  - Integrated with IAuthService, IAuthStateService, IAlertService, IErrorHandlingService
- **ProfilePage.xaml**: Material Design 3 styled
  - Circular avatar (100x100px) with user initials on Primary background
  - Three main sections with SectionContainer styling
  - Error color styling for logout button in Danger Zone
  - Full light/dark theme support

**Design**:
1. ? Header with user initials/avatar (circular)
2. ? User email/name
3. ? Settings sections using `SectionContainer`:
   - Account Settings
     - Initials display
     - Language Picker (future)
     - Theme Toggle (future)
   - About
     - Version info
     - Privacy Policy link (future)
     - Terms of Service link (future)
   - Danger Zone
     - Logout Button with confirmation (implemented)
     - Delete Account (future)

### 4.7 GlazeInventoryPage (Future)

**Priority**: Low

**Design Pattern**: Similar to MainPage
- List of glaze cards
- FAB to add new glaze
- Each glaze card shows:
  - Color swatch
  - Glaze name
  - Manufacturer
  - Stock level
  - Quick actions

### 4.8 WikiPage (Future)

**Priority**: Low

**Design**:
- SearchBar at top
- Categorized list or tabs
- Cards for each entry
- Detail page with rich text and images

### 4.9 AnalyticsPage (Future)

**Priority**: Low

**Design**:
- Summary cards at top
- Charts/graphs (MAUI Graphics)
- Export button
- Date range selector

---

## 5. Navigation & AppShell Refactor

### 5.1 Bottom Tab Bar Implementation

**Changes**:
1. Convert AppShell to use TabBar
2. Define 5 tabs:
   - Works (Home)
   - Inventory
   - Wiki
   - Analytics
   - Profile
3. Use icons for each tab
4. Implement conditional visibility (hide tabs when not authenticated)

**AppShell Structure**:
```xml
<Shell>
    <TabBar Route="main">
        <Tab Title="Works" Icon="work_icon.png" Route="works">
            <ShellContent ContentTemplate="{DataTemplate pages:MainPage}" />
        </Tab>
        <Tab Title="Inventory" Icon="inventory_icon.png" Route="inventory">
            <ShellContent ContentTemplate="{DataTemplate pages:GlazeInventoryPage}" />
        </Tab>
        <Tab Title="Wiki" Icon="wiki_icon.png" Route="wiki">
            <ShellContent ContentTemplate="{DataTemplate pages:WikiPage}" />
        </Tab>
        <Tab Title="Analytics" Icon="analytics_icon.png" Route="analytics">
            <ShellContent ContentTemplate="{DataTemplate pages:AnalyticsPage}" />
        </Tab>
        <Tab Title="Profile" Icon="profile_icon.png" Route="profile">
            <ShellContent ContentTemplate="{DataTemplate pages:ProfilePage}" />
        </Tab>
    </TabBar>
</Shell>
```

### 5.2 Sync Status Indicator

**Implementation**:
- Global component in AppShell
- Shows in top-right corner or bottom bar
- Icon changes based on sync state:
  - ? Synced (green)
  - ? Syncing (animated)
  - ! Error (red)
  - ? Offline (gray)

---

## 6. Implementation Sequence

### Phase 1: Foundation (Week 1)
1. ? Update Colors.xaml with Material Design 3 palette
2. ? Create all typography styles in Styles.xaml
3. ? Create all button styles
4. ? Create card and container styles
5. ? Create input field styles

### Phase 2: Core Pages (Week 2)
1. ? Refactor LoginPage
2. ? Refactor SignupPage
3. ? Refactor MainPage
4. ? Refactor NewWorkPage
5. ? Refactor WorkDetailPage

### Phase 3: Navigation (Week 3)
1. ? Implement Bottom Tab Bar in AppShell
2. ? Create Profile/Settings Page
3. ? Implement Sync Status Indicator
4. ? Test navigation flows

### Phase 4: Additional Pages (Week 4)
1. ? Create GlazeInventoryPage
2. ? Create WikiPage
3. ? Create AnalyticsPage

### Phase 5: Polish & Testing (Week 5)
1. ? Accessibility review
2. ? Dark mode testing
3. ? Responsive layout testing (phone/tablet/desktop)
4. ? Performance optimization
5. ? Localization review

---

## 7. Custom Components to Create

### 7.1 Reusable Components
1. **WorkCard**: Reusable card for work items
2. **InfoRow**: Label + Value pair in a row
3. **StatusBadge**: Colored badge for status display
4. **PhotoGallery**: Horizontal scrolling photo viewer
5. **SectionDivider**: Subtle divider with optional text
6. **EmptyState**: Reusable empty state component
7. **LoadingOverlay**: Full-screen loading indicator

### 7.2 Component Priority
- High: WorkCard, StatusBadge, EmptyState
- Medium: InfoRow, PhotoGallery
- Low: SectionDivider, LoadingOverlay

---

## 8. Assets & Resources Needed

### 8.1 Icons
- Tab bar icons (5 icons for each tab)
- Status icons (synced, syncing, error, offline)
- Action icons (camera, gallery, delete, edit)
- Empty state illustrations (optional)

### 8.2 Fonts
- Currently using OpenSans
- Consider adding Roboto for Material Design consistency (optional)

---

## 9. Testing Strategy

### 9.1 Visual Testing
- Screenshot comparisons before/after
- Test on multiple device sizes
- Test light and dark modes
- Test different text scaling settings

### 9.2 Functional Testing
- Navigation flows work correctly
- All buttons and inputs are accessible
- Forms validate correctly
- Loading states display properly

### 9.3 Performance Testing
- Smooth scrolling on long lists
- Fast page transitions
- Image loading performance
- No UI thread blocking

---

## 10. Documentation Updates

### 10.1 Style Guide Document
- Create comprehensive style guide
- Document all color tokens
- Document all typography styles
- Document all component styles
- Include usage examples

### 10.2 Component Library
- Document all custom components
- Include XAML examples
- Document properties and events
- Include screenshots

---

## 11. Success Criteria

? **Consistency**: All pages use the same design system
? **Accessibility**: WCAG AA compliant
? **Performance**: 60 FPS on target devices
? **Maintainability**: Easy to add new pages following established patterns
? **Localization**: All strings in RESX, no hardcoded text
? **Responsive**: Works on phone, tablet, and desktop
? **Dark Mode**: Full dark mode support

---

## 12. References

- [Material Design 3 Guidelines](https://m3.material.io/)
- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [Google Reply App](https://material.io/design/material-studies/reply.html)
- [MAUI UI Guidelines](./.kilocode/rules/maui-guidelines.md)
- [UI Implementation Plan](./ui-plan.md)

---

**Document Version**: 1.0
**Created**: 2025-01-01
**Last Updated**: 2025-01-01
**Author**: GitHub Copilot
**Status**: Ready for Review & Implementation
