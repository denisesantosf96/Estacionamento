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
                estacionamento.Data = DateTime.Now;
            }

            ViewBagVagas(id > 0 ? estacionamento.IdEstabelecimento : idestabelecimento);
            ViewBagManobristas();
            ViewBagTiposVeiculos();
            ViewBagEstabelecimentos();
            return View(estacionamento);
        }

        [HttpPost]
        public IActionResult Detalhe(Models.Estacionamento estacionamento){
        
            if(string.IsNullOrEmpty(estacionamento.Localizacao)){
                ModelState.AddModelError("", "Vaga indisponível");
            }

            if(string.IsNullOrEmpty(estacionamento.Placa)){
                ModelState.AddModelError("", "A Placa deve ser preenchida");
            }

            if(string.IsNullOrEmpty(estacionamento.Marca)){
                ModelState.AddModelError("", "A Marca deve ser preenchida");
            }
            if(string.IsNullOrEmpty(estacionamento.Modelo)){
                ModelState.AddModelError("", "O Modelo deve ser preenchido");
            }
            if(string.IsNullOrEmpty(estacionamento.Cor)){
                ModelState.AddModelError("", "A Cor deve ser preenchida");
            }

            if(estacionamento.IdTipoVeiculo == 0){
                ModelState.AddModelError("", "O Tipo de Veículo deve ser selecionado");
            }
            
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
                    new MySqlParameter("Nome", estacionamento.Nome),
                    new MySqlParameter("CPF", estacionamento.CPF),
                    new MySqlParameter("_Status", estacionamento.Status),
                    new MySqlParameter("_Localizacao", estacionamento.Localizacao),
                    new MySqlParameter("_IdEstabelecimento", estacionamento.IdEstabelecimento)
                };
                if (estacionamento.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", estacionamento.Id));
                }
                else {
                    parametros.Add(new MySqlParameter("Marca", estacionamento.Marca));
                    parametros.Add(new MySqlParameter("Modelo", estacionamento.Modelo));
                    parametros.Add(new MySqlParameter("Placa", estacionamento.Placa));
                    parametros.Add(new MySqlParameter("Cor", estacionamento.Cor));
                    parametros.Add(new MySqlParameter("IdTipoVeiculo", estacionamento.IdTipoVeiculo));
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

        public JsonResult TrazerNomeCliente(string cpf){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("cpfCliente", cpf)
            };

            Models.Cliente cliente = _context.ListarObjeto<Models.Cliente>("sp_consultarClienteCPF", parametros);

            return new JsonResult(new {cliente});
        }

        public JsonResult TrazerVeiculo(string placa){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_placa", placa)
            };

            Models.Veiculo veiculo = _context.ListarObjeto<Models.Veiculo>("sp_consultarVeiculoPlaca", parametros);

            return new JsonResult(new {veiculo});
        }

        public JsonResult TrazerValorTipoVeiculo(int idtipoveiculo, int idestabelecimento){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_idTipoVeiculo", idtipoveiculo),
                new MySqlParameter("_idEstabelecimento", idestabelecimento)
            };

            Models.EstabelecimentoTipoVeiculo estabelecimentoTipoVeiculo = _context.ListarObjeto<Models.EstabelecimentoTipoVeiculo>("sp_consultarValorTipoVeiculo", parametros);

            return new JsonResult(new {estabelecimentoTipoVeiculo});
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
            tiposveiculos.Insert(0,new Models.TipoVeiculo(){ Tipo = "", Id = 0 });
            
            ViewBag.TiposVeiculos = tiposveiculos.Select(c => new SelectListItem(){
                Text= c.Tipo, Value= c.Id.ToString()
            }).ToList();
        }

        private void ViewBagVagas(int idestabelecimento){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_IdEstabelecimento", idestabelecimento),
                new MySqlParameter("_status", "")
            };
            List<Models.Vaga> vagas = new List<Models.Vaga>(); 
            vagas = _context.RetornarLista<Models.Vaga>("sp_consultarVaga", param);
            
            ViewBag.Vagas = vagas.Select(c => new SelectListItem(){
                Text= c.Id +" - "+ c.Localizacao +" - "+ c.Status, Value= c.Id.ToString()
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