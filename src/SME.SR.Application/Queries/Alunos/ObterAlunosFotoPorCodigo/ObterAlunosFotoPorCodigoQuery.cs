using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosFotoPorCodigoQuery : IRequest<IEnumerable<AlunoFotoArquivoDto>>
    {
        public ObterAlunosFotoPorCodigoQuery(string[] alunosCodigo)
        {
            AlunosCodigo = alunosCodigo;
        }

        public string[] AlunosCodigo { get; set; }
    }
}
