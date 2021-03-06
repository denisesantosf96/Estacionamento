using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Estacionamento.Models;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estacionamento.Controllers
{
    public class EstabelecimentoController : Controller
    {
        private readonly ILogger<EstabelecimentoController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5;
  
        public EstabelecimentoController(ILogger<EstabelecimentoController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pagina)
        {
            var nome = HttpContext.Session.GetString("TextoPesquisa");          
            int numeroPagina = (pagina ?? 1);

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Estabelecimento> estabelecimentos = _context.RetornarLista<Models.Estabelecimento>("sp_consultarEstabelecimento", parametros);
            
            ViewBagEstabelecimentos();
            return View(estabelecimentos.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Estabelecimento estabelecimento = new Models.Estabelecimento();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                estabelecimento = _context.ListarObjeto<Models.Estabelecimento>("sp_buscarEstabelecimentoPorId", parametros); 
            }
            
            ViewBagTiposVeiculos();      
            return View(estabelecimento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Estabelecimento estabelecimento){
            if(string.IsNullOrEmpty(estabelecimento.Nome)){
                ModelState.AddModelError("", "O nome n??o pode ser vazio");
            }    
            if(string.IsNullOrEmpty(estabelecimento.Logradouro)){
                ModelState.AddModelError("", "O Logradouro deve ser informado");
            }
            if(string.IsNullOrEmpty(estabelecimento.Bairro)){
                ModelState.AddModelError("", "O Bairro deve ser informado");
            }
            if(string.IsNullOrEmpty(estabelecimento.Cidade)){
                ModelState.AddModelError("", "A Cidade deve ser informado");
            }
            if(string.IsNullOrEmpty(estabelecimento.Estado)){
                ModelState.AddModelError("", "O Estado deve ser informado");
            }
            if(string.IsNullOrEmpty(estabelecimento.Pais)){
                ModelState.AddModelError("", "O Pa??s deve ser informado"); 
            }
            if(string.IsNullOrEmpty(estabelecimento.CEP)){
                ModelState.AddModelError("", "O CEP deve ser informado");
            }
            if(string.IsNullOrEmpty(estabelecimento.Telefone)){
                ModelState.AddModelError("", "O CEP deve ser informado");
            }
            if(string.IsNullOrEmpty(estabelecimento.HorarioFuncionamento)){
                ModelState.AddModelError("", "O Hor??rio de Funcionamento deve ser informado");
            }

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("Nome", estabelecimento.Nome),
                    new MySqlParameter("Logradouro", estabelecimento.Logradouro),
                    new MySqlParameter("Numero", estabelecimento.Numero),
                    new MySqlParameter("Bairro", estabelecimento.Bairro),
                    new MySqlParameter("Cidade", estabelecimento.Cidade),
                    new MySqlParameter("Estado", estabelecimento.Estado),
                    new MySqlParameter("Pais", estabelecimento.Pais),
                    new MySqlParameter("CEP", estabelecimento.CEP),
                    new MySqlParameter("Telefone", estabelecimento.Telefone),
                    new MySqlParameter("HorarioFuncionamento", estabelecimento.HorarioFuncionamento)

                };
                if (estabelecimento.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", estabelecimento.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(estabelecimento.Id > 0? "sp_atualizarEstabelecimento" : "sp_inserirEstabelecimento", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }
            ViewBagTiposVeiculos();
            return View(estabelecimento);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirEstabelecimento", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Exclu??do", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Estabelecimento> estabelecimentos = _context.RetornarLista<Models.Estabelecimento>("sp_consultarEstabelecimento", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }

            return PartialView(estabelecimentos.ToPagedList(1, itensPorPagina));
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

    }
}

