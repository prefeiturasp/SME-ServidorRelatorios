using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterIdTipoCalendarioPorAnoLetivoEModalidadeQueryHandler : IRequestHandler<ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery, long>
    {
        private ITipoCalendarioRepository tipoCalendarioRepository;

        public ObterIdTipoCalendarioPorAnoLetivoEModalidadeQueryHandler(ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
        }

        public async Task<long> Handle(ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery request, CancellationToken cancellationToken)
            => await tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(request.AnoLetivo, request.Modalidade, request.Semestre);
    }
}
