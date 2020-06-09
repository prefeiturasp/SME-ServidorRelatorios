using MediatR;
using SME.SR.Workers.SGP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDadosComponenteSemNotaBimestreQueryHandler : IRequestHandler<ObterDadosComponenteSemNotaBimestreQuery, IEnumerable<GrupoMatrizComponenteSemNotaBimestre>>
    {
        private IMediator _mediator;

        public ObterDadosComponenteSemNotaBimestreQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<IEnumerable<GrupoMatrizComponenteSemNotaBimestre>> Handle(ObterDadosComponenteSemNotaBimestreQuery request, CancellationToken cancellationToken)
        {
            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.CodigoTurma);

            var lstComponentesSemNota = disciplinasPorTurma.Where(x => !x.LancaNota && x.GrupoMatriz != null)
                                             .GroupBy(c => new { c.GrupoMatriz?.Id, c.GrupoMatriz?.Nome });

            var frequenciaAluno = await _mediator.Send(new ObterFrequenciaPorDisciplinaBimestresQuery()
            {
                CodigoTurma = request.CodigoTurma,
                CodigoAluno = request.CodigoAluno,
                Bimestre = request.Bimestre
            });

            var lstGruposMatrizCompSemNota = new List<GrupoMatrizComponenteSemNotaBimestre>();

            foreach (var grupoDisciplinasMatriz in lstComponentesSemNota.OrderBy(k => k.Key.Nome))
            {
                var lstCompSemNota = new List<ComponenteSemNota>();

                foreach (var disciplina in grupoDisciplinasMatriz)
                {
                    var frequenciaDisciplina = frequenciaAluno.Where(x =>
                        x.DisciplinaId == disciplina.CodDisciplina.ToString());

                    var componenteSemNota = new ComponenteSemNota()
                    {
                        Componente = disciplina.Disciplina,
                        Faltas = frequenciaDisciplina?.Sum(x => x.TotalAusencias),
                        Frequencia = frequenciaDisciplina.Any() ? frequenciaDisciplina.Sum(x => x.PercentualFrequencia) / frequenciaDisciplina.Count() : 100
                    };

                    lstCompSemNota.Add(componenteSemNota);
                }

                var grupoMatriz = new GrupoMatrizComponenteSemNotaBimestre()
                {
                    Nome = grupoDisciplinasMatriz.Key.Nome ?? "",
                    ComponentesSemNota = lstCompSemNota
                };

                lstGruposMatrizCompSemNota.Add(grupoMatriz);
            }

            return lstGruposMatrizCompSemNota;
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurricularesPorTurma(string codigoTurma)
        {
            return await _mediator.Send(new ObterComponentesCurricularesPorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }
    }
}
