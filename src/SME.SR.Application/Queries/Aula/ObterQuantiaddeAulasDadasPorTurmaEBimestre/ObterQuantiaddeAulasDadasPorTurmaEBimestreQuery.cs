using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery : IRequest<IEnumerable<QuantidadeAulasDadasBimestreDto>>
    {
        public ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery(string turmaCodigo, long tipoCalendarioId, int[] bimestres)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
            Bimestres = bimestres;
        }

        public string TurmaCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public int[] Bimestres { get; set; }
    }
}
