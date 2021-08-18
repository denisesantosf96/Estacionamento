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
    public class TipoVeiculoController : Controller
    {
        private readonly ILogger<TipoVeiculoController> _logger;
        private readonly DadosContext _context ;
        const int itensPorPagina = 5;
  
        public TipoVeiculoController(ILogger<TipoVeiculoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pagina)
        {
            var tipo = HttpContext.Session.GetString("TextoPesquisa");
            int numeroPagina = (pagina ?? 1);

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_tipo", tipo)

            };
            List<TipoVeiculo> tiposveiculos = _context.RetornarLista<TipoVeiculo>("sp_consultarTipoVeiculo", parametros);
            
            return View(tiposveiculos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.TipoVeiculo tiposveiculos = new Models.TipoVeiculo();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                tiposveiculos = _context.ListarObjeto<Models.TipoVeiculo>("sp_buscarTipoVeiculoPorId", parametros); 
            }
       
            return View(tiposveiculos);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.TipoVeiculo tipoveiculo){
            if(string.IsNullOrEmpty(tipoveiculo.Tipo)){
                ModelState.AddModelError("", "A categoria deve ser selecionada");
            } 
            
            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("Tipo", tipoveiculo.Tipo)
                    
                };
                if (tipoveiculo.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", tipoveiculo.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(tipoveiculo.Id > 0? "sp_atualizarTipoVeiculo" : "sp_inserirTipoVeiculo", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            return View(tipoveiculo);
        }

        public JsonResult Excluir(int id){
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirTipoVeiculo", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string tipo){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_tipo", tipo)
            };
            List<TipoVeiculo> tiposveiculos = _context.RetornarLista<TipoVeiculo>("sp_consultarTipoVeiculo", parametros);
            if (string.IsNullOrEmpty(tipo)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", tipo);
            }

            return PartialView(tiposveiculos.ToPagedList(1, itensPorPagina));
        }
    }
}