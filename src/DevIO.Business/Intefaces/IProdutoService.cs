using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Intefaces
{
    public interface IProdutoService : IDisposable
    {
        #region Public Methods

        Task Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(Guid id);

        #endregion Public Methods
    }
}