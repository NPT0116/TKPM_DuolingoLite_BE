using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Media.Commands.Upload;
using SharedKernel;
namespace Application.Common.Interface
{
    public interface IMediaStorageService
    {
        Task<Result<string>> UploadFileAsync(MediaUploadRequest request, CancellationToken cancellationToken);
    }
}