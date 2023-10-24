using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class AdicionarComponentesCurricularesTerritorioSaberTurmaQuery : IRequest<List<ComponenteCurricular>>
    {
        public AdicionarComponentesCurricularesTerritorioSaberTurmaQuery()
        {}

        public AdicionarComponentesCurricularesTerritorioSaberTurmaQuery(string[] codigosTurmas, List<ComponenteCurricular>  componentesCurricularesTurma, string rfProfessor = null)
        {
            this.CodigosTurmas = codigosTurmas;
            this.ComponentesCurricularesTurma = componentesCurricularesTurma;
            this.RfProfessor = rfProfessor;
        }

        public string[] CodigosTurmas { get; set; }
        public List<ComponenteCurricular> ComponentesCurricularesTurma { get; set; }
        public string RfProfessor { get; set; }
    }
}
