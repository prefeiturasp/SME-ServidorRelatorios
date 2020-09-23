using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RecuperacaoParalela.ListarTotalAlunosSeries
{
    public class ListarTotalAlunosSeriesQueryHandler : IRequestHandler<ListarTotalAlunosSeriesQuery, IEnumerable<ResumoPAPTotalAlunosAnoDto>>
    {
        private readonly IRecuperacaoParalelaRepository recuperacaoParalelaRepository;

        public ListarTotalAlunosSeriesQueryHandler(IRecuperacaoParalelaRepository recuperacaoParalelaRepository)
        {
            this.recuperacaoParalelaRepository = recuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(recuperacaoParalelaRepository));
        }

        public async Task<IEnumerable<ResumoPAPTotalAlunosAnoDto>> Handle(ListarTotalAlunosSeriesQuery request, CancellationToken cancellationToken)
        {
            var totalAlunos = await recuperacaoParalelaRepository.ListarTotalAlunosSeries(request.Periodo, request.DreId, request.UeId, request.CicloId, 
                request.TurmaId, request.Ano, request.AnoLetivo);

            if (totalAlunos == null)
                throw new NegocioException("Não foi possível obter o total de alunos da recuperação paralela");

            return totalAlunos;
        }
    }
}
