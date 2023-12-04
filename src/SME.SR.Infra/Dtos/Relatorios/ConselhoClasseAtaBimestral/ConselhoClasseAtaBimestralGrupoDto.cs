using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaBimestralGrupoDto
    {
        public ConselhoClasseAtaBimestralGrupoDto()
        {
            ComponentesCurriculares = new List<ConselhoClasseAtaBimestralComponenteDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public int QuantidadeColunas { get; set; }
        public List<ConselhoClasseAtaBimestralComponenteDto> ComponentesCurriculares { get; set; }

        public void AdicionarComponente(long codDisciplina, long codComponenteCurricularTerritorioSaber, string ComponenteCurricular, long idGrupoMatriz, IEnumerable<int> bimestres)
        {
            var componenteCurricularDto = new ConselhoClasseAtaBimestralComponenteDto()
            {
                Id = codDisciplina,
                IdComponenteCurricularTerritorio = codComponenteCurricularTerritorioSaber,
                Nome = ComponenteCurricular,
                IdGrupoMatriz = idGrupoMatriz
            };            
            
            componenteCurricularDto.AdicionarColuna("F");
            componenteCurricularDto.AdicionarColuna("CA");
            componenteCurricularDto.AdicionarColuna("%");
            componenteCurricularDto.AdicionarColuna("N/C");

            ComponentesCurriculares.Add(componenteCurricularDto);
        }
    }
}
