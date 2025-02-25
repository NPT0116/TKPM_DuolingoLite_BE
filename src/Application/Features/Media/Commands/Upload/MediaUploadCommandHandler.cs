using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Interface;
using Domain.Repositories;
using MediatR;
using SharedKernel;
namespace Application.Features.Media.Commands.Upload
{
    public class MediaUploadCommandHandler : IRequestHandler<MediaUploadCommand, Result<string>>
    {
        private readonly IMediaStorageService _mediaStorageService;
        private readonly IMediaRepository _mediaRepository;
        private readonly IApplicationDbContext _context;
        public MediaUploadCommandHandler(IMediaStorageService mediaStorageService, IMediaRepository mediaRepository, IApplicationDbContext context)
        {
            _mediaStorageService = mediaStorageService;
            _mediaRepository = mediaRepository;
            _context = context;
        }
        
        public async Task<Result<string>> Handle(MediaUploadCommand request, CancellationToken cancellationToken)
        {
            var uploadResult = await _mediaStorageService.UploadFileAsync(request.Request, cancellationToken);
            if (uploadResult.IsFailure)
            {
                return Result.Failure<string>(uploadResult.Error);
            }

            var mediaType = Domain.Entities.Media.Media.GetMediaType(request.Request.FileName);

            await _mediaRepository.UploadFileAsync(request.Request.FileName, uploadResult.Value, mediaType.Value, request.Request.FileData.Length, DateTime.UtcNow, DateTime.UtcNow, cancellationToken);    
            await _context.SaveChangesAsync(cancellationToken);
            
            return Result.Success<string>(uploadResult.Value);
        }
    }
}