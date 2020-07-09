using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarQuery : IRequest<HistoricoEscolarDTO>
    {
        public MontarHistoricoEscolarQuery(Dre dre, Ue ue, IEnumerable<AreaDoConhecimento> areasConhecimento, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesTurmas, IEnumerable<AlunoTurmasHistoricoEscolarDto> alunosTurmas, string[] turmasCodigo)
        {
            Dre = dre;
            Ue = ue;
            AreasConhecimento = areasConhecimento;
            ComponentesCurricularesTurmas = componentesCurricularesTurmas;
            AlunosTurmas = alunosTurmas;
            TurmasCodigo = turmasCodigo;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<AreaDoConhecimento> AreasConhecimento { get; set; }
        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesTurmas { get; set; }
        public IEnumerable<AlunoTurmasHistoricoEscolarDto> AlunosTurmas { get; set; }
        public string[] TurmasCodigo { get; set; }
    }
}
