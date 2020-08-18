using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasFinaisPorTurmasBimestresComponentesQueryHandler : IRequestHandler<ObterNotasFinaisPorTurmasBimestresComponentesQuery, IEnumerable<RetornoNotaConceitoBimestreComponenteDto>>
    {
        private IConselhoClasseNotaRepository conselhoClasseNotaRepository;

        public ObterNotasFinaisPorTurmasBimestresComponentesQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }

        public async Task<IEnumerable<RetornoNotaConceitoBimestreComponenteDto>> Handle(ObterNotasFinaisPorTurmasBimestresComponentesQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseNotaRepository.ObterNotasFinaisPorTurmasBimestresComponentes(request.DresCodigos, request.UesCodigos, request.TurmasId, request.Semestre, request.Modalidade, request.Anos, request.AnoLetivo, request.Bimestres, request.ComponentesCurricularesCodigos);
        }
    }
}
