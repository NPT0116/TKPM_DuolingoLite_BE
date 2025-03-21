using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using Infrastructure.Services.Settings;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.Services
{
    public class AwsS3StorageService : IMediaStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsSettings _awsSettings;
        private readonly IMediaRepository _mediaRepository;

        public AwsS3StorageService(
            IAmazonS3 s3Client, 
            IOptions<AwsSettings> awsSettings,
            IMediaRepository mediaRepository)
        {
            _s3Client = s3Client;
            _awsSettings = awsSettings.Value;
            _mediaRepository = mediaRepository;
        }

        public async Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken)
        {
            var bucketName = _awsSettings.BucketName;
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucketName);
            if (!bucketExists) return false;
            await _s3Client.DeleteObjectAsync(bucketName, fileKey);
            return true;
        }

        public async Task<Result<Media>> UploadFileAsync(MediaUploadRequest request, CancellationToken cancellationToken)
        {   
            var bucketName = _awsSettings.BucketName;
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucketName);
            if (!bucketExists) return Result.Failure<Media>(MediaError.BucketDoesNotExist());
            var fileKey = Domain.Entities.Media.Media.GetFileKey(request.Prefix, request.FileName);
            var putObjectRequestrequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = fileKey,
                InputStream = new MemoryStream(request.FileData),
                ContentType = request.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };
            await _s3Client.PutObjectAsync(putObjectRequestrequest);
            
            var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{fileKey}";

            var createdMedia = await _mediaRepository.UploadFileAsync(
                request.FileName,
                fileUrl,
                MediaType.Image,
                request.FileData.Length,
                DateTime.UtcNow,
                DateTime.UtcNow,
                fileKey,
                cancellationToken
            );
            return Result.Success<Media>(createdMedia.Value);
        }
    }
}