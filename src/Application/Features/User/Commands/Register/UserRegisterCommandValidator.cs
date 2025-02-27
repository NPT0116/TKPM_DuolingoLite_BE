using System;
using Domain.Entities.Media;
using Domain.Entities.Media.Enums;
using Domain.Entities.Users.Constants;
using FluentValidation;

namespace Application.Features.User.Commands.Register;

public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterCommandValidator()
    {
        RuleFor(x => x.UserRegisterDto.FirstName)
            .NotEmpty()
            .MaximumLength(UserConstants.MAXIMUM_FIRSTNAME_LENGTH);
        RuleFor(x => x.UserRegisterDto.LastName)
            .NotEmpty()
            .MaximumLength(UserConstants.MAXIMUM_LASTNAME_LENGTH);
        RuleFor(x => x.UserRegisterDto.Email)
            .NotEmpty()
            .Matches(@"^(?!.*\.\.)(?!.*\s)(?!.*@-)[A-Za-z0-9._%+-]+@(?!-)[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")
            .WithMessage("Invalid email format.");


        RuleFor(x => x.UserRegisterDto.UserName)
            .NotEmpty()
            .MinimumLength(UserConstants.MINIMUM_USERNAME_LENGTH)
            .MaximumLength(UserConstants.MAXIMUM_USERNAME_LENGTH);
        RuleFor(x => x.UserRegisterDto.Password)
            .NotEmpty()
            .MinimumLength(UserConstants.MINIMUM_PASSWORD_LENGTH)
            .MaximumLength(UserConstants.MAXIMUM_PASSWORD_LENGTH);

        RuleFor(x => x.AvatarUploadRequest)
        .Must(avatar => avatar == null || 
                        Domain.Entities.Media.Media.GetMediaType(avatar.ContentType).IsSuccess && 
                        Domain.Entities.Media.Media.GetMediaType(avatar.ContentType).Value == MediaType.Image)
        .WithMessage("Invalid file type");


    }
}
