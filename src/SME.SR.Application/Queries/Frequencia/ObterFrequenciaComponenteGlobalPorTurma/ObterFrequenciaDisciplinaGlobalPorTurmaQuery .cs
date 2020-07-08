using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaComponenteGlobalPorTurmaQuery (string turmaCodigo, long tipoCalendarioId)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
        }

        public string TurmaCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
