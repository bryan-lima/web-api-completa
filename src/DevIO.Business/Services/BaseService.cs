using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Business.Notificacoes;
using FluentValidation;
using FluentValidation.Results;

namespace DevIO.Business.Services
{
    public abstract class BaseService
    {
        #region Private Fields

        private readonly INotificador _notificador;

        #endregion Private Fields

        #region Protected Constructors

        protected BaseService(INotificador notificador)
        {
            _notificador = notificador;
        }

        #endregion Protected Constructors

        #region Protected Methods

        protected void Notificar(ValidationResult validationResult)
        {
            foreach (ValidationFailure error in validationResult.Errors)
                Notificar(error.ErrorMessage);
        }

        protected void Notificar(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            ValidationResult _validacao = validacao.Validate(entidade);

            if (_validacao.IsValid)
                return true;

            Notificar(_validacao);

            return false;
        }

        #endregion Protected Methods
    }
}