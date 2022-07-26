using DevIO.Api.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevIO.Api.Extensions
{
    // Binder personalizado para envio de IFormFile e ViewModel dentro de um FormData compatível com .NET Core 3.1 ou superior (system.text.json)
    public class JsonWithFilesFormDataModelBinder : IModelBinder
    {
        #region Public Methods

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            JsonSerializerOptions _serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };

            ProdutoImagemViewModel _produtoImagemViewModel = JsonSerializer.Deserialize<ProdutoImagemViewModel>(bindingContext.ValueProvider.GetValue("produto").FirstOrDefault(), _serializeOptions);
            _produtoImagemViewModel.ImagemUpload = bindingContext.ActionContext.HttpContext.Request.Form.Files.FirstOrDefault();

            bindingContext.Result = ModelBindingResult.Success(_produtoImagemViewModel);
            return Task.CompletedTask;
        }

        #endregion Public Methods
    }
}