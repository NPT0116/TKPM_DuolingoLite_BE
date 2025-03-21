using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace Application.Features.User.Commands.UserProfile.UploadProfile
{
    public class UploadProfileCommandHandler : ICommandHandler<UploadProfileCommand, string>
    {
        private readonly IMediaStorageService _uploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly MediaSettings _mediaSettings;

        public UploadProfileCommandHandler(
            IMediaStorageService uploadService,
            MediaSettings mediaSettings,
            IHttpContextAccessor accessor,
            IUserRepository userRepository,
            IMediaRepository mediaRepository,
            IApplicationDbContext dbContext)
        {
            _uploadService = uploadService;
            _mediaSettings = mediaSettings;
            _httpContextAccessor = accessor;
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _dbContext = dbContext;
        }
        public async Task<Result<string>> Handle(UploadProfileCommand command, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if(user == null) return Result.Failure<string>(UserError.Unauthorized());

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Result.Failure<string>(UserError.Unauthorized());

            var request = command.request;
            var mediaTypeResult = Domain.Entities.Media.Media.GetMediaTypeFromContentType(request.ContentType);

            if (mediaTypeResult.Value != MediaType.Image)
            {
                return Result.Failure<string>(MediaError.InvalidFileType());
            }
            var avatarUploadRequest = new MediaUploadRequest(
                _mediaSettings.AvatarPrefix,
                request.FileData,
                request.FileName,
                request.ContentType);
            var avatarUploadResult = await _uploadService.UploadFileAsync(avatarUploadRequest, cancellationToken);
            if (avatarUploadResult.IsFailure)
            {
                return Result.Failure<string>(avatarUploadResult.Error);
            }

            var userProfile = await _userRepository.GetUserProfileById(Guid.Parse(userId));
            if(userProfile == null) return Result.Failure<string>(UserError.UserProfileNotFound(Guid.Parse(userId)));

            if(userProfile.ProfileImage != null)
            {
                var deleteExistingProfileImage = await _uploadService.DeleteFileAsync(userProfile.ProfileImage.FileKey, cancellationToken);
                if(deleteExistingProfileImage)
                {
                    await _mediaRepository.DeleteFile(userProfile.ProfileImage);
                }
            }

            userProfile.UpdateProfileImage(avatarUploadResult.Value);
            await _dbContext.SaveChangesAsync();

            return Result.Success(avatarUploadResult.Value.FileName);
        }
    }
}