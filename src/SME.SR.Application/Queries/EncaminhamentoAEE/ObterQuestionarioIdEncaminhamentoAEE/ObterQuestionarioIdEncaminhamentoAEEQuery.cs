using MediatR;

namespace SME.SR.Application
{
    public class ObterQuestionarioIdEncaminhamentoAEEQuery : IRequest<long>
    {
        public ObterQuestionarioIdEncaminhamentoAEEQuery(string nomeComponenteSecao)
        {
            NomeComponenteSecao = nomeComponenteSecao;
        }

        public string NomeComponenteSecao { get; }
    }
}