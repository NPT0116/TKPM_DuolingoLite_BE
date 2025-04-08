using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Features.Learning.Lessons.Commands.AddQuestions;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : ICommandHandler<UpdateQuestionCommand>
    {
        private readonly IMediator _mediator;
        public UpdateQuestionCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var command = new AddQuestionCommand(request.lessonId, request.dto);
            await _mediator.Send(command);

            return Result.Success();
        }
    }
}