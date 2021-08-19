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
    public class VeiculoController : Controller
    {
        private readonly ILogger<VeiculoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public VeiculoController(ILogger<VeiculoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            
            var idTipoVeiculo = 1;         
            int numeroPagina = (pagina ?? 1);
            

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_IdTipoVeiculo", idTipoVeiculo)
                
            };
            List<Veiculo> veiculos = _context.RetornarLista<Veiculo>("sp_consultarVeiculo", parametros);
            
            ViewBagEstabelecimentos();
            return View(veiculos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Veiculo veiculo = new Models.Veiculo();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                veiculo = _context.ListarObjeto<Veiculo>("sp_buscarVeiculoPorId", parametros); 
            }

            ViewBagEstabelecimentos();
            ViewBagTiposVeiculos();
            return View(veiculo);
        }



        [HttpPost]
        public IActionResult Detalhe(Models.Veiculo veiculo){
            
            

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    
                    new MySqlParameter("IdTipoVeiculo", veiculo.IdTipoVeiculo),
                    new MySqlParameter("Marca", veiculo.Marca),
                    new MySqlParameter("Modelo", veiculo.Modelo),
                    new MySqlParameter("Placa", veiculo.Placa) ,
                    new MySqlParameter("Cor", veiculo.Cor)                
                };
                if (veiculo.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", veiculo.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(veiculo.Id > 0? "sp_atualizarVeiculo" : "sp_inserirVeiculo", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            ViewBagEstabelecimentos();
            ViewBagTiposVeiculos();
            return View(veiculo);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirVeiculo", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idTipoVeiculo){
            
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_IdTipoVeiculo", idTipoVeiculo)
                
            };
            List<Veiculo> veiculos = _context.RetornarLista<Veiculo>("sp_consultarVeiculo", parametros);
            
            
            HttpContext.Session.SetInt32("IdTipoVeiculo", idTipoVeiculo);
            
            return PartialView(veiculos.ToPagedList(1, itensPorPagina));
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