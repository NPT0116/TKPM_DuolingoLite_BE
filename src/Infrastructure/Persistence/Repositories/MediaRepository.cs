using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Persistence.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly ApplicationDbContext _context;

        public MediaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Media>> DeleteFile(Media media)
        {
            var mediaToRemove = await _context.Medias.FirstOrDefaultAsync(m => m.Id == media.Id);
            if(mediaToRemove == null) return Result.Failure<Media>(MediaError.NotFound());
            _context.Medias.Remove(media);
            await _context.SaveChangesAsync();

            return Result.Success<Media>(media);
        }

        public async Task<Media?> GetMediaByKey(string key)
        {
            return await _context.Medias.FirstOrDefaultAsync(m => m.FileKey == key);
        }

        public async Task<Result<Media>> UploadFileAsync(string fileName, string url, MediaType mimeType, long fileSize, DateTime createdAt, DateTime updatedAt, string fileKey, CancellationToken cancellationToken)
        {
            var file = Media.Create(fileName, mimeType, fileSize, url, fileKey);
            if (file.IsFailure)
            {
                return Result.Failure<Media>(file.Error);
            }
            _context.Medias.Add(file.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success<Media>(file.Value);
        }
    }
}