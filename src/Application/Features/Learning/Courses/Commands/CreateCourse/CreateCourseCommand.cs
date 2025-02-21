using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Courses;

namespace Application.Features.Learning.Courses;

public record CreateCourseDto(string Name, int Level);
public record CreateCourseCommand(CreateCourseDto CreateCourseDto) : ICommand<Course>;
