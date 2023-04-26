using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosComponenteSemNotaBimestreQueryHandler : IRequestHandler<ObterDadosComponenteSemNotaBimestreQuery, IEnumerable<GrupoMatrizComponenteSemNotaBimestre>>
    {
        private readonly IMediator mediator;

        public ObterDadosComponenteSemNotaBimestreQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<GrupoMatrizComponenteSemNotaBimestre>> Handle(ObterDadosComponenteSemNotaBimestreQuery request, CancellationToken cancellationToken)
        {
            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.CodigoTurma);
            var turma = await mediator.Send(new ObterTurmaQuery(request.CodigoTurma));

            var lstComponentesSemNota = disciplinasPorTurma.Where(x => !x.LancaNota && x.GrupoMatriz != null)
                                             .GroupBy(c => new { c.GrupoMatriz?.Id, c.GrupoMatriz?.Nome });

            var frequenciaAluno = await mediator.Send(new ObterFrequenciaPorDisciplinaBimestresQuery()
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

                    if (request.Bimestre.HasValue)
                        frequenciaDisciplina.ToList().ForEach(f => f.AdicionarFrequenciaBimestre(request.Bimestre.Value, f.PercentualFrequencia));

                    var consideraFrequenciaEspecifica = turma.AnoLetivo.Equals(2020) && request.Bimestre.HasValue;
                    var frequencia = (double)100;

                    if (frequenciaDisciplina.Any() && frequenciaDisciplina.Any(a=> a.PercentualFrequenciaFinal.HasValue))
                        frequencia = frequenciaDisciplina.Sum(x => consideraFrequenciaEspecifica ? x.PercentualFrequenciaFinal.Value : x.PercentualFrequencia) / frequenciaDisciplina.Count();

                    var componenteSemNota = new ComponenteSemNota()
                    {
                        Componente = disciplina.Disciplina,
                        Faltas = frequenciaDisciplina?.Sum(x => x.TotalAusencias),
                        Frequencia = FrequenciaAluno.FormatarPercentual(frequencia)
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
            return await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }
    }
}
