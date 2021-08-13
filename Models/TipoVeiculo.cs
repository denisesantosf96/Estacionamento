namespace Estacionamento.Models
{
    public class TipoVeiculo
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public int IdEstabelecimento { get; set; }
    }
}