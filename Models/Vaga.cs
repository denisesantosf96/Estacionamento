namespace Estacionamento.Models
{
    public class Vaga
    {
        public int Id { get; set; }
        public int IdEstabelecimento { get; set; }
        public string Localizacao { get; set; }
        public string Status { get; set; }
    }
}