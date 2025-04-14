using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Lessons.Commands.RemoveQuestion;

public record RemoveQuestionCommand(Guid lessonId, int questionOrder) : ICommand;