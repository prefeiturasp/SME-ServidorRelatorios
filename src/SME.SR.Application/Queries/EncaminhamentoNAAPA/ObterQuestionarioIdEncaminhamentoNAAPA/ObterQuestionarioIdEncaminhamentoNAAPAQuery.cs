using MediatR;

namespace SME.SR.Application
{
    public class ObterQuestionarioIdEncaminhamentoNAAPAQuery : IRequest<long>
    {
        public ObterQuestionarioIdEncaminhamentoNAAPAQuery(string nomeComponenteSecao)
        {
            NomeComponenteSecao = nomeComponenteSecao;
        }

        public string NomeComponenteSecao { get; }
    }
}
