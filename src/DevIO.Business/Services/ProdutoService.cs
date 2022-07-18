using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Services
{
    public class ProdutoService : BaseService, IProdutoService
    {
        #region Private Fields

        private readonly IProdutoRepository _produtoRepository;
        private readonly IUser _user;

        #endregion Private Fields

        #region Public Constructors

        public ProdutoService(IProdutoRepository produtoRepository,
                              INotificador notificador,
                              IUser user) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _user = user;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task Adicionar(Produto produto)
        {
            if (!ExecutarValidacao(validacao: new ProdutoValidation(),
                                   entidade: produto))
                return;

            //var user = _user.GetUserId();

            await _produtoRepository.Adicionar(produto);
        }

        public async Task Atualizar(Produto produto)
        {
            if (!ExecutarValidacao(validacao: new ProdutoValidation(),
                                   entidade: produto))
                return;

            await _produtoRepository.Atualizar(produto);
        }

        public async Task Remover(Guid id)
        {
            await _produtoRepository.Remover(id);
        }

        public void Dispose()
        {
            _produtoRepository?.Dispose();
        }

        #endregion Public Methods
    }
}