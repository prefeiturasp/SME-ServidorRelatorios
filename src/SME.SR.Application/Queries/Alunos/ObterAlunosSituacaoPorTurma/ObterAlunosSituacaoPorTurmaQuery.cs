using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAlunosSituacaoPorTurmaQuery: IRequest<IEnumerable<AlunoSituacaoDto>>
    {
        public ObterAlunosSituacaoPorTurmaQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }
    }
}
