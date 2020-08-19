using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisAnoDto
    {
        public RelatorioNotasEConceitosFinaisAnoDto(string nome)
        {
            Nome = nome;
            Bimestres = new List<RelatorioNotasEConceitosFinaisBimestreDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisBimestreDto> Bimestres { get; set; }
    }
}
