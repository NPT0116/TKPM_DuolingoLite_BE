using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Media.Commands.Upload;
using Domain.Entities.Media;
using SharedKernel;
namespace Application.Common.Interface
{
    public interface IMediaStorageService
    {
        Task<Result<Media>> UploadFileAsync(MediaUploadRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken);
    }
}