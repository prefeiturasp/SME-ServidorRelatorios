using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestionarioIdEncaminhamentoAEEQueryHandler : IRequestHandler<ObterQuestionarioIdEncaminhamentoAEEQuery, long>
    {
        private readonly IQuestionarioEncaminhamentoAeeRepository questionarioRepository;

        public ObterQuestionarioIdEncaminhamentoAEEQueryHandler(IQuestionarioEncaminhamentoAeeRepository questionarioRepository)
        {
            this.questionarioRepository = questionarioRepository ?? throw new ArgumentNullException(nameof(questionarioRepository));
        }

        public async Task<long> Handle(ObterQuestionarioIdEncaminhamentoAEEQuery request, CancellationToken cancellationToken)
        {
            return await questionarioRepository.ObterQuestionarioIdPorTipoESecao((int)TipoQuestionario.EncaminhamentoAEE, request.NomeComponenteSecao);
        }
    }
}