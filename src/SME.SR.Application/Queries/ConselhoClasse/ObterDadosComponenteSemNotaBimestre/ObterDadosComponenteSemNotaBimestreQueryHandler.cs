using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
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
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterDadosComponenteSemNotaBimestreQueryHandler(IMediator mediator, IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }

        public async Task<IEnumerable<GrupoMatrizComponenteSemNotaBimestre>> Handle(ObterDadosComponenteSemNotaBimestreQuery request, CancellationToken cancellationToken)
        {
            var disciplinasPorTurma = await ObterComponentesCurricularesPorTurma(request.CodigoTurma);
            var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();
            var turma = await mediator.Send(new ObterTurmaQuery(request.CodigoTurma));

            var lstComponentesSemNota = disciplinasPorTurma.Where(x => !x.LancaNota && x.GrupoMatrizId != 0)
                                             .GroupBy(c => c.ObterGrupoMatriz(gruposMatriz));

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
                        x.DisciplinaId == disciplina.Codigo.ToString());

                    if (request.Bimestre.HasValue)
                        frequenciaDisciplina.ToList().ForEach(f => f.AdicionarFrequenciaBimestre(request.Bimestre.Value, f.PercentualFrequencia));

                    var consideraFrequenciaEspecifica = turma.AnoLetivo.Equals(2020) && request.Bimestre.HasValue;
                    var frequencia = (double)100;

                    if (frequenciaDisciplina.Any() && frequenciaDisciplina.Any(a=> a.PercentualFrequenciaFinal.HasValue))
                        frequencia = frequenciaDisciplina.Sum(x => consideraFrequenciaEspecifica ? x.PercentualFrequenciaFinal.Value : x.PercentualFrequencia) / frequenciaDisciplina.Count();

                    var componenteSemNota = new ComponenteSemNota()
                    {
                        Componente = disciplina.Descricao,
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

        private async Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorTurma(string codigoTurma)
        {
            var componentes = await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares();

            return await mediator.Send(new ObterComponentesCurricularesPorCodigosTurmaQuery()
            {
                CodigosTurma = new string[] { codigoTurma },
                ComponentesCurriculares = componentes
            });
        }
    }
}
