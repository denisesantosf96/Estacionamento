using System;
using System.Collections.Generic;
using System.Linq;
using Estacionamento.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using X.PagedList;

namespace Estacionamento.Controllers
{
    public class EstacionamentoController : Controller
    {
        private readonly ILogger<EstacionamentoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public EstacionamentoController(ILogger<EstacionamentoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {   
            var idEstabelecimento = 1;
            var situacao = HttpContext.Session.GetString("situacao");          
            int numeroPagina = (pagina ?? 1); 

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_IdEstabelecimento", idEstabelecimento),
                new MySqlParameter("_situacao", situacao)
            };
            List<Estacionamento.Models.Estacionamento> estacionamentos = _context.RetornarLista<Estacionamento.Models.Estacionamento>("sp_consultarEstacionamento", parametros);
            
            ViewBagEstabelecimentos();
            return View(estacionamentos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id, int idestabelecimento)
        {
            Models.Estacionamento estacionamento = new Models.Estacionamento();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                estacionamento = _context.ListarObjeto<Models.Estacionamento>("sp_buscarEstacionamentoPorId", parametros); 
            } else {
                estacionamento.IdEstabelecimento = idestabelecimento;
            }

            ViewBagVagas(id > 0 ? estacionamento.IdEstabelecimento : idestabelecimento);
            ViewBagManobristas();
            ViewBagTiposVeiculos();
            ViewBagEstabelecimentos();
            return View(estacionamento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Estacionamento estacionamento){
            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("IdVeiculo", estacionamento.IdVeiculo),
                    new MySqlParameter("IdVaga", estacionamento.IdVaga),
                    new MySqlParameter("IdManobrista", estacionamento.IdManobrista),
                    new MySqlParameter("IdCliente", estacionamento.IdCliente),
                    new MySqlParameter("Data", estacionamento.Data),
                    new MySqlParameter("Situacao", estacionamento.Situacao),
                    new MySqlParameter("FormaPagamento", estacionamento.FormaPagamento),
                    new MySqlParameter("ValorTotal", estacionamento.ValorTotal),
                    new MySqlParameter("DataPagamento", estacionamento.DataPagamento)

                };
                if (estacionamento.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", estacionamento.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(estacionamento.Id > 0? "sp_atualizarEstacionamento" : "sp_inserirEstacionamento", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            ViewBagVagas(estacionamento.IdEstabelecimento);
            ViewBagManobristas();
            ViewBagTiposVeiculos();
            ViewBagEstabelecimentos();
            return View(estacionamento);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirEstacionamento", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string situacao, int idestabelecimento){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_IdEstabelecimento", idestabelecimento),
                new MySqlParameter("_situacao", situacao)
            };
            List<Models.Estacionamento> estacionamentos = _context.RetornarLista<Models.Estacionamento>("sp_consultarEstacionamento", parametros);
            if (string.IsNullOrEmpty(situacao)){
                HttpContext.Session.Remove("situacao");
            } else {
            HttpContext.Session.SetString("situacao", situacao);
            }

            HttpContext.Session.SetInt32("IdEstabelecimento", idestabelecimento);

            return PartialView(estacionamentos.ToPagedList(1, itensPorPagina));
        }

        private void ViewBagEstabelecimentos(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Estabelecimento> estabelecimentos = new List<Models.Estabelecimento>(); 
            estabelecimentos = _context.RetornarLista<Models.Estabelecimento>("sp_consultarEstabelecimento", param);
            
            ViewBag.Estabelecimentos = estabelecimentos.Select(c => new SelectListItem(){
                Text= c.Nome, Value= c.Id.ToString()
            }).ToList();
        }
        private void ViewBagTiposVeiculos(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_tipo", "")
            };
            List<Models.TipoVeiculo> tiposveiculos = new List<Models.TipoVeiculo>(); 
            tiposveiculos = _context.RetornarLista<Models.TipoVeiculo>("sp_consultarTipoVeiculo", param);
            
            ViewBag.TiposVeiculos = tiposveiculos.Select(c => new SelectListItem(){
                Text= c.Tipo, Value= c.Id.ToString()
            }).ToList();
        }

        private void ViewBagVagas(int idestabelecimento){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_IdEstabelecimento", idestabelecimento)
            };
            List<Models.Vaga> vagas = new List<Models.Vaga>(); 
            vagas = _context.RetornarLista<Models.Vaga>("sp_consultarVaga", param);
            
            ViewBag.Vagas = vagas.Select(c => new SelectListItem(){
                Text= c.Id +" - "+ c.Localizacao, Value= c.Id.ToString()
            }).ToList();
        }

        private void ViewBagManobristas(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Manobrista> manobristas = new List<Models.Manobrista>(); 
            manobristas = _context.RetornarLista<Models.Manobrista>("sp_consultarManobrista", param);
            
            ViewBag.Manobristas = manobristas.Select(c => new SelectListItem(){
                Text= c.Nome, Value= c.Id.ToString()
            }).ToList();
        } 
                
    }
}