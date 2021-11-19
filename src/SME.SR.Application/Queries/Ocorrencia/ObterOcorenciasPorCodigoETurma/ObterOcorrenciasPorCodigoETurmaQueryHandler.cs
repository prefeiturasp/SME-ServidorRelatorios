using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterOcorrenciasPorCodigoETurmaQueryHandler : IRequestHandler<ObterOcorrenciasPorCodigoETurmaQuery, IEnumerable<OcorrenciasPorCodigoTurmaDto>>
    {
        private readonly IOcorrenciaRepository ocorrenciaRepository;
        public ObterOcorrenciasPorCodigoETurmaQueryHandler(IOcorrenciaRepository ocorrenciaRepository)
        {
            this.ocorrenciaRepository = ocorrenciaRepository ?? throw new ArgumentNullException(nameof(ocorrenciaRepository));
        }
        public Task<IEnumerable<OcorrenciasPorCodigoTurmaDto>> Handle(ObterOcorrenciasPorCodigoETurmaQuery request, CancellationToken cancellationToken)
        {
            return ocorrenciaRepository.ObterOcorrenciasCodigoETurma(request.TurmaCodigo,request.OcorrenciaIds);
        }
    }
}
