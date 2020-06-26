using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.ConselhoClasse
{
    public class ConselhoClasseAtaFinalGrupoMatrizDto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public int QuantidadeColunas { get; set; }

        public List<ConselhoClasseAtaFinalComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
