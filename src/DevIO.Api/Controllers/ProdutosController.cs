using AutoMapper;
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
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        public readonly IProdutoRepository produtoRepository;
        public readonly IProdutoService produtoService;
        public readonly IMapper mapper;

        public ProdutosController(INotificador notificador, IProdutoRepository produtoRepository, IProdutoService produtoService, IMapper mapper) : base(notificador)
        {
            this.produtoRepository = produtoRepository;
            this.produtoService = produtoService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> GetAll()
        {
            return mapper.Map<IEnumerable<ProdutoViewModel>>(await produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> GetById(Guid Id)
        {
            var produtoViewModel = await ObterProduto(Id);

            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }


        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Post(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;

            if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = imagemNome;
            await produtoService.Adicionar(mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }


            [HttpDelete("id:guid")]
        public async Task<ActionResult<ProdutoViewModel>> Remove(Guid Id)
        {
            var produto = await ObterProduto(Id);

            if(produto == null) return NotFound();

            await produtoService.Remover(Id);

            return CustomResponse(produto);
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            var imageDataByteArray = Convert.FromBase64String(arquivo);

            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para esse produto");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com esse nome");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return mapper.Map<ProdutoViewModel>(await produtoRepository.ObterProdutoFornecedor(id));
        }
    }
}
