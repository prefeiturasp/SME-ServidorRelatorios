using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaFinalDto
    {
        public ConselhoClasseAtaFinalDto()
        {
            GruposMatriz = new List<ConselhoClasseAtaFinalGrupoDto>();
            Linhas = new List<ConselhoClasseAtaFinalLinhaDto>();
        }

        public Modalidade Modalidade;
        public bool EhEJA => Modalidade == Modalidade.EJA;
        public bool EhCELP => Modalidade == Modalidade.CELP;
        public ConselhoClasseAtaFinalCabecalhoDto Cabecalho { get; set; }
        public List<ConselhoClasseAtaFinalGrupoDto> GruposMatriz { get; set; }
        public List<ConselhoClasseAtaFinalLinhaDto> Linhas { get; set; }
    }
}
