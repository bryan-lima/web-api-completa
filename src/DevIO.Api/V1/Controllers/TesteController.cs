using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador, 
                               IUser appUser) : base(notificador, appUser)
        {

        }

        [HttpGet]
        public string Valor()
        {
            return "Sou a V1";
        }
    }
}
