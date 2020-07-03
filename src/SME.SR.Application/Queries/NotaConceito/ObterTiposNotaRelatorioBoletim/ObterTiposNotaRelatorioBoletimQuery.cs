using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTiposNotaRelatorioBoletimQuery : IRequest<IEnumerable<TipoNotaTurmaPeriodoEscolar>>
    {
        public long DreId { get; set; }
        public long UeId { get; set; }
        public int AnoLetivo { get; set; }
        public IEnumerable<TipoNotaTurmaPeriodoEscolar> Turmas { get; set; }
    }
}
