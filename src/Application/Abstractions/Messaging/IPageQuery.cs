using System;
using MediatR;
using SharedKernel;

namespace Application.Abstractions.Messaging;

public interface IPageQuery<TResponse> : IRequest<Result<PaginationResult<TResponse>>>;