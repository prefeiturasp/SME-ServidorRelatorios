using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisAnoDto
    {
        public RelatorioNotasEConceitosFinaisAnoDto(string ano, string nome)
        {
            Ano = ano;
            Nome = nome;
            Bimestres = new List<RelatorioNotasEConceitosFinaisBimestreDto>();
        }

        public string Ano { get; set; }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisBimestreDto> Bimestres { get; set; }
    }
}
