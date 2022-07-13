using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevIO.Data.Mappings
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(produto => produto.Id);

            builder.Property(produto => produto.Nome)
                   .IsRequired()
                   .HasColumnType("varchar(200)");

            builder.Property(produto => produto.Descricao)
                   .IsRequired()
                   .HasColumnType("varchar(1000)");

            builder.Property(produto => produto.Imagem)
                   .IsRequired()
                   .HasColumnType("varchar(100)");

            builder.ToTable("Produtos");
        }
    }
}