using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCabecalhoHistoricoEscolarQuery : IRequest<CabecalhoDto>
    {
        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public int? Semestre { get; set; }

        public string TurmaCodigo { get; set; }

        public int AnoLetivo { get; set; }

        public Modalidade? Modalidade { get; set; }

        public string[] AlunosCodigo { get; set; }

    }
}
