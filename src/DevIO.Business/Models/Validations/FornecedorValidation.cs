using DevIO.Business.Models.Validations.Documentos;
using FluentValidation;

namespace DevIO.Business.Models.Validations
{
    public class FornecedorValidation : AbstractValidator<Fornecedor>
    {
        public FornecedorValidation()
        {
            RuleFor(fornecedor => fornecedor.Nome)
                .NotEmpty()
                    .WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 100)
                    .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            When(fornecedor => fornecedor.TipoFornecedor == TipoFornecedor.PessoaFisica, () =>
            {
                RuleFor(fornecedor => fornecedor.Documento.Length)
                    .Equal(CpfValidacao.TAMANHO_CPF)
                        .WithMessage("O campo Documento precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");
                RuleFor(fornecedor=> CpfValidacao.Validar(fornecedor.Documento))
                    .Equal(true)
                        .WithMessage("O documento fornecido é inválido.");
            });

            When(fornecedor => fornecedor.TipoFornecedor == TipoFornecedor.PessoaJuridica, () =>
            {
                RuleFor(fornecedor => fornecedor.Documento.Length)
                    .Equal(CnpjValidacao.TAMANHO_CNPJ)
                        .WithMessage("O campo Documento precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");
                RuleFor(fornecedor => CnpjValidacao.Validar(fornecedor.Documento))
                    .Equal(true)
                        .WithMessage("O documento fornecido é inválido.");
            });
        }
    }
}