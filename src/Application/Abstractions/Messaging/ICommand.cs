using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;

namespace Application.Abstractions.Messaging
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }

    public interface IBaseCommand;
}
