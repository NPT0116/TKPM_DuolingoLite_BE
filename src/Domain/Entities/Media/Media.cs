using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media.Enums;
using SharedKernel;

namespace Domain.Entities.Media
{
    public class Media : Entity
    {
        public string FileKey { get; private set; } = String.Empty; 
        public string FileName { get; private set; } = String.Empty;
        public string Url { get; private set; } = String.Empty;
        public MediaType MimeType { get; private set; }
        public long Size { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Media() {}
        private Media(
            Guid id,
            string fileName,
            string url,
            MediaType mimeType,
            long fileSize,
            string fileKey
        )
        {
            Id = id;
            FileName = fileName;
            Url = url;
            MimeType = mimeType;
            Size = fileSize;
            FileKey = fileKey;
            CreatedAt =  DateTime.UtcNow;
        }

        public static Result<Media> Create(
            string fileName,
            MediaType mimeType,
            long fileSize,
            string url,
            string fileKey
        )
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return Result.Failure<Media>(MediaError.NameEmpty());
            }

            if(fileSize <= 0)
            {
                return Result.Failure<Media>(MediaError.InvalidFileFize());
            }

            if(string.IsNullOrEmpty(url))
            {
                return Result.Failure<Media>(MediaError.FilePathEmpty());
            }

            return Result.Success(new Media(
                Guid.NewGuid(),
                fileName,
                url,
                mimeType,
                fileSize,
                fileKey
            ));
        }

        public static Result<MediaType> GetMediaType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" => MediaType.Image,
                ".mp3" or ".wav" or ".m4a" => MediaType.Audio,
                ".pdf" or ".doc" or ".docx" => MediaType.Document,
                _ => MediaType.Other
            };
        }

        public static Result<MediaType> GetMediaTypeFromContentType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" or "image/png" => MediaType.Image,
                "audio/mpeg" or "audio/wav" or "audio/mp4" => MediaType.Audio,
                "application/pdf" or "application/msword" or "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => MediaType.Document,
                _ => MediaType.Other
            };
}

        
        public static string GetFileKey(string? prefix, string fileName)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Timestamp format: 20240225123045987
            string fileExtension = Path.GetExtension(fileName); // Extract file extension
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName); // Extract file name without extension

            string newFileName = $"{fileNameWithoutExt}_{timestamp}{fileExtension}"; // Append timestamp to filename

            return string.IsNullOrEmpty(prefix) ? newFileName : $"{prefix.TrimEnd('/')}/{newFileName}";
        }


    }
}