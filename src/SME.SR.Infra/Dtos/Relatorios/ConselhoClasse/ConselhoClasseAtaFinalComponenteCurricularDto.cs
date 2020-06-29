using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.ConselhoClasse
{
    public class ConselhoClasseAtaFinalComponenteCurricularDto
    {
        public int Id { get; set; }
        public int IdGrupoMatriz { get; set; }
        public string Nome { get; set; }
        public List<ConselhoClasseAtaFinalColunaDto> Colunas { get; set; }
    }
}
