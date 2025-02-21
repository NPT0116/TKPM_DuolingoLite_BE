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
            long fileSize
        )
        {
            Id = id;
            FileName = fileName;
            Url = url;
            MimeType = mimeType;
            Size = fileSize;
            CreatedAt =  DateTime.UtcNow;
        }

        public static Result<Media> Create(
            string fileName,
            MediaType mimeType,
            long fileSize,
            string url
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
                fileSize
            ));
        }

    }
}