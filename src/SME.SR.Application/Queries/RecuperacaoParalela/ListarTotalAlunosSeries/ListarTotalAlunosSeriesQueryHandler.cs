using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RecuperacaoParalela.ListarTotalAlunosSeries
{
    public class ListarTotalAlunosSeriesQueryHandler : IRequestHandler<ListarTotalAlunosSeriesQuery, ResumoPAPTotalEstudantesDto>
    {
        private readonly IRecuperacaoParalelaRepository recuperacaoParalelaRepository;

        public ListarTotalAlunosSeriesQueryHandler(IRecuperacaoParalelaRepository recuperacaoParalelaRepository)
        {
            this.recuperacaoParalelaRepository = recuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(recuperacaoParalelaRepository));
        }

        public async Task<ResumoPAPTotalEstudantesDto> Handle(ListarTotalAlunosSeriesQuery request, CancellationToken cancellationToken)
        {
            var totalAlunos = await recuperacaoParalelaRepository.ListarTotalAlunosSeries(request.Periodo, request.DreId, request.UeId, request.CicloId, 
                request.TurmaId, request.Ano, request.AnoLetivo);

            if (!totalAlunos.Any()) return null;
            var total = totalAlunos.Sum(s => s.Total);

            return MapearParaDtoTotalEstudantes(total, totalAlunos);
        }

        private ResumoPAPTotalEstudantesDto MapearParaDtoTotalEstudantes(int total, IEnumerable<RetornoResumoPAPTotalAlunosAnoDto> totalAlunosPorSeries)
        {
            return new ResumoPAPTotalEstudantesDto
            {
                QuantidadeTotal = total,
                PorcentagemTotal = 100,
                Anos = totalAlunosPorSeries.Select(x => new ResumoPAPTotalAnoDto
                {
                    AnoDescricao = x.Ano,
                    Quantidade = x.Total,
                    Porcentagem = ((double)(x.Total * 100)) / total
                })
            };
        }
    }
}
