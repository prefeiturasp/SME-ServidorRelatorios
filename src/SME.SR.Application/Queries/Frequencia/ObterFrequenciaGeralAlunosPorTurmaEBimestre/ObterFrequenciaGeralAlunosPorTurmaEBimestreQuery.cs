using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery(long turmaId, string alunoCodigo, int[] bimestres)
        {
            TurmaId = turmaId;
            AlunoCodigo = alunoCodigo;
            Bimestres = bimestres;
        }

        public long TurmaId { get; set; }
        public string AlunoCodigo { get; set; }
        public int[] Bimestres { get; set; }
    }
}
