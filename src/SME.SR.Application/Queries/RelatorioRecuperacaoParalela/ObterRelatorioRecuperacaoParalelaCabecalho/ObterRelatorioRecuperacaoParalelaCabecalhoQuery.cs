using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioRecuperacaoParalelaCabecalhoQuery : IRequest<RelatorioRecuperacaoParalelaDto>
    {
        public string TurmaCodigo { get; internal set; }

        public string AlunoCodigo { get; internal set; }

        public int Semestre { get; internal set; }
    }
}
