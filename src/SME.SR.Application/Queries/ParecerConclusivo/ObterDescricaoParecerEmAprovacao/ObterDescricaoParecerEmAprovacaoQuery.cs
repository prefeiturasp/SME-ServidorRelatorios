using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDescricaoParecerEmAprovacaoQuery : IRequest<string>
    {
        public string CodigoAluno { get; set; }
        public int AnoLetivo { get; set; }

        public ObterDescricaoParecerEmAprovacaoQuery(string codigoAluno, int ano)
        {
            this.CodigoAluno = codigoAluno;
            this.AnoLetivo = ano;
        }
    }
}
