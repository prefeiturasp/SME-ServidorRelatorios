using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosAlunosEscolaQuery : IRequest<IEnumerable<DadosAlunosEscolaDto>>
    {
        public ObterDadosAlunosEscolaQuery(string codigoEscola, int anoLetivo, string[] codigosAlunos)
        {
            CodigoEscola = codigoEscola;
            AnoLetivo = anoLetivo;
            CodigosAlunos = codigosAlunos;
        }

        public string CodigoEscola { get; set; }
        public int AnoLetivo { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
