using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaFinalGrupoDto
    {
        public ConselhoClasseAtaFinalGrupoDto()
        {
            ComponentesCurriculares = new List<ConselhoClasseAtaFinalComponenteDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public int QuantidadeColunas { get; set; }
        public List<ConselhoClasseAtaFinalComponenteDto> ComponentesCurriculares { get; set; }

        public void AdicionarComponente(long codDisciplina, string ComponenteCurricular, long idGrupoMatriz, IEnumerable<int> bimestres)
        {
            var componenteCurricularDto = new ConselhoClasseAtaFinalComponenteDto()
            {
                Id = codDisciplina,
                Nome = ComponenteCurricular,
                IdGrupoMatriz = idGrupoMatriz
            };

            // Colunas dos Bimestres
            foreach(var bimestre in bimestres)
                componenteCurricularDto.AdicionarColuna($"{bimestre}º");
            componenteCurricularDto.AdicionarColuna("SF");
            // Colunas de Frequencia
            componenteCurricularDto.AdicionarColuna("F");
            componenteCurricularDto.AdicionarColuna("CA");
            componenteCurricularDto.AdicionarColuna("%");

            ComponentesCurriculares.Add(componenteCurricularDto);
        }
    }
}
