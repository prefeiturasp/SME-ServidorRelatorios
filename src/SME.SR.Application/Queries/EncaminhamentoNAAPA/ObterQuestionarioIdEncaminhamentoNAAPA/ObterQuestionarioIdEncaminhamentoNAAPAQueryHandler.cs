using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterQuestionarioIdEncaminhamentoNAAPAQueryHandler : IRequestHandler<ObterQuestionarioIdEncaminhamentoNAAPAQuery, long>
    {
        private readonly IQuestionarioEncaminhamentoNAAPARepository questionarioEncaminhamentoNAAPARepository;

        public ObterQuestionarioIdEncaminhamentoNAAPAQueryHandler(IQuestionarioEncaminhamentoNAAPARepository questionarioEncaminhamentoNAAPARepository)
        {
            this.questionarioEncaminhamentoNAAPARepository = questionarioEncaminhamentoNAAPARepository ?? throw new ArgumentNullException(nameof(questionarioEncaminhamentoNAAPARepository));
        }

        public Task<long> Handle(ObterQuestionarioIdEncaminhamentoNAAPAQuery request, CancellationToken cancellationToken)
        {
            return questionarioEncaminhamentoNAAPARepository.ObterQuestionarioIdPorTipoESecao((int)TipoQuestionario.EncaminhamentoNAAPA, request.NomeComponenteSecao);
        }
    }
}
