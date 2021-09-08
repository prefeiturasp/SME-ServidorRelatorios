using MediatR;
using SME.SR.Infra.Dtos.AulasPrevistas;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAulasDadasTurmaBimestreComponenteCurricularQuery : IRequest<IEnumerable<TurmaComponenteQuantidadeAulasDto>>
    {
        public ObterAulasDadasTurmaBimestreComponenteCurricularQuery(string[] turmasCodigo, long tipoCalendarioId, string[] componentesCurricularesId)
        {
            TurmasCodigo = turmasCodigo;
            TipoCalendarioId = tipoCalendarioId;
            ComponentesCurricularesId = componentesCurricularesId;
        }

        public string[] TurmasCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public string[] ComponentesCurricularesId { get; set; }
    }   
}
