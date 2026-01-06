using Microsoft.Extensions.Logging;
using PottMasterLib.Models;
using Supabase.Postgrest;
using System.IO;

namespace PottMasterLib.Services;

public class SupabaseApi : IApiEndpoint
{
    private readonly Supabase.Client _supabaseClient;
    private readonly ILogger<SupabaseApi> _logger;

    public SupabaseApi(Supabase.Client supabaseClient, ILogger<SupabaseApi> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<Result<List<IWork>>> GetWorksByUserIdAsync(string userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<RemoteWork>()
                .Filter("user_id", Constants.Operator.Equals, userId)
                .Get();

            var works = response.Models.Cast<IWork>().ToList();
            return Result<List<IWork>>.Success(works);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get works for user {UserId}", userId);
            return Result<List<IWork>>.Failure("Failed to load works from server", ex);
        }
    }

    public async Task<Result<IWork>> GetWorkByIdAsync(string workId)
    {
        try
        {
            var response = await _supabaseClient
                .From<RemoteWork>()
                .Filter("id", Constants.Operator.Equals, workId)
                .Single();

            if (response == null)
            {
                return Result<IWork>.Failure("Work not found");
            }

            return Result<IWork>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work {WorkId}", workId);
            return Result<IWork>.Failure("Failed to load work from server", ex);
        }
    }

    public async Task<Result<IWork>> UpsertWorkAsync(IWork work)
    {
        try
        {
            var remoteWork = new RemoteWork
            {
                Id = work.Id,
                UserId = work.UserId,
                Code = work.Code,
                CategoryId = work.CategoryId,
                WallThickness = work.WallThickness,
                PhotoPath = work.PhotoPath,
                StatusId = work.StatusId,
                CreatedAt = work.CreatedAt,
                DryingStartedAt = work.DryingStartedAt,
                DryingCompletedAt = work.DryingCompletedAt,
                SyncStatus = work.SyncStatus,
                UpdatedAt = work.UpdatedAt
            };

            var response = await _supabaseClient
                .From<RemoteWork>()
                .Upsert(remoteWork);

            var upserted = response.Models.FirstOrDefault();
            if (upserted == null)
            {
                return Result<IWork>.Failure("Failed to upsert work");
            }

            return Result<IWork>.Success(upserted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert work {WorkId}", work.Id);
            return Result<IWork>.Failure("Failed to save work to server", ex);
        }
    }

    public async Task<Result> DeleteWorkAsync(string workId)
    {
        try
        {
            await _supabaseClient
                .From<RemoteWork>()
                .Filter("id", Constants.Operator.Equals, workId)
                .Delete();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete work {WorkId}", workId);
            return Result.Failure("Failed to delete work from server", ex);
        }
    }

    public async Task<Result<List<IWorkCategory>>> GetWorkCategoriesAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<WorkCategory>()
                .Get();

            var categories = response.Models.Cast<IWorkCategory>().ToList();
            return Result<List<IWorkCategory>>.Success(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work categories");
            return Result<List<IWorkCategory>>.Failure("Failed to load categories from server", ex);
        }
    }

    public async Task<Result<List<IWorkStatus>>> GetWorkStatusesAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<WorkStatus>()
                .Get();

            var statuses = response.Models.Cast<IWorkStatus>().ToList();
            return Result<List<IWorkStatus>>.Success(statuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work statuses");
            return Result<List<IWorkStatus>>.Failure("Failed to load statuses from server", ex);
        }
    }

    public async Task<Result<List<IGlazeType>>> GetGlazeTypesAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<GlazeType>()
                .Get();

            var types = response.Models.Cast<IGlazeType>().ToList();
            return Result<List<IGlazeType>>.Success(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get glaze types");
            return Result<List<IGlazeType>>.Failure("Failed to load glaze types from server", ex);
        }
    }

    public async Task<Result<List<IGlaze>>> GetGlazesAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<Glaze>()
                .Get();

            var glazes = response.Models.Cast<IGlaze>().ToList();
            return Result<List<IGlaze>>.Success(glazes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get glazes");
            return Result<List<IGlaze>>.Failure("Failed to load glazes from server", ex);
        }
    }

    public async Task<Result<IUserProfile>> GetUserProfileByIdAsync(string userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<UserProfiles>()
                .Filter("id", Constants.Operator.Equals, userId)
                .Single();

            if (response == null)
            {
                return Result<IUserProfile>.Failure("User profile not found");
            }

            return Result<IUserProfile>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user profile {UserId}", userId);
            return Result<IUserProfile>.Failure("Failed to load user profile from server", ex);
        }
    }

    public async Task<Result<IUserProfile>> UpsertUserProfileAsync(IUserProfile profile)
    {
        try
        {
            var remoteProfile = new UserProfiles
            {
                Id = profile.Id,
                Initials = profile.Initials,
                Email = profile.Email,
                CreatedAt = profile.CreatedAt
            };

            var response = await _supabaseClient
                .From<UserProfiles>()
                .Upsert(remoteProfile);

            var upserted = response.Models.FirstOrDefault();
            if (upserted == null)
            {
                return Result<IUserProfile>.Failure("Failed to upsert user profile");
            }

            return Result<IUserProfile>.Success(upserted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert user profile {UserId}", profile.Id);
            return Result<IUserProfile>.Failure("Failed to save user profile to server", ex);
        }
    }

    public async Task<Result<string>> UploadPhotoAsync(LocalPhoto photo)
    {
        try
        {
            // Upload to storage
            var fileName = $"{photo.WorkId}/{photo.Id}.jpg"; // Assuming jpg, adjust if needed
            var fileBytes = await File.ReadAllBytesAsync(photo.Path);
            var uploadResponse = await _supabaseClient.Storage
                .From("photos")
                .Upload(fileBytes, fileName);

            if (uploadResponse == null)
            {
                return Result<string>.Failure("Failed to upload photo to storage");
            }

            // Get public URL
            var publicUrl = _supabaseClient.Storage
                .From("photos")
                .GetPublicUrl(fileName);

            // Insert into photos table
            var remotePhoto = new Photo
            {
                Id = photo.Id,
                WorkId = photo.WorkId,
                RemotePath = publicUrl,
                Order = photo.Order,
                CreatedAt = photo.CreatedAt,
                UpdatedAt = photo.UpdatedAt
            };

            var insertResponse = await _supabaseClient
                .From<Photo>()
                .Insert(remotePhoto);

            if (insertResponse.Models.Count == 0)
            {
                return Result<string>.Failure("Failed to insert photo record");
            }

            return Result<string>.Success(publicUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload photo {PhotoId}", photo.Id);
            return Result<string>.Failure("Failed to upload photo", ex);
        }
    }

    public async Task<Result<List<Photo>>> GetPhotosByWorkIdAsync(string workId)
    {
        try
        {
            var response = await _supabaseClient
                .From<Photo>()
                .Filter("work_id", Constants.Operator.Equals, workId)
                .Get();

            var photos = response.Models.ToList();
            return Result<List<Photo>>.Success(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get photos for work {WorkId}", workId);
            return Result<List<Photo>>.Failure("Failed to load photos from server", ex);
        }
    }

    public async Task<Result<byte[]>> DownloadPhotoAsync(string remotePath)
    {
        try
        {
            // Assuming remotePath is the public URL
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(remotePath);
            if (!response.IsSuccessStatusCode)
            {
                return Result<byte[]>.Failure("Failed to download photo");
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return Result<byte[]>.Success(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download photo from {RemotePath}", remotePath);
            return Result<byte[]>.Failure("Failed to download photo", ex);
        }
    }
}
