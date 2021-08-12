using System.Collections.Generic;
using Estacionamento.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using X.PagedList;

namespace Estacionamento.Controllers
{
    public class ManobristaController : Controller
    {
      private readonly ILogger<ManobristaController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public ManobristaController(ILogger<ManobristaController> logger, DadosContext context)
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
            List<Manobrista> manobristas = _context.RetornarLista<Manobrista>("sp_consultarManobrista", parametros);
            
            return View(manobristas.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Manobrista manobrista = new Models.Manobrista();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                manobrista = _context.ListarObjeto<Manobrista>("sp_buscarManobristaPorId", parametros); 
            }
                   
            return View(manobrista);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Manobrista manobrista){
            if(string.IsNullOrEmpty(manobrista.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if(string.IsNullOrEmpty(manobrista.CPF)){
                ModelState.AddModelError("", "O CPF deve ser preenchido");
            }
            if(string.IsNullOrEmpty(manobrista.RG)){
                ModelState.AddModelError("", "O RG deve ser preenchido");
            }
            if(string.IsNullOrEmpty(manobrista.Genero)){
                ModelState.AddModelError("", "O Gênero não pode ser deixado em branco. Por favor selecione uma opção");
            }
            
            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("Nome", manobrista.Nome),
                    new MySqlParameter("CPF", manobrista.CPF),
                    new MySqlParameter("RG", manobrista.RG),
                    new MySqlParameter("Telefone", manobrista.Telefone),
                    new MySqlParameter("Logradouro", manobrista.Logradouro),
                    new MySqlParameter("Numero", manobrista.Numero),
                    new MySqlParameter("Bairro", manobrista.Bairro),
                    new MySqlParameter("Cidade", manobrista.Cidade),
                    new MySqlParameter("Estado", manobrista.Estado),
                    new MySqlParameter("Pais", manobrista.Pais),
                    new MySqlParameter("CEP", manobrista.CEP),
                    new MySqlParameter("Genero", manobrista.Genero),
                    new MySqlParameter("DataNascimento", manobrista.DataNascimento),
                    new MySqlParameter("DataAdmissao", manobrista.DataAdmissao)

                };
                if (manobrista.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", manobrista.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(manobrista.Id > 0? "sp_atualizarManobrista" : "sp_inserirManobrista", parametros.ToArray());
            
                if (retorno.Mensagem.ToLower() == "ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                }
            }
            return View(manobrista);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirManobrista", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Manobrista> manobristas = _context.RetornarLista<Manobrista>("sp_consultarManobrista", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }
            return PartialView(manobristas.ToPagedList(1, itensPorPagina));
        }  
    }
}