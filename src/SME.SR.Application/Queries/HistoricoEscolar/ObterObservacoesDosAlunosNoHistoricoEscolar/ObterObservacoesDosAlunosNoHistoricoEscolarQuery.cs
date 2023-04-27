using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterObservacoesDosAlunosNoHistoricoEscolarQuery : IRequest<IEnumerable<FiltroHistoricoEscolarAlunosDto>>
    {
        public ObterObservacoesDosAlunosNoHistoricoEscolarQuery(string[] codigosAlunos)
        {
            CodigosAlunos = codigosAlunos;
        }

        public string[] CodigosAlunos { get; }
    }
}
