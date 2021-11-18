using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Ocorrencia.ObterOcorenciasPorCodigoETurma
{
    public class ObterOcorenciasPorCodigoETurmaQueryHandler : IRequestHandler<ObterOcorenciasPorCodigoETurmaQuery, IEnumerable<OcorrenciasPorCodigoTurmaDto>>
    {
        private readonly IOcorrenciaRepository ocorrenciaRepository;
        public ObterOcorenciasPorCodigoETurmaQueryHandler(IOcorrenciaRepository ocorrenciaRepository)
        {
            this.ocorrenciaRepository = ocorrenciaRepository ?? throw new ArgumentNullException(nameof(ocorrenciaRepository));
        }
        public Task<IEnumerable<OcorrenciasPorCodigoTurmaDto>> Handle(ObterOcorenciasPorCodigoETurmaQuery request, CancellationToken cancellationToken)
        {
            return ocorrenciaRepository.ObterOcorrenciasCodigoETurma(request.TurmaId,request.OcorenciaCodigo);
        }
    }
}
