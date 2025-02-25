using System;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using FluentValidation;

namespace Application.Features.User.Commands.Register;

public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterCommandValidator()
    {
        RuleFor(x => x.UserRegisterDto.FirstName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.UserRegisterDto.LastName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.UserRegisterDto.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.UserRegisterDto.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);
        RuleFor(x => x.UserRegisterDto.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);

        RuleFor(x => x.AvatarUploadRequest)
        .Must(avatar => avatar == null || 
                        Domain.Entities.Media.Media.GetMediaType(avatar.ContentType).IsSuccess && 
                        Domain.Entities.Media.Media.GetMediaType(avatar.ContentType).Value == MediaType.Image)
        .WithMessage("Invalid file type");


    }
}
