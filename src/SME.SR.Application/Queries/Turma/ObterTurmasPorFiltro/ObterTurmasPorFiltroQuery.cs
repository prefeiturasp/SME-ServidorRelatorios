using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorFiltroQuery : IRequest<IEnumerable<Turma>>
    {
        public string CodigoUe { get; set; }

        public Modalidade? Modalidade { get; set; }

        public int? AnoLetivo { get; set; }

        public int? Semestre { get; set; }
    }
}
