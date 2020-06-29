using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.ConselhoClasse
{
    public class ConselhoClasseAtaFinalDto
    {
        public ConselhoClasseAtaFinalCabecalhoDto Cabecalho { get; set; }

        public List<ConselhoClasseAtaFinalGrupoMatrizDto> GruposMatriz { get; set; }

        public List<ConselhoClasseAtaFinalLinhaDto> Linhas { get; set; }
    }
}
