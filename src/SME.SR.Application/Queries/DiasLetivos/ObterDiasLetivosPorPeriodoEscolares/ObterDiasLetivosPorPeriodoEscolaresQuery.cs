using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDiasLetivosPorPeriodoEscolaresQuery : IRequest<List<DiaLetivoDto>>
    {
        public ObterDiasLetivosPorPeriodoEscolaresQuery(IEnumerable<PeriodoEscolar> periodosEscolares, long tipoCalendarioId, string ueCodigo = "", bool desconsiderarCriacaoDiaLetivoProximasUes = false)
        {
            PeriodosEscolares = periodosEscolares;
            TipoCalendarioId = tipoCalendarioId;
            DesconsiderarCriacaoDiaLetivoProximasUes = desconsiderarCriacaoDiaLetivoProximasUes;
            UeCodigo = ueCodigo;
        }

        public long TipoCalendarioId { get; set; }
        public string UeCodigo { get; set; }
        public IEnumerable<PeriodoEscolar> PeriodosEscolares { get; set; }
        public bool DesconsiderarCriacaoDiaLetivoProximasUes { get; set; }
    }
}
