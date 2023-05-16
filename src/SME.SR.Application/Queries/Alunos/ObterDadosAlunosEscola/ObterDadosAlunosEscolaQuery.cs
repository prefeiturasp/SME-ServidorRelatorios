using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosAlunosEscolaQuery : IRequest<IEnumerable<DadosAlunosEscolaDto>>
    {
        public ObterDadosAlunosEscolaQuery(string codigoEscola, string codigoDre, int anoLetivo, string[] codigosAlunos)
        {
            CodigoUe = codigoEscola;
            AnoLetivo = anoLetivo;
            CodigosAlunos = codigosAlunos;
            CodigoDre = codigoDre;
        }

        public string CodigoUe { get; set; }
        public string CodigoDre { get; set; }
        public int AnoLetivo { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
