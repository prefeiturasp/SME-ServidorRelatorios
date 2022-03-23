using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterSemestreUltimoAcompanhamentoGeradoQuery: IRequest<UltimoSemestreAcompanhamentoGeradoDto>
    {
        public string AlunoCodigo { get; set; }

        public ObterSemestreUltimoAcompanhamentoGeradoQuery(string alunoCodigo)
        {
            AlunoCodigo = alunoCodigo;
        }
    }
}
