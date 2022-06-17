using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(MeuDbContext context) : base(context)
        {

        }

        public async Task<Fornecedor> ObterFornecedorEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                                        .Include(fornecedor => fornecedor.Endereco)
                                        .FirstOrDefaultAsync(fornecedor => fornecedor.Id == id);
        }

        public async Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                                        .Include(fornecedor => fornecedor.Produtos)
                                        .Include(fornecedor => fornecedor.Endereco)
                                        .FirstOrDefaultAsync(fornecedor => fornecedor.Id == id);
        }
    }
}