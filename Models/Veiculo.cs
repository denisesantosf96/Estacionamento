namespace Estacionamento.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public int IdTipoVeiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public string Cor { get; set; }
        public int IdEstabelecimento { get; set; }
        public string Nome { get; set; }
    }
}