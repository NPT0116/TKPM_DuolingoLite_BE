using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;
namespace Application.Features.Media.Commands.Upload;

public record MediaUploadRequest(
    string BucketName,
    string Prefix,
    byte[] FileData,
    string FileName,
    string ContentType
);

public record MediaUploadCommand(MediaUploadRequest Request) : IRequest<Result<string>>;
