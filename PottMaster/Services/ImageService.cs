namespace PottMaster.Services;

public class ImageService : IImageService
{
    public async Task<string> CompressAndSaveImageAsync(string sourcePath, int maxWidth = 1920, int maxHeight = 1920, int quality = 80)
    {
        using var sourceStream = File.OpenRead(sourcePath);
        var fileName = Path.GetFileName(sourcePath);
        return await CompressAndSaveImageAsync(sourceStream, fileName, maxWidth, maxHeight, quality);
    }

    public async Task<string> CompressAndSaveImageAsync(Stream sourceStream, string fileName, int maxWidth = 1920, int maxHeight = 1920, int quality = 80)
    {
        var compressedFileName = $"compressed_{DateTime.UtcNow:yyyyMMddHHmmss}_{fileName}";
        var targetPath = Path.Combine(FileSystem.AppDataDirectory, "photos", compressedFileName);

        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);

#if WINDOWS || MACCATALYST
        using var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(sourceStream);
#elif ANDROID
        using var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(sourceStream, Microsoft.Maui.Graphics.ImageFormat.Png);
#elif IOS
        using var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(sourceStream);
#else
        using var image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(sourceStream);
#endif
        
        if (image == null)
        {
            throw new InvalidOperationException("Failed to load image");
        }

        var originalWidth = image.Width;
        var originalHeight = image.Height;

        float scaleFactor = Math.Min(
            maxWidth / (float)originalWidth,
            maxHeight / (float)originalHeight
        );

        if (scaleFactor >= 1)
        {
            scaleFactor = 1;
        }

        var newWidth = (int)(originalWidth * scaleFactor);
        var newHeight = (int)(originalHeight * scaleFactor);

        var resizedImage = image.Resize(newWidth, newHeight);

        using var outputStream = File.Create(targetPath);
        await resizedImage.SaveAsync(outputStream, Microsoft.Maui.Graphics.ImageFormat.Jpeg, quality / 100f);

        return targetPath;
    }

    public Task DeleteImageAsync(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
        }
        return Task.CompletedTask;
    }
}
