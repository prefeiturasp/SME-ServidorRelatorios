using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisUeDto
    {
        public RelatorioNotasEConceitosFinaisUeDto(string codigo, string nome)
        {
            Codigo = codigo;
            Nome = nome;
            Anos = new List<RelatorioNotasEConceitosFinaisAnoDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisAnoDto> Anos { get; set; }
    }
}
