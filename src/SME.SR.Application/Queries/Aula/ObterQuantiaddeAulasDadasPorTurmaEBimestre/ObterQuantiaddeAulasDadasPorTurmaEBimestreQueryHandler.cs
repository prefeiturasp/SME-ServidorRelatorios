using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterQuantiaddeAulasDadasPorTurmaEBimestreQueryHandler : IRequestHandler<ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery, IEnumerable<QuantidadeAulasDadasBimestreDto>>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterQuantiaddeAulasDadasPorTurmaEBimestreQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<IEnumerable<QuantidadeAulasDadasBimestreDto>> Handle(ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery request, CancellationToken cancellationToken)
        {
            var aulasDadas = await aulaRepository.ObterAulasDadasPorTurmaEBimestre(request.TurmaCodigo, request.TipoCalendarioId, request.Bimestres);

            return aulasDadas;
        }
    }
}
