namespace PottMaster.Services;

public interface IImageService
{
    Task<string> CompressAndSaveImageAsync(string sourcePath, int maxWidth = 1920, int maxHeight = 1920, int quality = 80);
    Task<string> CompressAndSaveImageAsync(Stream sourceStream, string fileName, int maxWidth = 1920, int maxHeight = 1920, int quality = 80);
    Task DeleteImageAsync(string imagePath);
}
