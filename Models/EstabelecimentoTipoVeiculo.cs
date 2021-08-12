namespace Estacionamento.Models
{
    public class EstabelecimentoTipoVeiculo
    {
        public int Id { get; set; }
        public int IdEstabelecimento { get; set; }
        public int IdTipoVeiculo { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
    }
}