using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Entities.User;
using Domain.Entities.Users;
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
        if (await _identityService.UseEmailExistsAsync(request.UserRegisterDto.Email))
        {
            return Result.Failure<Guid>(UserError.EmailNotUnique) as Result<Guid>;
        }
        if (await _identityService.UserNameExistsAsync(request.UserRegisterDto.UserName))
        {
            return Result.Failure<Guid>(UserError.UserNameNotUnique);
        }
        var result = await _identityService.CreateUserAsync(
            request.UserRegisterDto.FirstName, 
            request.UserRegisterDto.LastName, 
            request.UserRegisterDto.Email, 
            request.UserRegisterDto.UserName, 
            request.UserRegisterDto.Password);

        if (result.Result.IsFailure)
        {
            return Result.Failure<Guid>(result.Result.Error);
        }

        var userId = result.UserId;

        var userActivity = UserActivity.Create(userId, DateTime.UtcNow, true);
        if (userActivity.IsFailure)
        {
            return Result.Failure<Guid>(userActivity.Error);
        }

        var userStats = UserStats.Create(userId);
        if (userStats.IsFailure)
        {
            return Result.Failure<Guid>(userStats.Error);
        }

        Domain.Entities.Media.Media? avatarMedia = null;

        if(request.AvatarUploadRequest != null)
        {
            Console.WriteLine($"Media type: {Domain.Entities.Media.Media.GetMediaTypeFromContentType(request.AvatarUploadRequest.ContentType).Value}");
            if(Domain.Entities.Media.Media.GetMediaTypeFromContentType(request.AvatarUploadRequest.ContentType).Value != MediaType.Image)
            {
                return Result.Failure<Guid>(MediaError.InvalidFileType());
            }
            var avatarUploadRequest = new MediaUploadRequest(
                _mediaSettings.AvatarPrefix,
                request.AvatarUploadRequest.FileData, 
                request.AvatarUploadRequest.FileName, 
                request.AvatarUploadRequest.ContentType);
            var avatarUploadResult = await _mediaStorageService.UploadFileAsync(avatarUploadRequest, cancellationToken);
            if (avatarUploadResult.IsFailure)
            {
                return Result.Failure<Guid>(avatarUploadResult.Error);
            }
            avatarMedia = avatarUploadResult.Value;
        }

        var userProfile = UserProfile.Create(
            userId, 
            request.UserRegisterDto.Email, 
            request.UserRegisterDto.UserName, 
            request.UserRegisterDto.FirstName, 
            request.UserRegisterDto.LastName, 
            avatarMedia, 
            null);
        if (userProfile.IsFailure)
        {
            return Result.Failure<Guid>(userProfile.Error);
        }

        await _userRepository.CreateUserActivity(userActivity.Value);
        await _userRepository.CreateUserStats(userStats.Value);
        await _userRepository.CreateUserProfile(userProfile.Value);

        await _context.SaveChangesAsync(cancellationToken);

        return userId;
    }
}
