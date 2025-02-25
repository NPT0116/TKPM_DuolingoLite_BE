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
        public async Task<Result<string>> UploadFileAsync(MediaUploadRequest request, CancellationToken cancellationToken)
        {
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,request.BucketName);
            if (!bucketExists) return Result.Failure<string>(MediaError.BucketDoesNotExist());
            var fileKey = string.IsNullOrEmpty(request.Prefix) ? request.FileName : $"{request.Prefix?.TrimEnd('/')}/{request.FileName}";
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
            return Result.Success<string>(fileUrl);
        }
    }
}