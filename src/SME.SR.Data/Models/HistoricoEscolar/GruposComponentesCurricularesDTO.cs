using System.Collections.Generic;

namespace SME.SR.Data
{
    public partial class HistoricoEscolarDTO
    {
        public class GruposComponentesCurricularesDTO
        {
            public string nome { get; set; }
            public List<AreaDeConhecimentoDTO> areasDeConhecimento { get; set; }

        }


    }
}
