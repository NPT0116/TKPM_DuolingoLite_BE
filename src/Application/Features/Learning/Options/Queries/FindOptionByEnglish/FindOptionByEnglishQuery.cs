using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Options;
using MediatR;

namespace Application.Features.Learning.Options.Queries.FindOptionByEnglish;

public record FindOptionByEnglishQuery(string EnglishText) : IQuery<List<Option>>;