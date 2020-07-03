using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class MontarBoletinsQuery : IRequest<BoletimEscolarDto>
    {
        public Dre Dre { get; set; }

        public Ue Ue { get; set; }

        public IEnumerable<Turma> Turmas { get; set; }

        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesPorTurma { get; set; }

        public IEnumerable<IGrouping<string, Aluno>> AlunosPorTuma { get; set; }

        public IEnumerable<IGrouping<string, NotasFrequenciaAlunoBimestre>> NotasFrequencia { get; set; }
    }
}
