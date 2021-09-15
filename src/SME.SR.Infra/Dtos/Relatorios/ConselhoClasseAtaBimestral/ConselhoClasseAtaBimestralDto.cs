using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaBimestralDto
    {
        

        public ConselhoClasseAtaBimestralDto()
        {
            GruposMatriz = new List<ConselhoClasseAtaBimestralGrupoDto>();
            Linhas = new List<ConselhoClasseAtaBimestralLinhaDto>();
        }

        public Modalidade Modalidade;
        public bool EhEJA => Modalidade == Modalidade.EJA;
        public ConselhoClasseAtaBimestralCabecalhoDto Cabecalho { get; set; }
        public List<ConselhoClasseAtaBimestralGrupoDto> GruposMatriz { get; set; }
        public List<ConselhoClasseAtaBimestralLinhaDto> Linhas { get; set; }
    }
}
