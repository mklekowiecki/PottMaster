# Localization Additions for AppShell, ProfilePage, and ProfileViewModel

## New Entries to Add to AppResources.resx (Polish) and AppResources.en.resx (English)

### AppShell.xaml Strings:
```
<data name="AppTitle" xml:space="preserve">
    <value>PottMaster</value>
</data>
<data name="LoginTitle" xml:space="preserve">
    <value>Logowanie</value>
</data>
<data name="SignupTitle" xml:space="preserve">
    <value>Rejestracja</value>
</data>
<data name="TabWorksTitle" xml:space="preserve">
    <value>Prace</value>
</data>
<data name="TabInventoryTitle" xml:space="preserve">
    <value>Inwentarz</value>
</data>
<data name="TabWikiTitle" xml:space="preserve">
    <value>Wikipedia</value>
</data>
<data name="TabAnalyticsTitle" xml:space="preserve">
    <value>Analizy</value>
</data>
<data name="TabProfileTitle" xml:space="preserve">
    <value>Profil</value>
</data>
```

### ProfilePage.xaml Strings:
```
<data name="ProfileTitle" xml:space="preserve">
    <value>Profil</value>
</data>
<data name="AccountSettings" xml:space="preserve">
    <value>Ustawienia konta</value>
</data>
<data name="Initials" xml:space="preserve">
    <value>Inicja?y</value>
</data>
<data name="About" xml:space="preserve">
    <value>O aplikacji</value>
</data>
<data name="Version" xml:space="preserve">
    <value>Wersja</value>
</data>
<data name="DangerZone" xml:space="preserve">
    <value>Strefa zagro?enia</value>
</data>
<data name="Logout" xml:space="preserve">
    <value>Wyloguj</value>
</data>
```

### ProfileViewModel.cs Strings:
```
<data name="LogoutConfirmationTitle" xml:space="preserve">
    <value>Wylogowanie</value>
</data>
<data name="LogoutConfirmationMessage" xml:space="preserve">
    <value>Czy na pewno chcesz si? wylogowa??</value>
</data>
```

---

## English Translations (AppResources.en.resx):
```
<data name="AppTitle" xml:space="preserve">
    <value>PottMaster</value>
</data>
<data name="LoginTitle" xml:space="preserve">
    <value>Login</value>
</data>
<data name="SignupTitle" xml:space="preserve">
    <value>Sign Up</value>
</data>
<data name="TabWorksTitle" xml:space="preserve">
    <value>Works</value>
</data>
<data name="TabInventoryTitle" xml:space="preserve">
    <value>Inventory</value>
</data>
<data name="TabWikiTitle" xml:space="preserve">
    <value>Wiki</value>
</data>
<data name="TabAnalyticsTitle" xml:space="preserve">
    <value>Analytics</value>
</data>
<data name="TabProfileTitle" xml:space="preserve">
    <value>Profile</value>
</data>
<data name="ProfileTitle" xml:space="preserve">
    <value>Profile</value>
</data>
<data name="AccountSettings" xml:space="preserve">
    <value>Account Settings</value>
</data>
<data name="Initials" xml:space="preserve">
    <value>Initials</value>
</data>
<data name="About" xml:space="preserve">
    <value>About</value>
</data>
<data name="Version" xml:space="preserve">
    <value>Version</value>
</data>
<data name="DangerZone" xml:space="preserve">
    <value>Danger Zone</value>
</data>
<data name="Logout" xml:space="preserve">
    <value>Logout</value>
</data>
<data name="LogoutConfirmationTitle" xml:space="preserve">
    <value>Logout</value>
</data>
<data name="LogoutConfirmationMessage" xml:space="preserve">
    <value>Are you sure you want to logout?</value>
</data>
```

---

## Files to Update:
1. Add the Polish entries to `PottMaster/Resources/AppResources.resx`
2. Add the English entries to `PottMaster/Resources/AppResources.en.resx`

## Usage in Code:
After adding these resources, update the XAML and C# files to use them:

### AppShell.xaml:
```xaml
<Shell Title="{x:Static resx:AppResources.AppTitle}">
    <ShellContent Title="{x:Static resx:AppResources.LoginTitle}" ... />
    <ShellContent Title="{x:Static resx:AppResources.SignupTitle}" ... />
    <Tab Title="{x:Static resx:AppResources.TabWorksTitle}" ... />
    <Tab Title="{x:Static resx:AppResources.TabInventoryTitle}" ... />
    <Tab Title="{x:Static resx:AppResources.TabWikiTitle}" ... />
    <Tab Title="{x:Static resx:AppResources.TabAnalyticsTitle}" ... />
    <Tab Title="{x:Static resx:AppResources.TabProfileTitle}" ... />
</Shell>
```

### ProfilePage.xaml:
```xaml
<ContentPage Title="{x:Static resx:AppResources.ProfileTitle}">
    <Label Text="{x:Static resx:AppResources.AccountSettings}" />
    <Label Text="{x:Static resx:AppResources.Initials}" />
    <Label Text="{x:Static resx:AppResources.About}" />
    <Label Text="{x:Static resx:AppResources.Version}" />
    <Label Text="{x:Static resx:AppResources.DangerZone}" />
    <Button Text="{x:Static resx:AppResources.Logout}" />
</ContentPage>
```

### ProfileViewModel.cs:
```csharp
var confirm = await _alertService.ShowConfirmationAsync(
    AppResources.LogoutConfirmationTitle,
    AppResources.LogoutConfirmationMessage);
```</content>
<parameter name="filePath">PottMaster/.ai/localization-additions.md