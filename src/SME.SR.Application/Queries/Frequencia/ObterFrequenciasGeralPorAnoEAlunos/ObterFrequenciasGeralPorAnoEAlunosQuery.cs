using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasGeralPorAnoEAlunosQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciasGeralPorAnoEAlunosQuery(int anoLetivo, long tipoCalendarioId, string[] alunosCodigo)
        {
            AnoLetivo = anoLetivo;
            TipoCalendarioId = tipoCalendarioId;
            AlunosCodigo = alunosCodigo;
        }

        public int AnoLetivo { get; set; }
        public long TipoCalendarioId { get; set; }
        public string[] AlunosCodigo { get; set; }
    }
}
