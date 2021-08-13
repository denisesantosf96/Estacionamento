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
    public class EstabelecimentoTipoVeiculoController : Controller
    {
        private readonly ILogger<EstabelecimentoTipoVeiculoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public EstabelecimentoTipoVeiculoController(ILogger<EstabelecimentoTipoVeiculoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            
            var idEstabelecimento = 1;           
            int numeroPagina = (pagina ?? 1);
            

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_idEstabelecimento", idEstabelecimento)
                
            };
            List<EstabelecimentoTipoVeiculo> estabelecimentosveiculos = _context.RetornarLista<EstabelecimentoTipoVeiculo>("sp_consultarEstabelecimentoTipoVeiculo", parametros);
            
            ViewBagEstabelecimentos();
            return View(estabelecimentosveiculos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.EstabelecimentoTipoVeiculo estabelecimentoveiculo = new Models.EstabelecimentoTipoVeiculo();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                estabelecimentoveiculo = _context.ListarObjeto<EstabelecimentoTipoVeiculo>("sp_buscarEstabelecimentoTipoVeiculoPorId", parametros); 
            }

            ViewBagEstabelecimentos();
            return View(estabelecimentoveiculo);
        }



        [HttpPost]
        public IActionResult Detalhe(Models.EstabelecimentoTipoVeiculo estabelecimentosveiculos){
            
            

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    
                    new MySqlParameter("IdEstabelecimento", estabelecimentosveiculos.IdEstabelecimento),
                    new MySqlParameter("IdTipoVeiculo", estabelecimentosveiculos.IdTipoVeiculo),
                    new MySqlParameter("Valor", estabelecimentosveiculos.Valor)                 
                };
                if (estabelecimentosveiculos.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", estabelecimentosveiculos.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(estabelecimentosveiculos.Id > 0? "sp_atualizarEstabelecimentoTipoVeiculo" : "sp_inserirEstabelecimentoTipoVeiculo", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            ViewBagEstabelecimentos();
            return View(estabelecimentosveiculos);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirEstabelecimentoTipoVeiculo", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idEstabelecimento){
            
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_idEstabelecimento", idEstabelecimento)
                
            };
            List<EstabelecimentoTipoVeiculo> estabelecimentoveiculos = _context.RetornarLista<EstabelecimentoTipoVeiculo>("sp_consultarEstabelecimentoTipoVeiculo", parametros);
                        
            HttpContext.Session.SetInt32("_idEstabelecimento", idEstabelecimento);
            
            return PartialView(estabelecimentoveiculos.ToPagedList(1, itensPorPagina));
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

    }
}