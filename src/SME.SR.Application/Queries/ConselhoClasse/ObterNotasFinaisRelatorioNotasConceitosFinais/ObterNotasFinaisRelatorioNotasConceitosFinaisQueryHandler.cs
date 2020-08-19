using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasFinaisRelatorioNotasConceitosFinaisQueryHandler : IRequestHandler<ObterNotasFinaisRelatorioNotasConceitosFinaisQuery, IEnumerable<RetornoNotaConceitoBimestreComponenteDto>>
    {
        private readonly IConselhoClasseNotaRepository conselhoClasseNotaRepository;

        public ObterNotasFinaisRelatorioNotasConceitosFinaisQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }

        public async Task<IEnumerable<RetornoNotaConceitoBimestreComponenteDto>> Handle(ObterNotasFinaisRelatorioNotasConceitosFinaisQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseNotaRepository.ObterNotasFinaisRelatorioNotasConceitosFinais(request.DresCodigos, request.UesCodigos, request.Semestre, request.Modalidade, request.Anos, request.AnoLetivo, request.Bimestres, request.ComponentesCurricularesCodigos);
        }
    }
}
