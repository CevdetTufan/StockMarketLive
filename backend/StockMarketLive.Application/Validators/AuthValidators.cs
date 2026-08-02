using FluentValidation;
using StockMarketLive.Application.DTOs.Auth;

namespace StockMarketLive.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı adı boş olamaz.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre boş olamaz.");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).WithMessage("Rol adı en az 2 karakter olmalıdır.");
    }
}
