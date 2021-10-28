using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTotalAulasPorDisciplinaTurmaBimestreQuery : IRequest<int>
    {
        public ObterTotalAulasPorDisciplinaTurmaBimestreQuery(int bimestre, string disciplinaId, string[] turmasId)
        {
            Bimestre = bimestre;
            DisciplinaId = disciplinaId;
            TurmasId = turmasId;
        }
        public ObterTotalAulasPorDisciplinaTurmaBimestreQuery(int bimestre, string disciplinaId, string turmaId)
        {
            Bimestre = bimestre;
            DisciplinaId = disciplinaId;
            TurmasId = new string[] { turmaId };
        }

        public int Bimestre { get; set; }
        public string DisciplinaId { get; set; }
        public string[] TurmasId { get; set; }
    }
}
