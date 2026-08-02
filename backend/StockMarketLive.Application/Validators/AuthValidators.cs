using FluentValidation;
using StockMarketLive.Application.DTOs.Auth;
using StockMarketLive.Domain.Constants;

namespace StockMarketLive.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode(AppConstants.ErrorCodes.Auth.UsernameEmpty);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(AppConstants.ErrorCodes.Auth.PasswordEmpty);
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithErrorCode(AppConstants.ErrorCodes.Auth.UsernameEmpty)
            .MinimumLength(3).WithErrorCode(AppConstants.ErrorCodes.Auth.UsernameTooShort);
            
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(AppConstants.ErrorCodes.Auth.EmailEmpty)
            .EmailAddress().WithErrorCode(AppConstants.ErrorCodes.Auth.EmailInvalid);
            
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(AppConstants.ErrorCodes.Auth.PasswordEmpty)
            .MinimumLength(6).WithErrorCode(AppConstants.ErrorCodes.Auth.PasswordTooShort);
    }
}

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(AppConstants.ErrorCodes.Role.NameEmpty)
            .MinimumLength(2).WithErrorCode(AppConstants.ErrorCodes.Role.NameTooShort);
    }
}
