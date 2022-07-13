using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasGeralPorAnoEAlunosQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciasGeralPorAnoEAlunosQuery(int anoLetivo, string codigoTurma, long tipoCalendarioId, string[] alunosCodigo)
        {
            AnoLetivo = anoLetivo;
            AlunosCodigo = alunosCodigo;
            CodigoTurma = codigoTurma;
            TipoCalendarioId = tipoCalendarioId;
        }

        public int AnoLetivo { get; set; }
        public string CodigoTurma { get; set; }
        public long TipoCalendarioId { get; set; }
        public string[] AlunosCodigo { get; set; }
    }
}
