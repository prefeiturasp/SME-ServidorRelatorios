using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarQuery : IRequest<HistoricoEscolarDTO>
    {
        public MontarHistoricoEscolarQuery(IEnumerable<AreaDoConhecimento> areasConhecimento, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesTurmas, IEnumerable<AlunoTurmasHistoricoEscolarDto> alunosTurmas, string[] turmasCodigo)
        {
            AreasConhecimento = areasConhecimento;
            ComponentesCurricularesTurmas = componentesCurricularesTurmas;
            AlunosTurmas = alunosTurmas;
            TurmasCodigo = turmasCodigo;
        }

        public IEnumerable<AreaDoConhecimento> AreasConhecimento { get; set; }
        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesTurmas { get; set; }
        public IEnumerable<AlunoTurmasHistoricoEscolarDto> AlunosTurmas { get; set; }
        public string[] TurmasCodigo { get; set; }
    }
}
