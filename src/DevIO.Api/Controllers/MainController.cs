using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        #region Private Fields

        private readonly INotificador _notificador;

        #endregion Private Fields

        #region Protected Constructors

        protected MainController(INotificador notificador,
                                 IUser appUser)
        {
            _notificador = notificador;
            AppUser = appUser;

            if (appUser.IsAuthenticated())
            {
                UsuarioId = appUser.GetUserId();
                UsuarioAutenticado = true;
            }
        }

        #endregion Protected Constructors

        #region Protected Properties

        protected Guid UsuarioId { get; set; }
        protected bool UsuarioAutenticado { get; set; }

        #endregion Protected Properties

        #region Public Properties

        public readonly IUser AppUser;

        #endregion Public Properties

        #region Protected Methods

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
                return Ok(new
                {
                    success = true,
                    data = result
                });

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes()
                                     .Select(notificacao => notificacao.Mensagem)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
                NotificarErroModelInvalida(modelState);

            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            IEnumerable<ModelError> _erros = modelState.Values.SelectMany(modelStateEntry => modelStateEntry.Errors);

            foreach (ModelError erro in _erros)
            {
                string _mensagemErro = erro.Exception is null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(_mensagemErro);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

        #endregion Protected Methods
    }
}
