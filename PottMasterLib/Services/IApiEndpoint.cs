using PottMasterLib.Models;

namespace PottMasterLib.Services;

public interface IApiEndpoint
{
    Task<Result<List<IWork>>> GetWorksByUserIdAsync(string userId);
    Task<Result<IWork>> GetWorkByIdAsync(string workId);
    Task<Result<IWork>> UpsertWorkAsync(IWork work);
    Task<Result> DeleteWorkAsync(string workId);

    Task<Result<List<IWorkCategory>>> GetWorkCategoriesAsync();
    Task<Result<List<IWorkStatus>>> GetWorkStatusesAsync();

    Task<Result<List<IGlazeType>>> GetGlazeTypesAsync();
    Task<Result<List<IGlaze>>> GetGlazesAsync();

    Task<Result<IUserProfile>> GetUserProfileByIdAsync(string userId);
    Task<Result<IUserProfile>> UpsertUserProfileAsync(IUserProfile profile);

    Task<Result<string>> UploadPhotoAsync(LocalPhoto photo);
    Task<Result<List<Photo>>> GetPhotosByWorkIdAsync(string workId);
    Task<Result<byte[]>> DownloadPhotoAsync(string remotePath);
}
