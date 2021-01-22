using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAtribuicaoCjDreDto
    {
        public RelatorioAtribuicaoCjDreDto(string nome, string codigo)
        {
            Ues = new List<RelatorioAtribuicaoCjUeDto>();
            Nome = nome;
            Codigo = codigo;
        }

        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<RelatorioAtribuicaoCjUeDto> Ues { get; set; }
    }
}