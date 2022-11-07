using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAulasPorTurmasComponentesTipoCalendarioBimestresQuery : IRequest<IEnumerable<TurmaComponenteDataAulaQuantidadeDto>>
    {
        public ObterAulasPorTurmasComponentesTipoCalendarioBimestresQuery(string[] turmasCodigo, long tipoCalendarioId, string[] componentesCurricularesId, int[] bimestres)
        {
            TurmasCodigo = turmasCodigo;
            TipoCalendarioId = tipoCalendarioId;
            ComponentesCurricularesId = componentesCurricularesId;
            Bimestres = bimestres;
        }

        public string[] TurmasCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public string[] ComponentesCurricularesId { get; set; }
        public int[] Bimestres { get; set; }
    }
}
