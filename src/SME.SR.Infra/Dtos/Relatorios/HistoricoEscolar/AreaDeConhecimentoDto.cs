using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class AreaDeConhecimentoDto
    {
        
        public string nome { get; set; }
        public List<ComponenteCurricularDto> componentesCurriculares { get; set; }

    }

}
