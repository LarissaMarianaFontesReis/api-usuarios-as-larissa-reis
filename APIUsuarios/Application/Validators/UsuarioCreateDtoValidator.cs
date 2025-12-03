using APIUsuarios.Application.DTOs;
using FluentValidation;

namespace APIUsuarios.Application.Validators;

public class UsuarioCreateDtoValidator : AbstractValidator<UsuarioCreateDto>
{
    public UsuarioCreateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Usuário deve ter pelo menos 18 anos");

        RuleFor(x => x.Telefone)
            .Matches(@"^\([1-9]{2}\) (?:[2-8]|9[1-9])[0-9]{3}\-[0-9]{4}$")
            .When(x => !string.IsNullOrEmpty(x.Telefone))
            .WithMessage("Telefone inválido. Use o formato (XX) XXXXX-XXXX");
    }
}