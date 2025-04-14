using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Courses;

namespace Application.Features.Learning.Courses.Commands.EditCourse;

public record EditCourseDto(string newName);

public record EditCourseCommand(Guid courseId, EditCourseDto dto) : ICommand<Course>;