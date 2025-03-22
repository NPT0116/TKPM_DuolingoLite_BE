using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Features.User.Commands.Common;

namespace Application.Features.User.Commands.UserProfile.UploadProfile;

public record UploadProfileCommand(AvatarUploadRequest request) : ICommand<string>;