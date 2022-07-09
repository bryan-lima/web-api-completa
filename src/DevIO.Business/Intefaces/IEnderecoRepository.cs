using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Intefaces
{
    public interface IEnderecoRepository : IRepository<Endereco>
    {
        #region Public Methods

        Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId);

        #endregion Public Methods
    }
}