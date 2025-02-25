using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Domain.Entities.Media;
using SharedKernel;

namespace Infrastructure.Services
{
    public class AwsS3StorageService : IMediaStorageService
    {
        private readonly IAmazonS3 _s3Client;

        public AwsS3StorageService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        public async Task<Result<Media>> UploadFileAsync(MediaUploadRequest request, CancellationToken cancellationToken)
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,request.BucketName);
            if (!bucketExists) return Result.Failure<Media>(MediaError.BucketDoesNotExist());
            var fileKey = Domain.Entities.Media.Media.GetFileKey(request.Prefix, request.FileName);
            var putObjectRequestrequest = new PutObjectRequest()
            {
                BucketName = request.BucketName,
                Key = fileKey,
                InputStream = new MemoryStream(request.FileData),
                ContentType = request.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };
            await _s3Client.PutObjectAsync(putObjectRequestrequest);
            
            var fileUrl = $"https://{request.BucketName}.s3.amazonaws.com/{fileKey}";

            var createdMedia = Domain.Entities.Media.Media.Create(request.FileName, Media.GetMediaType(request.ContentType).Value, request.FileData.Length, fileUrl);
            if (createdMedia.IsFailure)
            {
                return Result.Failure<Media>(createdMedia.Error);
            }
            return Result.Success<Media>(createdMedia.Value);
        }
    }
}