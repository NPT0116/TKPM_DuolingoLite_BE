using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using SharedKernel;

namespace Application.Features.User.Commands.UserProfile.UploadProfile
{
    public class UploadProfileCommandHandler : ICommandHandler<UploadProfileCommand, string>
    {
        private readonly IMediaStorageService _uploadService;
        private readonly MediaSettings _mediaSettings;

        public UploadProfileCommandHandler(
            IMediaStorageService uploadService,
            MediaSettings mediaSettings)
        {
            _uploadService = uploadService;
            _mediaSettings = mediaSettings;
        }
        public async Task<Result<string>> Handle(UploadProfileCommand command, CancellationToken cancellationToken)
        {
            var request = command.request;
            var mediaTypeResult = Domain.Entities.Media.Media.GetMediaTypeFromContentType(request.ContentType);
                Console.WriteLine($"Media type: {mediaTypeResult.Value}");
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

                return Result.Success(avatarUploadRequest.FileName);
        }
    }
}