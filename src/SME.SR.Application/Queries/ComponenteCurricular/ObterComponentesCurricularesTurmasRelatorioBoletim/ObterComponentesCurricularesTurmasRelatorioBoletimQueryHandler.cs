using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ComponenteCurricular.ObterComponentesCurricularesTurmasRelatorioBoletim
{
    public class ObterComponentesCurricularesTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterComponentesCurricularesTurmasRelatorioBoletimQuery, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterComponentesCurricularesTurmasRelatorioBoletimQueryHandler(IComponenteCurricularRepository componenteCurricularRepository,
                                                                              IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> Handle(ObterComponentesCurricularesTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var componentesDasTurmas = await componenteCurricularRepository.ObterComponentesPorTurmas(request.CodigosTurma);

            var disciplinasDaTurma = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesDasTurmas.Select(x => x.Codigo).Distinct().ToArray()));

            if (componentesDasTurmas != null && componentesDasTurmas.Any())
            {
                var componentes = await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                var componentesMapeados = componentesDasTurmas?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodigoTurma = c.CodigoTurma,
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(componentes),
                    BaseNacional = c.EhBaseNacional(componentes),
                    Compartilhada = c.EhCompartilhada(componentes),
                    Disciplina = disciplinasDaTurma.FirstOrDefault(d => d.CodDisciplina == c.Codigo).Disciplina,
                    GrupoMatriz = disciplinasDaTurma.FirstOrDefault(d => d.CodDisciplina == c.Codigo).GrupoMatriz,
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
                        CodigosTurma = componentesRegentes.Select(r => r.CodigoTurma).ToArray(),
                        CdComponentesCurriculares = componentesRegentes.DistinctBy(c=> c.CodDisciplina).Select(r => r.CodDisciplina).ToArray(),
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
                            if (c.Regencia)
                                c.ComponentesCurricularesRegencia = componentesRegenciaPorTurma.FirstOrDefault(r => r.Key == c.CodigoTurma)?.ToList();

                            return c;

                        }).ToList();
                    }
                }

                return componentesMapeados.GroupBy(cm => cm.CodigoTurma);
            }

            throw new NegocioException("Não foi possível localizar as disciplinas das tumas");
        }
    }
}
