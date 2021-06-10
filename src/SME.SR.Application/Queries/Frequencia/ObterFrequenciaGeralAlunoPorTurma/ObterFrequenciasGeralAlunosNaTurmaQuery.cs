using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasGeralAlunosNaTurmaQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciasGeralAlunosNaTurmaQuery(int anoTurma, long tipoCalendarioId)
        {
            AnoTurma = anoTurma;
            TipoCalendarioId = tipoCalendarioId;
        }

        public int AnoTurma { get; set; }

        public long TipoCalendarioId { get; set; }


    }
}
