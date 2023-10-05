using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigosTurmaQuery : IRequest<IEnumerable<ComponenteCurricular>>
    {
        public string[] CodigosTurma { get; set; }
        public IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> ComponentesCurriculares { get; set; }
        public bool EhEJA { get; set; }
        public bool IgnorarAdicaoComponentesPlanejamentoRegencia { get; set; }


        public ObterComponentesCurricularesPorCodigosTurmaQuery(string[] codigosTurma, IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesCurriculares = null, 
                                                                bool ehEJA = false, bool ignorarAdicaoComponentesPlanejamentoRegencia = false)
        {
            CodigosTurma = codigosTurma;
            ComponentesCurriculares = componentesCurriculares;
            EhEJA = ehEJA;
            IgnorarAdicaoComponentesPlanejamentoRegencia = ignorarAdicaoComponentesPlanejamentoRegencia;
        }

        public ObterComponentesCurricularesPorCodigosTurmaQuery()
        {
            
        }
    }
}
