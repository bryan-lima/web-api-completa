import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";

import { Observable } from 'rxjs';
import { catchError, map } from "rxjs/operators";
import { Fornecedor } from '../models/Fornecedor';
import { Produto } from '../models/Produto';
import { BaseService } from 'src/app/base/baseService';

@Injectable()
export class ProdutoService extends BaseService {
    constructor(private http: HttpClient) { super() }

    obterTodos(): Observable<Produto[]> {
        return this.http
            .get<Produto[]>(this.UrlServiceV1 + "produtos", super.ObterAuthHeaderJson())
            .pipe(
                catchError(this.serviceError));
    }

    registrarProdutoAlternativo(produto: FormData): Observable<Produto> {

        return this.http
            .post(this.UrlServiceV1 + 'produtos/adicionar', produto, super.ObterHeaderFormData())
            .pipe(
                map(super.extractData),
                catchError(super.serviceError)
            );
    }

    registrarProduto(produto: Produto): Observable<Produto> {

        return this.http
            .post(this.UrlServiceV1 + 'produtos', produto, super.ObterAuthHeaderJson())
            .pipe(
                map(super.extractData),
                catchError(super.serviceError)
            );
    }

    obterFornecedores(): Observable<Fornecedor[]> {
        return this.http
            .get<Fornecedor[]>(this.UrlServiceV1 + 'fornecedores', super.ObterAuthHeaderJson())
            .pipe(
                catchError(super.serviceError)
            );
    }
}