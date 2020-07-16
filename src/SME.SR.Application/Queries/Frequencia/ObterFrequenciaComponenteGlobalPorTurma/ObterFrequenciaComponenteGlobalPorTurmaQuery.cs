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
        public ObterFrequenciaComponenteGlobalPorTurmaQuery (string turmaCodigo, long tipoCalendarioId, IEnumerable<int> bimestres)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
            Bimestres = bimestres;
        }

        public string TurmaCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public IEnumerable<int> Bimestres { get; set; }
    }
}
