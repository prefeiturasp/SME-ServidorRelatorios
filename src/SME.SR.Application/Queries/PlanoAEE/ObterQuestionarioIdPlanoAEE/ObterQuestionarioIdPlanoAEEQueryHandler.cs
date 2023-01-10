using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestionarioIdPlanoAEEQueryHandler : IRequestHandler<ObterQuestionarioIdPlanoAEEQuery, long>
    {
        private readonly IQuestionarioRepository questionarioRepository;

        public ObterQuestionarioIdPlanoAEEQueryHandler(IQuestionarioRepository questionarioRepository)
        {
            this.questionarioRepository = questionarioRepository ?? throw new ArgumentNullException(nameof(questionarioRepository));
        }

        public async Task<long> Handle(ObterQuestionarioIdPlanoAEEQuery request, CancellationToken cancellationToken)
        {
            return await questionarioRepository.ObterQuestionarioIdPorTipo((int)TipoQuestionario.PlanoAEE);
        }
    }
}