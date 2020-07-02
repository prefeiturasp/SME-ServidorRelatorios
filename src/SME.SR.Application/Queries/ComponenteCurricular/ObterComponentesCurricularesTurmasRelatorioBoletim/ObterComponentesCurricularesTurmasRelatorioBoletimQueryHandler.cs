using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ComponenteCurricular.ObterComponentesCurricularesTurmasRelatorioBoletim
{
    public class ObterComponentesCurricularesTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterComponentesCurricularesTurmasRelatorioBoletimQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterComponentesCurricularesTurmasRelatorioBoletimQueryHandler(IComponenteCurricularRepository componenteCurricularRepository,
                                                                              IMediator mediator)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository)); 
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); 
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var componentesDasTurmas = await componenteCurricularRepository.ObterComponentesPorTurmas(request.CodigosTurma);
            if (componentesDasTurmas != null && componentesDasTurmas.Any())
            {
                var componentesApiEol = await componenteCurricularRepository.ListarApiEol();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                var componentesMapeados = componentesDasTurmas?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodigoTurma = c.CodigoTurma,
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(componentesApiEol),
                    BaseNacional = c.EhBaseNacional(componentesApiEol),
                    Compartilhada = c.EhCompartilhada(componentesApiEol),
                    Disciplina = c.DescricaoFormatada,
                    GrupoMatriz = c.ObterGrupoMatriz(componentesApiEol, gruposMatriz),
                    LancaNota = c.PodeLancarNota(componentesApiEol),
                    Regencia = c.EhRegencia(componentesApiEol),
                    TerritorioSaber = c.TerritorioSaber,
                    TipoEscola = c.TipoEscola
                });

                if (componentesMapeados.Any(c => c.Regencia))
                {
                    var componentesRegentes = componentesMapeados.Where(cm => cm.Regencia);

                    var componentesRegenciaPorTurma = await mediator.Send(new ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery()
                    {
                        CodigosTurma = componentesRegentes.Select(r => r.CodigoTurma).ToArray(),
                        CdComponentesCurriculares = componentesRegentes.Select(r => r.CodDisciplina).ToArray(),
                        CodigoUe = request.CodigoUe,
                        Modalidade = request.Modalidade,
                        ComponentesCurricularesApiEol = componentesApiEol,
                        GruposMatriz = gruposMatriz,
                        Usuario = request.Usuario
                    });

                    componentesMapeados = componentesMapeados.Select(cm =>
                     {
                         if (cm.Regencia)
                             cm.ComponentesCurricularesRegencia = componentesRegenciaPorTurma.FirstOrDefault(c => c.Key == cm.CodigoTurma).ToList();

                         return cm;
                     });
                }

                return componentesMapeados;
            }

            throw new NegocioException("Não foi possível localizar as disciplinas das tumas");
        }
    }
}
