using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioRecuperacaoParalelaAlunoSecaoQuery : IRequest<IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto>>
    {
        public string TurmaCodigo { get; internal set; }

        public string AlunoCodigo { get; internal set; }

        public int Semestre { get; internal set; }
    }
}
