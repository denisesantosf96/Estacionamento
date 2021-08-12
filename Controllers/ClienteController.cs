using System.Collections.Generic;
using Estacionamento.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using X.PagedList;

namespace Estacionamento.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public ClienteController(ILogger<ClienteController> logger, DadosContext context)
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
            List<Cliente> clientes = _context.RetornarLista<Cliente>("sp_consultarCliente", parametros);
            
            return View(clientes.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Cliente cliente = new Models.Cliente();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                cliente = _context.ListarObjeto<Cliente>("sp_buscarClientePorId", parametros); 
            }
                   
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Cliente cliente){
            if(string.IsNullOrEmpty(cliente.Nome)){
                ModelState.AddModelError("", "O nome não pode ser vazio");
            }
            if(string.IsNullOrEmpty(cliente.CPF)){
                ModelState.AddModelError("", "O CPF deve ser preenchido");
            }
            if(string.IsNullOrEmpty(cliente.Genero)){
                ModelState.AddModelError("", "O Gênero não pode ser deixado em branco, por favor selecione uma opção");
            }
            
            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    new MySqlParameter("Nome", cliente.Nome),
                    new MySqlParameter("CPF", cliente.CPF),
                    new MySqlParameter("RG", cliente.RG),
                    new MySqlParameter("Telefone", cliente.Telefone),
                    new MySqlParameter("Logradouro", cliente.Logradouro),
                    new MySqlParameter("Numero", cliente.Numero),
                    new MySqlParameter("Bairro", cliente.Bairro),
                    new MySqlParameter("Cidade", cliente.Cidade),
                    new MySqlParameter("Estado", cliente.Estado),
                    new MySqlParameter("Pais", cliente.Pais),
                    new MySqlParameter("CEP", cliente.CEP),
                    new MySqlParameter("Genero", cliente.Genero),
                    new MySqlParameter("DataNascimento", cliente.DataNascimento)

                };
                if (cliente.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", cliente.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(cliente.Id > 0? "sp_atualizarCliente" : "sp_inserirCliente", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }
            return View(cliente);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirCliente", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string nome){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_nome", nome)
            };
            List<Cliente> clientes = _context.RetornarLista<Cliente>("sp_consultarCliente", parametros);
            if (string.IsNullOrEmpty(nome)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", nome);
            }
            return PartialView(clientes.ToPagedList(1, itensPorPagina));
        }    
    }
}