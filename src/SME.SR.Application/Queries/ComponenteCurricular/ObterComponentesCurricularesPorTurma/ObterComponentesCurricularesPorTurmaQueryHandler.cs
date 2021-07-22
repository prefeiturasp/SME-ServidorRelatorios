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
    public class ObterComponentesCurricularesPorTurmaQueryHandler : IRequestHandler<ObterComponentesCurricularesPorTurmaQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterComponentesCurricularesPorTurmaQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var componentesDaTurma = await componenteCurricularRepository.ObterComponentesPorTurma(request.CodigoTurma);
            if (componentesDaTurma != null && componentesDaTurma.Any())
            {
                var componentesApiEol = await componenteCurricularRepository.ListarApiEol();
                var gruposMatriz = await componenteCurricularRepository.ListarGruposMatriz();

                return componentesDaTurma?.Select(c => new ComponenteCurricularPorTurma
                {
                    CodDisciplina = c.Codigo,
                    CodDisciplinaPai = c.CodigoComponentePai(componentesApiEol),
                    BaseNacional = c.EhBaseNacional(componentesApiEol),
                    Compartilhada = c.EhCompartilhada(componentesApiEol),
                    Disciplina = c.DescricaoFormatada,
                    GrupoMatriz = c.ObterGrupoMatriz(gruposMatriz),
                    LancaNota = c.PodeLancarNota(componentesApiEol),
                    Regencia = c.EhRegencia(componentesApiEol),
                    TerritorioSaber = c.TerritorioSaber,
                    TipoEscola = c.TipoEscola,
                    Frequencia = componentesApiEol.FirstOrDefault(x => x.IdComponenteCurricular == c.Codigo).PermiteRegistroFrequencia

                });
            }

            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }
    }
}
