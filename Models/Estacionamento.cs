using System;

namespace Estacionamento.Models
{
    public class Estacionamento
    {
        public int Id { get; set; }
        public int IdVeiculo { get; set; }
        public int IdVaga { get; set; }
        public int IdManobrista { get; set; }
        public int IdCliente { get; set; }
        public DateTime Data { get; set; }
        public string Situacao { get; set; }
        public string FormaPagamento { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataPagamento { get; set; }
        public int IdEstabelecimento { get; set; }
        public string Nome { get; set; }
        public string NomeManobrista { get; set; }
        public string Modelo { get; set; }
        public string  Placa { get; set; }
        public string Status { get; set; }
        public string Localizacao { get; set; }
        public int IdTipoVeiculo { get; set; }
    }
}