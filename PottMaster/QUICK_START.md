# Quick Start - Add Localization Resources

## ?? Required Manual Step

The localization converters have been implemented and all code changes are complete. However, you must **manually add the resource strings** to enable localization.

## Step-by-Step Instructions

### Step 1: Add Polish Translations (Default)

1. Open Visual Studio
2. In Solution Explorer, navigate to: `PottMaster` ? `Resources` ? `AppResources.resx`
3. Double-click to open the file
4. Switch to the XML editor view (right-click on the file ? "Open With" ? "XML (Text) Editor" or "Code")
5. Scroll to the bottom of the file, just before `</root>`
6. Copy the entire content from the file: `PottMaster/Resources/AppResources_entries_to_add.txt`
7. Paste it before `</root>`
8. Save the file

### Step 2: Add English Translations

1. In Solution Explorer, navigate to: `PottMaster` ? `Resources` ? `AppResources.en.resx`
2. Double-click to open the file
3. Switch to the XML editor view (if needed)
4. Scroll to the bottom of the file, just before `</root>`
5. Copy the entire content from the file: `PottMaster/Resources/AppResources.en_entries_to_add.txt`
6. Paste it before `</root>`
7. Save the file

### Step 3: Verify the Changes

After adding the resources, rebuild the project:
- Press `Ctrl+Shift+B` or go to `Build` ? `Rebuild Solution`
- Verify there are no build errors

### Step 4: Test the Application

Run the application and verify:
- [ ] Work list shows category names like "Kubek" instead of "CUP"
- [ ] Work list shows status names like "Mokra" instead of "WET"
- [ ] Category dropdown in NewWorkPage shows localized names
- [ ] Work detail page shows localized category and status

### Step 5: Clean Up (Optional)

After successfully adding the resources, you can delete these temporary files:
- `PottMaster/Resources/AppResources_entries_to_add.txt`
- `PottMaster/Resources/AppResources.en_entries_to_add.txt`

## What These Resources Do

Each entry follows this pattern:

```xml
<data name="WorkCategory_CUP" xml:space="preserve">
  <value>Kubek</value>
  <comment>Work category: Cup</comment>
</data>
```

- **name**: The resource key (e.g., `WorkCategory_CUP`)
- **value**: The translated text (e.g., "Kubek" in Polish, "Cup" in English)
- **comment**: Developer note explaining the context

## Resource Keys Added

### Work Categories (7 entries per language)
- `WorkCategory_CUP`
- `WorkCategory_BOWL`
- `WorkCategory_VASE`
- `WorkCategory_PLATE`
- `WorkCategory_SCULPTURE`
- `WorkCategory_TILE`
- `WorkCategory_OTHER`

### Work Statuses (8 entries per language)
- `WorkStatus_WET`
- `WorkStatus_LEATHER_HARD`
- `WorkStatus_BONE_DRY`
- `WorkStatus_BISQUE_FIRED`
- `WorkStatus_GLAZED`
- `WorkStatus_GLAZE_FIRED`
- `WorkStatus_COMPLETED`
- `WorkStatus_DISCARDED`

**Total**: 15 entries per language file

## Troubleshooting

### Problem: Build errors after adding resources
**Solution**: Make sure you pasted the XML before the closing `</root>` tag and that the XML is well-formed.

### Problem: Still seeing codes like "CUP" instead of "Kubek"
**Solution**: 
1. Verify the resources were added correctly
2. Clean and rebuild the solution
3. Check that the resource keys match exactly (case-sensitive)

### Problem: Resource designer file not updating
**Solution**: 
1. Right-click on `AppResources.resx`
2. Select "Run Custom Tool"
3. This will regenerate the `AppResources.Designer.cs` file

## Need Help?

If you encounter issues:
1. Check the `LOCALIZATION_IMPLEMENTATION.md` file for detailed information
2. Review the `IMPLEMENTATION_SUMMARY.md` file for technical details
3. Verify that all code files were properly created and modified

## What's Already Done

? Created converter classes for categories and statuses
? Updated the Work model to support localization
? Modified all XAML files to use converters
? Registered converters in App.xaml
? Build verified - all code compiles successfully

## What You Need to Do

?? Add resource strings to `.resx` files (critical step)
? Test the application
? Clean up temporary helper files (optional)

---

**Estimated Time**: 5-10 minutes
**Difficulty**: Easy (copy and paste)
**Impact**: High (enables full localization support)
