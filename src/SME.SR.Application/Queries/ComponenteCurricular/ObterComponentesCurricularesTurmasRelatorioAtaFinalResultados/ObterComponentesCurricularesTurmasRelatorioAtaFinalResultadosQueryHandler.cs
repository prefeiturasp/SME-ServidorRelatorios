﻿using MediatR;
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
    public class ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQueryHandler : IRequestHandler<ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery, IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQueryHandler(IComponenteCurricularRepository componenteCurricularRepository,
                                                                              IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> Handle(ObterComponentesCurricularesTurmasRelatorioAtaFinalResultadosQuery request, CancellationToken cancellationToken)
        {
            var componentesDasTurmas = await mediator.Send(new ObterComponentesCurricularesPorCodigosTurmaQuery(request.CodigosTurma, ignorarAdicaoComponentesPlanejamentoRegencia: true)); 
            var componentesRegencia = await mediator.Send(new ObterComponentesCurricularesPorTurmasQuery(request.CodigosTurma));
            
            if (componentesRegencia.Any(x=> x.Regencia == true))
            {
                var turmas = await mediator.Send(new ObterTurmasPorCodigoQuery(request.CodigosTurma));
                var tipoCalendarioId = await mediator.Send(new ObterTipoCalendarioIdPorTurmaQuery(turmas.FirstOrDefault()));
                var totalAulasSemFrequencia = await mediator.Send(new ObterTotalAlunosSemFrequenciaPorTurmaBimestreQuery(componentesDasTurmas.Select(x => x.Codigo.ToString()).ToArray(), request.CodigosTurma, request.Bimestres));
                var aulasDaTurma = await mediator.Send(new ObterTotalAulasTurmaEBimestreEComponenteCurricularQuery(request.CodigosTurma, tipoCalendarioId, componentesDasTurmas.Select(x => x.Codigo.ToString()).ToArray(), request.Bimestres));
                var componentesSemRegencia = componentesRegencia.Where(r => !r.Regencia);
                var componentesComAula = aulasDaTurma.Select(a => a.ComponenteCurricularCodigo).ToList();
                var componentesCodigosRegencia = componentesRegencia.Where(r => r.Regencia).Select(a => a.CodDisciplina).ToList();

                if (componentesSemRegencia.Any())
                    foreach(var componente in componentesSemRegencia)
                    {
                        componentesComAula.Add(componente.CodDisciplina.ToString());
                    }

                componentesDasTurmas = componentesDasTurmas.Where(x => componentesComAula.Contains(x.Codigo.ToString())
                || totalAulasSemFrequencia.Any(t => t.ComponenteCurricularId.Equals(x.Codigo.ToString())) || componentesCodigosRegencia.Contains(x.Codigo));
            }

            var componentesCurricularesTurmas = new List<ComponenteCurricularPorTurma>();
            if (request.CodigosTurma.Length > 0)
            {
                foreach(var turmaCodigo in request.CodigosTurma)
                {
                    componentesCurricularesTurmas.AddRange(await ObterComponentesCurriculares(componentesDasTurmas.Select(x => x.Codigo).Distinct().ToArray(), turmaCodigo));
                }
            }

            if (componentesDasTurmas != null && componentesDasTurmas.Any())
            {
                var componentes = await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                var componentesMapeados = componentesDasTurmas?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodigoTurma = c.CodigoTurma,
                    CodDisciplina = c.Codigo,
                    CodigoComponenteCurricularTerritorioSaber = c.CodigoComponenteCurricularTerritorioSaber,
                    CodDisciplinaPai = c.CodigoComponentePai(componentes),
                    BaseNacional = c.EhBaseNacional(componentes),
                    Compartilhada = c.EhCompartilhada(componentes),
                    Disciplina = componentesCurricularesTurmas.FirstOrDefault(d => d.CodDisciplina == c.Codigo)?.Disciplina,
                    GrupoMatriz = componentesCurricularesTurmas.FirstOrDefault(d => d.CodDisciplina == c.Codigo)?.GrupoMatriz,
                    LancaNota = c.PodeLancarNota(componentes),
                    Frequencia = c.ControlaFrequencia(componentes),
                    Regencia = c.EhRegencia(componentes),
                    TerritorioSaber = c.TerritorioSaber,
                    TipoEscola = c.TipoEscola,
                }).DistinctBy(c => (c.CodDisciplina, c.CodigoTurma));

                if (componentesMapeados.Any(c => c.Regencia))
                {
                    var componentesRegentes = componentesMapeados.Where(cm => cm.Regencia).ToList();

                    var componentesRegenciaPorTurma = await mediator.Send(new ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery()
                    {
                        CodigosTurma = componentesRegentes.Select(r => r.CodigoTurma).ToArray(),
                        CdComponentesCurriculares = componentesRegentes.DistinctBy(c => c.CodDisciplina).Select(r => r.CodDisciplina).ToArray(),
                        CodigoUe = request.CodigoUe,
                        Modalidade = request.Modalidade,
                        ComponentesCurriculares = componentes,
                        GruposMatriz = gruposMatriz
                    });

                    if (componentesRegenciaPorTurma != null && componentesRegenciaPorTurma.Any())
                    {
                        componentesMapeados = componentesMapeados.Select(c =>
                        {
                            if (c.Regencia)
                                c.ComponentesCurricularesRegencia = componentesRegenciaPorTurma.FirstOrDefault(r => r.Key == c.CodigoTurma).ToList();

                            return c;

                        }).ToList();
                    }
                }

                return componentesMapeados.GroupBy(cm => cm.CodigoTurma);
            }

            throw new NegocioException("Não foi possível localizar as disciplinas das tumas");
        }


        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(IEnumerable<long> componentesCurriculares, string turmaId)
        {
            string[] turmaCodigo = { turmaId };
            var componentes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesCurriculares.ToArray(), turmaCodigo));
            return componentes;
        }
    }
}
