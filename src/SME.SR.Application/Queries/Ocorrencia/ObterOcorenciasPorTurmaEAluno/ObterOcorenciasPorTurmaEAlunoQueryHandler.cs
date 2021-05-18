using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterOcorenciasPorTurmaEAlunoQueryHandler : IRequestHandler<ObterOcorenciasPorTurmaEAlunoQuery, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>>
    {
        private readonly IOcorrenciaRepository ocorrenciaRepository;

        public ObterOcorenciasPorTurmaEAlunoQueryHandler(IOcorrenciaRepository ocorrenciaRepository)
        {
            this.ocorrenciaRepository = ocorrenciaRepository ?? throw new ArgumentNullException(nameof(ocorrenciaRepository));
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>> Handle(ObterOcorenciasPorTurmaEAlunoQuery request, CancellationToken cancellationToken)
        {
            var ocorrencias = await ocorrenciaRepository.ObterOcorenciasPorTurmaEAluno(request.TurmaId, request.AlunoCodigo, request.DataInicio, request.DataFim);

            return ocorrencias;
        }
    }
}
