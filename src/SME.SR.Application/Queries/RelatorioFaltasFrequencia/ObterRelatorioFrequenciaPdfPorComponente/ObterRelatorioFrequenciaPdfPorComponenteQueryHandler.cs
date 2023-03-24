using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorComponenteQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorComponenteQuery, string>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorComponenteQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(ObterRelatorioFrequenciaPdfPorComponenteQuery request, CancellationToken cancellationToken)
        {
            var ultimoAluno = string.Empty;

            foreach (var ue in request.Alunos)
            {
                var componenteAtual = final.Componentes
                    .FirstOrDefault(c => c.CodigoComponente == componente.CodigoComponente);

                var periodoEscolar = periodosEscolares
                    .FirstOrDefault(p => p.Bimestre == int.Parse(bimestre.Numero));

                if (componenteAtual == null)
                {
                    componenteAtual = new RelatorioFrequenciaComponenteDto();
                    componenteAtual.NomeComponente = componente.NomeComponente.ToUpper();
                    componenteAtual.CodigoComponente = componente.CodigoComponente;
                    final.Componentes.Add(componenteAtual);
                }

                if (componente.Alunos.Any())
                {
                    var frequencias = await mediator
                        .Send(new ObterFrequenciasAlunosPorFiltroQuery(componente.Alunos.FirstOrDefault().CodigoTurma,
                            componente.CodigoComponente, int.Parse(bimestre.Numero)));
                }
                
                ultimoAluno = await mediator.Send(new ObterRelatorioFrequenciaPdfPorAlunoQuery(request.Filtro, request.PeriodosEscolares, request.DeveAdicionarFinal, request.MostrarSomenteFinal));
            }
            return ultimoAluno;
        }
    }
}
