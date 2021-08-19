namespace Estacionamento.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly ILogger<MesaController> _logger;  
        private readonly DadosContext _context ;
        const int itensPorPagina = 5; 

        public MesaController(ILogger<MesaController> logger, DadosContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index(int? pagina)
        {
            
            var idRestaurante = 1;
            var localizacao = HttpContext.Session.GetString("TextoPesquisa");          
            int numeroPagina = (pagina ?? 1);
            

            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_localizacao", localizacao),
                new MySqlParameter("_idRestaurante", idRestaurante)
                
            };
            List<Mesa> mesas = _context.RetornarLista<Mesa>("sp_consultarMesa", parametros);
            
            ViewBagRestaurantes();
            return View(mesas.ToPagedList(numeroPagina, itensPorPagina));
        }

        public IActionResult Detalhe(int id)
        {
            Models.Mesa mesa = new Models.Mesa();
            if (id > 0)  {
                MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
                mesa = _context.ListarObjeto<Mesa>("sp_buscarMesaPorId", parametros); 
            }

            ViewBagRestaurantes();
            return View(mesa);
        }



        [HttpPost]
        public IActionResult Detalhe(Models.Mesa mesa){
            if(string.IsNullOrEmpty(mesa.Localizacao)){
                ModelState.AddModelError("", "A localização da mesa deve ser preenchida");
            }
            

            if(ModelState.IsValid){
           
                List<MySqlParameter> parametros = new List<MySqlParameter>(){
                    
                    new MySqlParameter("localizacao", mesa.Localizacao),
                    new MySqlParameter("idrestaurante", mesa.IdRestaurante),
                    new MySqlParameter("numerodamesa", mesa.NumeroDaMesa)                  
                };
                if (mesa.Id > 0){
                    parametros.Add(new MySqlParameter("identificacao", mesa.Id));
                }
                var retorno = _context.ListarObjeto<RetornoProcedure>(mesa.Id > 0? "sp_atualizarMesa" : "sp_inserirMesa", parametros.ToArray());
            
                if (retorno.Mensagem == "Ok"){
                    return RedirectToAction("Index");
                } else {
                    ModelState.AddModelError("", retorno.Mensagem);
                    
                }
            }

            ViewBagRestaurantes();
            return View(mesa);
        }

        public JsonResult Excluir(int id){
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("identificacao", id)
            };
            var retorno = _context.ListarObjeto<RetornoProcedure>("sp_excluirMesa", parametros);
            return new JsonResult(new {Sucesso = retorno.Mensagem == "Excluído", Mensagem = retorno.Mensagem });
        }

        public PartialViewResult ListaPartialView(string localizacao, int idrestaurante){
            
            MySqlParameter[] parametros = new MySqlParameter[]{
                new MySqlParameter("_localizacao", localizacao),
                new MySqlParameter("_idRestaurante", idrestaurante)
                
            };
            List<Mesa> mesas = _context.RetornarLista<Mesa>("sp_consultarMesa", parametros);
            if (string.IsNullOrEmpty(localizacao)){
                HttpContext.Session.Remove("TextoPesquisa");
            } else {
            HttpContext.Session.SetString("TextoPesquisa", localizacao);
            }
            
            HttpContext.Session.SetInt32("IdRestaurante", idrestaurante);
            
            return PartialView(mesas.ToPagedList(1, itensPorPagina));
        }

        private void ViewBagRestaurantes(){
            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("_nome", "")
            };
            List<Models.Restaurante> restaurantes = new List<Models.Restaurante>(); 
            restaurantes = _context.RetornarLista<Models.Restaurante>("sp_consultarRestaurante", param);
            
            ViewBag.Restaurantes = restaurantes.Select(c => new SelectListItem(){
                Text= c.Nome, Value= c.Id.ToString()
            }).ToList();
        }
    }
}