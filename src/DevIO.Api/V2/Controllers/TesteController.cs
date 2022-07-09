using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        #region Private Fields

        private readonly ILogger _logger;

        #endregion Private Fields

        #region Public Constructors

        public TesteController(ILogger<TesteController> logger,
                               INotificador notificador,
                               IUser appUser) : base(notificador, appUser)
        {
            _logger = logger;
        }

        #endregion Public Constructors

        #region GET

        [HttpGet]
        public string Valor()
        {
            throw new Exception("Erro");

            //try
            //{
            //    var i = 0;
            //    var result = 42 / i;
            //}
            //catch(DivideByZeroException ex)
            //{
            //    ex.Ship(HttpContext);
            //}

            _logger.LogTrace("Log de Trace");
            _logger.LogDebug("Log de Debug");
            _logger.LogInformation("Log de Informação");
            _logger.LogWarning("Log de Aviso");
            _logger.LogError("Log de Erro");
            _logger.LogCritical("Log de Problema Crítico");

            return "Sou a V2";
        }

        #endregion GET
    }
}
