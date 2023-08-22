using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQuery(string codigoAluno, int anoTurma, long tipoCalendarioId = 0)
        {
            CodigoAluno = codigoAluno;
            AnoTurma = anoTurma;
            TipoCalendarioId = tipoCalendarioId;
        }

        public string CodigoAluno { get; set; }
        public int AnoTurma { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
