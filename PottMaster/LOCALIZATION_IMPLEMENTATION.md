# Localization Implementation for Work Categories and Statuses

## Overview
This implementation adds localization support for work categories and work statuses in the PottMaster application. Users will now see localized names instead of technical codes (e.g., "Kubek" instead of "CUP" in Polish).

## Files Created

### 1. Converters
- **PottMaster/Converters/WorkCategoryCodeToLocalizedConverter.cs**
  - Converts work category codes (CUP, BOWL, etc.) to localized strings
  - Uses AppResources with key pattern: `WorkCategory_{CODE}`

- **PottMaster/Converters/WorkStatusCodeToLocalizedConverter.cs**
  - Converts work status codes (WET, LEATHER_HARD, etc.) to localized strings
  - Uses AppResources with key pattern: `WorkStatus_{CODE}`

### 2. Resource Files to Update
Two helper files have been created with the resource entries that need to be added:

- **PottMaster/Resources/AppResources_entries_to_add.txt** - Polish translations
- **PottMaster/Resources/AppResources.en_entries_to_add.txt** - English translations

## Manual Steps Required

### Step 1: Add Resource Strings to AppResources.resx (Polish - Default)

1. Open `PottMaster/Resources/AppResources.resx` in Visual Studio
2. Copy all entries from `AppResources_entries_to_add.txt`
3. Paste them before the closing `</root>` tag in the resx file
4. Save the file

The resource file should include entries like:
```xml
<data name="WorkCategory_CUP" xml:space="preserve">
  <value>Kubek</value>
  <comment>Work category: Cup</comment>
</data>
```

### Step 2: Add Resource Strings to AppResources.en.resx (English)

1. Open `PottMaster/Resources/AppResources.en.resx` in Visual Studio
2. Copy all entries from `AppResources.en_entries_to_add.txt`
3. Paste them before the closing `</root>` tag in the resx file
4. Save the file

The resource file should include entries like:
```xml
<data name="WorkCategory_CUP" xml:space="preserve">
  <value>Cup</value>
  <comment>Work category: Cup</comment>
</data>
```

### Step 3: Clean Up Helper Files (Optional)

After adding the resources, you can delete these temporary files:
- `PottMaster/Resources/AppResources_entries_to_add.txt`
- `PottMaster/Resources/AppResources.en_entries_to_add.txt`

## Files Modified

### 1. PottMaster/App.xaml
- Registered the two new converters as application resources:
  - `WorkCategoryCodeToLocalizedConverter`
  - `WorkStatusCodeToLocalizedConverter`

### 2. PottMaster/Pages/WorkDetailPage.xaml
- Applied `WorkCategoryCodeToLocalizedConverter` to category display
- Applied `WorkStatusCodeToLocalizedConverter` to status display
- Changes in lines displaying CurrentWork.CategoryCode and CurrentWork.StatusCode

### 3. PottMaster/Pages/MainPage.xaml
- Applied `WorkCategoryCodeToLocalizedConverter` to CategoryName binding in work list items
- Applied `WorkStatusCodeToLocalizedConverter` to StatusName binding in work list items

### 4. PottMaster/Pages/NewWorkPage.xaml
- Applied `WorkCategoryCodeToLocalizedConverter` to Picker's ItemDisplayBinding
- Now shows localized category names in the dropdown selection

## Resource Key Mapping

### Work Categories
| Code | Polish (pl) | English (en) |
|------|-------------|--------------|
| CUP | Kubek | Cup |
| BOWL | Miska | Bowl |
| VASE | Wazon | Vase |
| PLATE | Talerz | Plate |
| SCULPTURE | Rze?ba | Sculpture |
| TILE | P?ytka | Tile |
| OTHER | Inne | Other |

### Work Statuses
| Code | Polish (pl) | English (en) |
|------|-------------|--------------|
| WET | Mokra | Wet |
| LEATHER_HARD | Skórzasta | Leather Hard |
| BONE_DRY | Ko?cista sucha | Bone Dry |
| BISQUE_FIRED | Biszkwit | Bisque Fired |
| GLAZED | Glazurowana | Glazed |
| GLAZE_FIRED | Wypalona z glazur? | Glaze Fired |
| COMPLETED | Uko?czona | Completed |
| DISCARDED | Odrzucona | Discarded |

## Testing

After completing the manual steps, test the following:

1. **MainPage** - Verify work items show localized category and status names
2. **WorkDetailPage** - Verify category and status display with localized names
3. **NewWorkPage** - Verify category picker shows localized names in dropdown
4. **Language Switching** - Change the app language and verify translations update correctly

## Database Schema Reference

The local SQLite database stores:
- `CategoryName` - Contains the code (e.g., "CUP")
- `StatusName` - Contains the code (e.g., "WET")

The converters translate these codes to localized strings at display time, so no database changes are needed.

## Future Enhancements

To add support for additional languages:
1. Create a new resource file (e.g., `AppResources.de.resx` for German)
2. Add all the category and status entries with appropriate translations
3. The converters will automatically use the correct resource file based on the current culture

## Notes

- The converters gracefully fallback to showing the code if a translation is missing
- All XAML bindings use the StaticResource key to reference the converters
- The implementation follows .NET MAUI localization best practices
- Resource files are automatically compiled and embedded in the application
