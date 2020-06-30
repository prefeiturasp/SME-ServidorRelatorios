using System.Collections.Generic;

namespace SME.SR.Data
{
    public partial class HistoricoEscolarDTO
    {
        public class AreaDeConhecimentoDTO
        {
            public string nome { get; set; }
            public List<ComponenteCurricularDTO> componentesCurriculares { get; set; }

        }


    }
}
