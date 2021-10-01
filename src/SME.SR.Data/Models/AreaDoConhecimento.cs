using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Data
{
    public class AreaDoConhecimento
    {
        public long Id { get; set; }

        public string Nome { get; set; }

        public long CodigoComponenteCurricular { get; set; }
        public string NomeComponenteCurricular { get; set; }

        public int? Ordem { get; private set; }

        public void DefinirOrdem(IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao, long grupoMatrizId) 
        {
            int? ordem = grupoAreaOrdenacao.FirstOrDefault(o => o.GrupoMatrizId == grupoMatrizId && o.AreaConhecimentoId == Id)?.Ordem;

            if (ordem.HasValue)
                Ordem = ordem;
        }
    }
}
