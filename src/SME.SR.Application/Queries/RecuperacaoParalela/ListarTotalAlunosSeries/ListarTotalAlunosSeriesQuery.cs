using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ListarTotalAlunosSeriesQuery : IRequest<ResumoPAPTotalEstudantesDto>
    {
        public int? Periodo { get; set; }

        public string DreId { get; set; }

        public string UeId { get; set; }

        public int? CicloId { get; set; }

        public string TurmaId { get; set; }

        public string Ano { get; set; }

        public int AnoLetivo { get; set; }
    }
}
