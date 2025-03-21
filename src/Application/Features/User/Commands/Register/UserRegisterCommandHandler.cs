using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Entities.User;
using Domain.Entities.Users;
using Domain.Repositories;
using Domain.Repositories;
using SharedKernel;
using System;

namespace Application.Features.User.Commands.Register;

public class UserRegisterCommandHandler : ICommandHandler<UserRegisterCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly MediaSettings _mediaSettings;
    public UserRegisterCommandHandler(
        IIdentityService identityService, 
        IUserRepository userRepository, 
        IApplicationDbContext context, 
        IMediaStorageService mediaStorageService,
        MediaSettings mediaSettings)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _context = context;
        _mediaStorageService = mediaStorageService;
        _mediaSettings = mediaSettings;
    }
    public async Task<Result<Guid>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Media.Media? avatarMedia = null;
        // Begin a transaction (assuming your _context implements BeginTransactionAsync)
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        try
        {
            if (await _identityService.UseEmailExistsAsync(request.UserRegisterDto.Email))
            {
                throw new ApplicationErrorException(UserError.EmailNotUnique);
            }
            if (await _identityService.UserNameExistsAsync(request.UserRegisterDto.UserName))
            {
                throw new ApplicationErrorException(UserError.UserNameNotUnique);
            }

            var result = await _identityService.CreateUserAsync(
                request.UserRegisterDto.FirstName,
                request.UserRegisterDto.LastName,
                request.UserRegisterDto.Email,
                request.UserRegisterDto.UserName,
                request.UserRegisterDto.Password);

            if (result.Result.IsFailure)
            {
                throw new ApplicationErrorException(result.Result.Error);
            }

            var userId = result.UserId;

            var userActivity = UserActivity.Create(userId, DateTime.UtcNow, true);
            if (userActivity.IsFailure)
            {
                throw new ApplicationErrorException(userActivity.Error);
            }

            var userStats = UserStats.Create(userId);
            if (userStats.IsFailure)
            {
                throw new ApplicationErrorException(userStats.Error);
            }

            
            if (request.AvatarUploadRequest != null)
            {
                // Validate the content type directly
                var mediaTypeResult = Domain.Entities.Media.Media.GetMediaTypeFromContentType(request.AvatarUploadRequest.ContentType);
                Console.WriteLine($"Media type: {mediaTypeResult.Value}");
                if (mediaTypeResult.Value != MediaType.Image)
                {
                    throw new ApplicationErrorException(MediaError.InvalidFileType());
                }
                var avatarUploadRequest = new MediaUploadRequest(
                    _mediaSettings.AvatarPrefix,
                    request.AvatarUploadRequest.FileData,
                    request.AvatarUploadRequest.FileName,
                    request.AvatarUploadRequest.ContentType);
                var avatarUploadResult = await _mediaStorageService.UploadFileAsync(avatarUploadRequest, cancellationToken);
                if (avatarUploadResult.IsFailure)
                {
                    throw new ApplicationErrorException(avatarUploadResult.Error);
                }
                avatarMedia = avatarUploadResult.Value;
            }

            var userProfile = Domain.Entities.Users.UserProfile.Create(
                userId,
                request.UserRegisterDto.Email,
                request.UserRegisterDto.UserName,
                request.UserRegisterDto.FirstName,
                request.UserRegisterDto.LastName,
                avatarMedia,
                null);
            if (userProfile.IsFailure)
            {
                // Optionally, if an avatar was uploaded, you might consider deleting it here.
                throw new ApplicationErrorException(userProfile.Error);
            }

            await _userRepository.CreateUserActivity(userActivity.Value);
            await _userRepository.CreateUserStats(userStats.Value);
            await _userRepository.CreateUserProfile(userProfile.Value);

            await _context.SaveChangesAsync(cancellationToken);
            // Commit the transaction if all operations succeed.
            await _context.CommitTransactionAsync(transaction, cancellationToken);
            return userId;
        }
        catch (Exception ex)
        {
            if(avatarMedia != null)
            {
                await _mediaStorageService.DeleteFileAsync(avatarMedia.FileKey, cancellationToken);
            }
            await _context.RollbackTransactionAsync(transaction, cancellationToken);
            if (ex is ApplicationErrorException appEx)
            {
                return Result.Failure<Guid>(appEx.AppError);
            }
            return Result.Failure<Guid>(new Error("UnhandledError", ex.Message, ErrorType.Validation));
        }

    }

}
