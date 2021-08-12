using System;

namespace Estacionamento.Models
{
    public class Manobrista
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string Telefone { get; set; }
        public string Logradouro { get; set; }
        public decimal Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CEP { get; set; }
        public string Genero { get; set; }  
        public DateTime DataNascimento { get; set; }
        public DateTime DataAdmissao { get; set; }
    }
}