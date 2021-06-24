using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorAlunosQueryHandler : IRequestHandler<ObterComponentesCurricularesPorAlunosQuery, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterComponentesCurricularesPorAlunosQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> Handle(ObterComponentesCurricularesPorAlunosQuery request, CancellationToken cancellationToken)
        {
            var componentesDasTurmas = await componenteCurricularRepository.ObterComponentesPorAlunos(request.AlunosCodigos, request.AnoLetivo, request.Semestre);

            if (componentesDasTurmas != null && componentesDasTurmas.Any())
            {
                var componentesId = componentesDasTurmas.Select(x => x.Codigo).Distinct().ToArray();

                var disciplinasDaTurma = await mediator.Send(new ObterComponentesCurricularesPorIdsQuery(componentesId));

                var componentes = await componenteCurricularRepository.ListarComponentes();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();
                var areasConhecimento = await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(componentesId));

                var componentesMapeados = componentesDasTurmas?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodigoAluno = c.CodigoAluno,
                    CodigoTurma = c.CodigoTurma,
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(componentes),
                    BaseNacional = c.EhBaseNacional(componentes),
                    Compartilhada = c.EhCompartilhada(componentes),
                    Disciplina = disciplinasDaTurma.FirstOrDefault(d => d.Id == c.Codigo).Nome,
                    GrupoMatriz = c.ObterGrupoMatrizSgp(disciplinasDaTurma, gruposMatriz),
                    AreaDoConhecimento = c.ObterAreaDoConhecimento(areasConhecimento),
                    LancaNota = c.PodeLancarNota(componentes),
                    Frequencia = c.ControlaFrequencia(componentes),
                    Regencia = c.EhRegencia(componentes),
                    TerritorioSaber = c.TerritorioSaber,
                    TipoEscola = c.TipoEscola,
                });

                if (componentesMapeados.Any(c => c.Regencia))
                {
                    var componentesRegentes = componentesMapeados.Where(cm => cm.Regencia);

                    var componentesRegenciaPorTurma = await mediator.Send(new ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery()
                    {
                        CodigosTurma = componentesRegentes.Select(r => r.CodigoTurma).Distinct().ToArray(),
                        CdComponentesCurriculares = componentesRegentes.Select(r => r.CodDisciplina).Distinct().ToArray(),
                        CodigoUe = request.CodigoUe,
                        Modalidade = request.Modalidade,
                        ComponentesCurriculares = componentes,
                        GruposMatriz = gruposMatriz,
                        Usuario = request.Usuario
                    });


                    if (componentesRegenciaPorTurma != null && componentesRegenciaPorTurma.Any())
                    {
                        componentesMapeados = componentesMapeados.Select(c =>
                        {
                            if (c.Regencia && componentesRegenciaPorTurma.Any(r => r.Key == c.CodigoTurma))
                                c.ComponentesCurricularesRegencia = componentesRegenciaPorTurma.FirstOrDefault(r => r.Key == c.CodigoTurma).ToList();

                            return c;

                        }).ToList();
                    }
                }

                return componentesMapeados.GroupBy(cm => cm.CodigoAluno);
            } throw new NegocioException("Não foi possível localizar os componentes curriculares da turma.");
        }
    }
}
