using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTotalAulasPorDisciplinaTurmaBimestreQueryHandler : IRequestHandler<ObterTotalAulasPorDisciplinaTurmaBimestreQuery, int>
    {
        private readonly IRelatorioFrequenciaRepository relatorioFrequenciaRepository;

        public ObterTotalAulasPorDisciplinaTurmaBimestreQueryHandler(IRelatorioFrequenciaRepository relatorioFrequenciaRepository)
        {
            this.relatorioFrequenciaRepository = relatorioFrequenciaRepository ?? throw new ArgumentNullException(nameof(relatorioFrequenciaRepository));
        }

        public async Task<int> Handle(ObterTotalAulasPorDisciplinaTurmaBimestreQuery request, CancellationToken cancellationToken)
            => await relatorioFrequenciaRepository.ObterTotalAulasPorDisciplinaTurmaBimestre(request.Bimestre, request.DisciplinaId, request.TurmasId);
    }
}
