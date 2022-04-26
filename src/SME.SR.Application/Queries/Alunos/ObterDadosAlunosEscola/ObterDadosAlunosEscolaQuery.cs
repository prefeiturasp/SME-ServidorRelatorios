using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosAlunosEscolaQuery : IRequest<IEnumerable<DadosAlunosEscolaDto>>
    {
        public ObterDadosAlunosEscolaQuery(string codigoEscola, int anoLetivo)
        {
            CodigoEscola = codigoEscola;
            AnoLetivo = anoLetivo;
        }

        public string CodigoEscola { get; set; }
        public int AnoLetivo { get; set; }
    }
}
