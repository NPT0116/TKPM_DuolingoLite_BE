using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using SharedKernel;

namespace Domain.Repositories
{
    public interface IMediaRepository
    {
        Task<Result<Media>> UploadFileAsync(
            string fileName, 
            string url,
            MediaType mimeType,
            long fileSize,
            DateTime createdAt,
            DateTime updatedAt,
            string fileKey,
            CancellationToken cancellationToken);

        Task<Result<Media>> DeleteFile(Media media);
    }
}