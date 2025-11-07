using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorTurmasQueryHandler : IRequestHandler<ObterComponentesCurricularesPorTurmasQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IRepositorioElasticComponenteCurricular componenteCurricularElasticRepository;

        public ObterComponentesCurricularesPorTurmasQueryHandler(IComponenteCurricularRepository componenteCurricularRepository, IRepositorioElasticComponenteCurricular componenteCurricularElasticRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.componenteCurricularElasticRepository = componenteCurricularElasticRepository ?? throw new ArgumentNullException(nameof(componenteCurricularElasticRepository)); ;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorTurmasQuery request, CancellationToken cancellationToken)
        {
            var componentesDaTurma = await componenteCurricularElasticRepository.ObterComponentesCurricularesAsync(request.CodigosTurma);

            if (componentesDaTurma != null && componentesDaTurma.Any())
            {
                var informacoesComponentesCurriculares = await componenteCurricularRepository.ListarInformacoesPedagogicasComponentesCurriculares();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                PreencherGruposMatriz(componentesDaTurma, informacoesComponentesCurriculares);

                return componentesDaTurma?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodigoTurma = c.CodigoTurma,
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(informacoesComponentesCurriculares),
                    BaseNacional = c.EhBaseNacional(informacoesComponentesCurriculares),
                    Compartilhada = c.EhCompartilhada(informacoesComponentesCurriculares),
                    Disciplina = c.DescricaoFormatada,
                    GrupoMatriz = c.ObterGrupoMatriz(gruposMatriz),
                    LancaNota = c.PodeLancarNota(informacoesComponentesCurriculares),
                    Regencia = c.EhRegencia(informacoesComponentesCurriculares),
                    TerritorioSaber = c.TerritorioSaber,
                    TipoEscola = c.TipoEscola,
                    Frequencia = c.ControlaFrequencia(informacoesComponentesCurriculares)
                });
            }

            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }

        public void PreencherGruposMatriz(IEnumerable<ComponenteCurricular> componentesCurriculares, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> informacoesComponentesCurriculares)
        {
            foreach (var componente in componentesCurriculares)
            {
                var informacaoComponenteCurricular = informacoesComponentesCurriculares.Where(cce => cce.Codigo == componente.Codigo || cce.Codigo == componente.CodigoComponenteCurricularTerritorioSaber).FirstOrDefault();
                componente.GrupoMatrizId = informacaoComponenteCurricular?.GrupoMatrizId ?? 0;
            }
        }
    }
}
