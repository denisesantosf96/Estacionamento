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
    public class VagaController : Controller
    {
        private readonly ILogger<VagaController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public VagaController(ILogger<VagaController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            
            var idEstabelecimento = 1;
            var status = HttpContext.Session.GetString("TextoPesquisa");          
            int numeroPagina = (pagina ?? 1);
            

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_IdEstabelecimento", idEstabelecimento),
                new MySqlParameter("_status", status)
            };
            List<Vaga> vagas = _context.RetornarLista<Vaga>("sp_consultarVaga", parametros);
            
            ViewBagEstabelecimentos();
            return View(vagas.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Vaga vaga = new Models.Vaga();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                vaga = _context.ListarObjeto<Vaga>("sp_buscarVagaPorId", parametros); 
            }

            ViewBagEstabelecimentos();
            return View(vaga);
        }



        [HttpPost]
        public IActionResult Detalhe(Models.Vaga vaga){
            

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    
                    new MySqlParameter("IdEstabelecimento", vaga.IdEstabelecimento),
                    new MySqlParameter("Localizacao", vaga.Localizacao),
                    new MySqlParameter("Status", vaga.Status)                  
                };
                if (vaga.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", vaga.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(vaga.Id > 0? "sp_atualizarVaga" : "sp_inserirVaga", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            ViewBagEstabelecimentos();
            return View(vaga);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirVaga", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(int idestabelecimento,string status){
            
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_IdEstabelecimento", idestabelecimento),
                new MySqlParameter("_status", status)
                
            };
            List<Vaga> vagas = _context.RetornarLista<Vaga>("sp_consultarVaga", parametros);
            if (string.IsNullOrEmpty(status)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", status);
            }
            
            HttpContext.Session.SetInt32("IdEstabelecimento", idestabelecimento);
            
            return PartialView(vagas.ToPagedList(1, itensPorPagina));
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