using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/produtos")]
    public class ProdutosController : MainController
    {
        #region Private Fields

        private readonly IMapper _mapper;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;

        #endregion Private Fields

        #region Public Constructors

        public ProdutosController(IMapper mapper,
                                  INotificador notificador,
                                  IProdutoRepository produtoRepository,
                                  IProdutoService produtoService,
                                  IUser user) : base(notificador, user)
        {
            _mapper = mapper;
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
        }

        #endregion Public Constructors

        #region GET

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            IEnumerable<ProdutoViewModel> _produtosViewModel = _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());

            foreach (ProdutoViewModel produtoViewModel in _produtosViewModel)
                produtoViewModel.ImagemUpload = ObterArquivo(produtoViewModel.Imagem);

            return _produtosViewModel;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            ProdutoViewModel _produtoViewModel = await ObterProduto(id);

            if (_produtoViewModel is null)
                return NotFound();

            return _produtoViewModel;
        }

        #endregion GET

        #region POST

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            string _imagemNome = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";

            if (!UploadArquivo(produtoViewModel.ImagemUpload, _imagemNome))
                return CustomResponse(produtoViewModel);

            produtoViewModel.Imagem = _imagemNome;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost("adicionar")]
        public async Task<ActionResult<ProdutoImagemViewModel>> AdicionarAlternativo(
            // Binder personalizado para envio de IFormFile e ViewModel dentro de um FormData compatível com .NET Core 3.1 ou superior (system.text.json)
            [ModelBinder(BinderType = typeof(JsonWithFilesFormDataModelBinder))]
            ProdutoImagemViewModel produtoViewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            string _prefixo = $"{Guid.NewGuid()}_";

            if (!await UploadArquivoAlternativo(produtoViewModel.ImagemUpload, _prefixo))
                return CustomResponse(ModelState);

            produtoViewModel.Imagem = _prefixo + produtoViewModel.ImagemUpload.FileName;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        //[DisableRequestSizeLimit]
        [RequestSizeLimit(40000000)] // Limitando upload em 40 MB
        [HttpPost("imagem")]
        public ActionResult AdicionarImagem(IFormFile file)
        {
            return Ok(file);
        }

        #endregion POST

        #region PUT

        [ClaimsAuthorize("Produto", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
            {
                NotificarErro("Os IDs informados são diferentes");
                return CustomResponse();
            }

            ProdutoViewModel _produtoAtualizacao = await ObterProduto(id);

            produtoViewModel.Imagem = _produtoAtualizacao.Imagem;

            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            if (produtoViewModel.ImagemUpload != null)
            {
                string _imagemNome = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";

                if (!UploadArquivo(produtoViewModel.ImagemUpload, _imagemNome))
                    return CustomResponse(ModelState);

                _produtoAtualizacao.Imagem = _imagemNome;
            }

            _produtoAtualizacao.Nome = produtoViewModel.Nome;
            _produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            _produtoAtualizacao.Valor = produtoViewModel.Valor;
            _produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(_produtoAtualizacao));

            return CustomResponse(produtoViewModel);
        }

        #endregion PUT

        #region DELETE

        [ClaimsAuthorize("Produto", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            ProdutoViewModel _produto = await ObterProduto(id);

            if (_produto is null)
                return NotFound();

            ExcluirArquivo(_produto.Imagem);

            await _produtoService.Remover(id);

            return CustomResponse(_produto);
        }

        #endregion DELETE

        #region Private Methods

        private bool UploadArquivo(string arquivo, string imagemNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            byte[] _imageDataByteArray = Convert.FromBase64String(arquivo);

            string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imagemNome);

            if (System.IO.File.Exists(_filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(_filePath, _imageDataByteArray);

            return true;
        }

        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string prefixo)
        {
            if (arquivo is null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            string _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", prefixo + arquivo.FileName);

            if (System.IO.File.Exists(_path))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            using (FileStream stream = new FileStream(_path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }

        private string ObterArquivo(string imagemNome)
        {
            if (imagemNome is null)
                return null;

            string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imagemNome);

            if (System.IO.File.Exists(_filePath))
                return $"data:image/{imagemNome.Substring(imagemNome.Length - 3)};base64,{Convert.ToBase64String(System.IO.File.ReadAllBytes(_filePath))}";

            return null;
        }

        private bool ExcluirArquivo(string imagemNome)
        {
            if (imagemNome is null)
                return false;

            string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imagemNome);

            if (System.IO.File.Exists(_filePath))
            {
                System.IO.File.Delete(_filePath);
                return true;
            }

            return false;
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            ProdutoViewModel _produtoViewModel = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));

            _produtoViewModel.ImagemUpload = ObterArquivo(_produtoViewModel.Imagem);

            return _produtoViewModel;
        }

        #endregion Private Methods
    }
}
