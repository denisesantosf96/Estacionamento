namespace Estacionamento.Models
{
    public class Estabelecimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Logradouro { get; set; }
        public decimal Numero { get; set; }
        public string Bairro { get; set; }  
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public string HorarioFuncionamento { get; set; }
        public int IdTipoVeiculo { get; set; }
    }
}