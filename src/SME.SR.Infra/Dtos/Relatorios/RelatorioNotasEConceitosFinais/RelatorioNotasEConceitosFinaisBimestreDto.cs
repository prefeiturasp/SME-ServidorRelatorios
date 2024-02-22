using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisBimestreDto
    {
        public RelatorioNotasEConceitosFinaisBimestreDto(int? bimestre, string nome)
        {
            Bimestre = bimestre;
            Nome = nome;
            ComponentesCurriculares = new List<RelatorioNotasEConceitosFinaisComponenteCurricularDto>();
        }
        public int? Bimestre { get; set; }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
