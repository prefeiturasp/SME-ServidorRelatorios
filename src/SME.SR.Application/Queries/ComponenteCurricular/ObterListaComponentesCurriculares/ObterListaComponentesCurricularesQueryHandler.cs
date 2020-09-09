using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterListaComponentesCurricularesQueryHandler : IRequestHandler<ObterListaComponentesCurricularesQuery, IEnumerable<ComponenteCurricular>>
    {
        private readonly IComponenteCurricularRepository componenteCurricularRepository;

        public ObterListaComponentesCurricularesQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
        }


        public async Task<IEnumerable<ComponenteCurricular>> Handle(ObterListaComponentesCurricularesQuery request, CancellationToken cancellationToken)
        {
            var componentes = await componenteCurricularRepository.ListarComponentes();
            var componentesApiEol = await componenteCurricularRepository.ListarApiEol();

            componentes.Where(c => componentesApiEol.Any(a => a.IdComponenteCurricular == c.Codigo && a.IdComponenteCurricularPai > 0)).ToList()
                .ForEach(componente =>
                {
                    var componenteApi = componentesApiEol.First(c => c.IdComponenteCurricular == componente.Codigo);
                    var componentePai = componentesApiEol.FirstOrDefault(c => c.IdComponenteCurricular == componenteApi.IdComponenteCurricularPai);
                    if (componentePai != null)
                    {
                        componente.Codigo = componentePai.IdComponenteCurricular;
                        componente.Descricao = componentePai.Descricao;
                    }
                });
            return componentes.DistinctBy(c => c.Codigo);
        }
    }
}
