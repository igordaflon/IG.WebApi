using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository fornecedorRepository;
        private readonly IFornecedorService fornecedorService;
        private readonly IMapper mapper;
        private readonly IEnderecoRepository enderecoRepository;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, IFornecedorService fornecedorService, IMapper mapper, INotificador notificador, IEnderecoRepository enderecoRepository) : base(notificador)
        {
            this.fornecedorRepository = fornecedorRepository;
            this.fornecedorService = fornecedorService;
            this.mapper = mapper;
            this.enderecoRepository = enderecoRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> GetAll()
        {
            var fornecedor = mapper.Map<IEnumerable<FornecedorViewModel>>(await fornecedorRepository.ObterTodos());
            return fornecedor;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> GetById(Guid id)
        {
            var fornecedor = mapper.Map<FornecedorViewModel>(await fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (fornecedor == null) return NotFound();

            return fornecedor;
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Post(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await fornecedorService.Adicionar(mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpPut("id:guid")]
        public async Task<ActionResult<FornecedorViewModel>> Put(Guid id, FornecedorViewModel fornecedorViewModel)
        {

            if (id != fornecedorViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(fornecedorViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await fornecedorService.Atualizar(mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpDelete("id:guid")]
        public async Task<ActionResult<FornecedorViewModel>> Delete(Guid id)
        {
            var fornecedorViewModel = mapper.Map<FornecedorViewModel>(await fornecedorRepository.ObterFornecedorEndereco(id));

            if (fornecedorViewModel == null) return NotFound();

            await fornecedorService.Remover(id);

            return CustomResponse(fornecedorViewModel);
        }

        [HttpGet("get-address/{id:guid}")]
        public async Task<EnderecoViewModel> GetAddressById(Guid Id)
        {
            return mapper.Map<EnderecoViewModel>(await enderecoRepository.ObterPorId(Id));
        }

        [HttpPut("update-address/{id:guid}")]
        public async Task<IActionResult> UpdateAddress(Guid Id, EnderecoViewModel enderecoViewModel)
        {
            if (Id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(enderecoViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await fornecedorService.AtualizarEndereco(mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
        }

    }
}
