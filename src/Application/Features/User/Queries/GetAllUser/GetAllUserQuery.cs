using System;
using Application.Abstractions.Messaging;
using Application.Features.User.Queries.GetUserProfile;
using Domain.Query.User;
using SharedKernel;

namespace Application.Features.User.Queries.GetAllUser;


public record GetAllUserQuery(GetAllUserQueryParams GetAllUserQueryParams) : IPageQuery<UserWithProfileResponseDto>;