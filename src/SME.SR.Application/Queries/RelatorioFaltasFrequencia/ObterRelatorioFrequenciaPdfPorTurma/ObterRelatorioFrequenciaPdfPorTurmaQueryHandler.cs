using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorTurmaQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorTurmaQuery, RelatorioFrequenciaTurmaAnoDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorTurmaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFrequenciaTurmaAnoDto> Handle(ObterRelatorioFrequenciaPdfPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var relatorioFrequenciaTurmaAnoDto = new RelatorioFrequenciaTurmaAnoDto();

            foreach (var componente in request.Componentes)
            {
                var componenteAtual = request.Componentes.ToList()
                    .FirstOrDefault(c => c.Codigo.ToString() == componente.CodigoComponente);

                var turmasccc = componente.Alunos.Select(c => c.CodigoTurma).Distinct().ToList();

                if (componenteAtual != null)
                    componente.NomeComponente = componenteAtual.Descricao.ToUpper();

                var frequencias = await mediator
                    .Send(new ObterFrequenciasAlunosConsolidadoQuery(turmasccc.ToArray(), componente.CodigoComponente, bimestre.Numero));
            }
                ultimoAluno = await mediator.Send(new ObterRelatorioFrequenciaPdfPorComponenteQuery(request.Filtro, request.PeriodosEscolares, request.Alunos, request.DeveAdicionarFinal, request.MostrarSomenteFinal));

            return relatorioFrequenciaTurmaAnoDto;
        }
    }
}
