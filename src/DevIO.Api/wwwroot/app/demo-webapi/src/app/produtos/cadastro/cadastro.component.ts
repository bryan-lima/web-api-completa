import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

import { Observable } from 'rxjs';

import { Produto } from '../models/Produto';
import { Fornecedor } from '../models/Fornecedor';
import { ProdutoService } from '../services/produtoService';

@Component({
  selector: 'app-cadastro',
  templateUrl: './cadastro.component.html'
})
export class CadastroComponent implements OnInit {

  produtoForm: FormGroup;
  produto: Produto;
  errors: any[] = [];
  fornecedores: Fornecedor[];
  imagemForm: any;
  imagemNome: string;
  imageBase64: any;

  constructor(private fb: FormBuilder,
              private router: Router,
              private produtoService: ProdutoService) {

    this.produtoService.obterFornecedores()
      .subscribe(
        fornecedores => this.fornecedores = fornecedores,
        fail => this.errors = fail.error.errors
      );

    this.imagemForm = new FormData();
  }

  ngOnInit(): void {

    this.produtoForm = this.fb.group({
      fornecedorId: '',
      nome: '',
      descricao: '',
      imagemUpload: '',
      imagem: '',
      valor: '0',
      ativo: new FormControl(false),
      nomeFornecedor: ''
    });
  }

  cadastrarProduto() {
    if (this.produtoForm.valid && this.produtoForm.dirty) {

      let produtoForm = Object.assign({}, this.produto, this.produtoForm.value);
      produtoForm.ativo = this.produtoForm.get('ativo').value

      this.produtoHandle(produtoForm)
        .subscribe(
          result => { this.onSaveComplete(result) },
          fail => { this.onError(fail) }
        );
    }
  }

  onSaveComplete(response: any) {
    this.router.navigate(['/lista-produtos']);
  }

  onError(fail: any) {
    this.errors = fail.error.errors;
  }

  produtoHandleAlternativo(produto: Produto): Observable<Produto> {

    let formdata = new FormData();
    produto.imagem = this.imagemNome;
    produto.imagemUpload = null;

    formdata.append('produto', JSON.stringify(produto));
    formdata.append('ImagemUpload', this.imagemForm, this.imagemNome);

    return this.produtoService.registrarProdutoAlternativo(formdata);
  }

  produtoHandle(produto: Produto): Observable<Produto> {

    produto.imagemUpload = this.imageBase64;
    produto.imagem = this.imagemNome;

    return this.produtoService.registrarProduto(produto);
  }

  upload(file: any) {
    // necessario para upload via IformFile
    this.imagemForm = file[0];
    this.imagemNome = file[0].name;

    // necessario para upload via base64
    var reader = new FileReader();
    reader.onload = this.manipularReader.bind(this);
    reader.readAsBinaryString(file[0]);
  }

  manipularReader(readerEvt: any) {
    var binaryString = readerEvt.target.result;
    this.imageBase64 = btoa(binaryString);
  } 
}


