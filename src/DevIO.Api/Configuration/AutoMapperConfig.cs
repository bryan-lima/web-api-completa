﻿using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>()
                .ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>()
                .ReverseMap();
            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(produtoViewModel => produtoViewModel.NomeFornecedor, 
                           configuracao => configuracao.MapFrom(produto => produto.Fornecedor.Nome));

            CreateMap<ProdutoViewModel, Produto>();
            CreateMap<ProdutoImagemViewModel, Produto>()
                .ReverseMap();
        }
    }
}
