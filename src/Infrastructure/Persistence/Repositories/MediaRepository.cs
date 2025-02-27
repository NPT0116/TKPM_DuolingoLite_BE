using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using Infrastructure.Data;
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
        public async Task<Result<string>> UploadFileAsync(string fileName, string url, MediaType mimeType, long fileSize, DateTime createdAt, DateTime updatedAt, string fileKey, CancellationToken cancellationToken)
        {
            var file = Media.Create(fileName, mimeType, fileSize, url, fileKey);
            if (file.IsFailure)
            {
                return Result.Failure<string>(file.Error);
            }
            _context.Medias.Add(file.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success<string>(file.Value.Id.ToString());
        }
    }
}