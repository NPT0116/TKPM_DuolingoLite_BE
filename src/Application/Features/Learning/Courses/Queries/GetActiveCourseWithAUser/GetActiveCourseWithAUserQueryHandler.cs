using System;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Learning.Courses.Queries.GetActiveCourseWithAUser;

public class GetActiveCourseWithAUserQueryHandler : IQueryHandler<GetActiveCourseWithAUserQuery, GetActiveCourseWithAUserResponseDto>
{
    public Task<Result<GetActiveCourseWithAUserResponseDto>> Handle(GetActiveCourseWithAUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
