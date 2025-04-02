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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.Services
{
    public class AwsS3StorageService : IMediaStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsSettings _awsSettings;
        private readonly IMediaRepository _mediaRepository;
        private readonly ILogger<AwsS3StorageService> _logger;
        private string BaseUrl(string bucketName) => $"https://{bucketName}.s3.amazonaws.com/";
        public AwsS3StorageService(
            IAmazonS3 s3Client, 
            IOptions<AwsSettings> awsSettings,
            IMediaRepository mediaRepository,
            ILogger<AwsS3StorageService> logger)
        {
            _s3Client = s3Client;
            _awsSettings = awsSettings.Value; 
            _mediaRepository = mediaRepository;
            _logger = logger;
        }

        public async Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken)
        {
            var bucketName = _awsSettings.BucketName;
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucketName);
            if (!bucketExists) return false;
            await _s3Client.DeleteObjectAsync(bucketName, fileKey);
            return true;
        }

        public async Task ScanAsync(List<string> folders, DateTime fromUtc)
        {
            var bucketName = _awsSettings.BucketName;
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if(!bucketExists) return;
            Console.WriteLine("Start cleaning...");

            foreach(var folder in folders)
            {
                Console.WriteLine("Clean in folder " + folder);
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = folder
                };

                ListObjectsV2Response response;
                do
                {
                    response = await _s3Client.ListObjectsV2Async(listRequest);
                    foreach (var s3Object in response.S3Objects)
                    {
                        if(s3Object.LastModified.ToUniversalTime() > fromUtc)
                        {
                            var mediaEntry = await _mediaRepository.GetMediaByKey(s3Object.Key);
                            if (mediaEntry == null)
                            {
                                // File is orphaned, attempt to delete.
                                try
                                {
                                    await _s3Client.DeleteObjectAsync(bucketName, s3Object.Key);
                                    _logger.LogInformation($"Deleted orphaned file: {s3Object.Key}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error deleting file: {s3Object.Key}");
                                }
                            }
                        }
                    }

                    listRequest.ContinuationToken = response.NextContinuationToken;
                } while(response.IsTruncated);
            }
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
            
            var fileUrl = $"{BaseUrl(bucketName)}{fileKey}";

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

        public async Task<bool> DeleteFileFromUrl(string url)
        {
            var bucketName = _awsSettings.BucketName;
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucketName);
            if (!bucketExists) return false;
            
            var baseUrl = BaseUrl(bucketName);
            if(!url.StartsWith(baseUrl)) return false;
            string fileKey = url.Substring(baseUrl.Length);
            return await DeleteFileAsync(fileKey, CancellationToken.None);
        }
    }
}