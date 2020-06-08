using System;

namespace SME.SR.Infra.Dtos.Relatorios.ConselhoClasse
{
    public class ConselhoClasseCabecalhoDto
    {
        public ConselhoClasseCabecalhoDto(string nomeDre, string nomeUe, DateTime data)
        {
            NomeDre = nomeDre;
            NomeUe = nomeUe;
            Data = data;
        }

        public string NomeDre { get; set; }
        public string NomeUe { get; set; }
        public DateTime Data { get; set; }
    }
}
