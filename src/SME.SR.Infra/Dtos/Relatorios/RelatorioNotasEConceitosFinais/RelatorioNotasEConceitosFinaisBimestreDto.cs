using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisBimestreDto
    {
        public RelatorioNotasEConceitosFinaisBimestreDto(string nome)
        {
            Nome = nome;
            ComponentesCurriculares = new List<RelatorioNotasEConceitosFinaisComponenteCurricularDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
