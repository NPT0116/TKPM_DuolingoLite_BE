using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.User.Commands.Common;

public record AvatarUploadRequest(byte[] FileData, string FileName, string ContentType);