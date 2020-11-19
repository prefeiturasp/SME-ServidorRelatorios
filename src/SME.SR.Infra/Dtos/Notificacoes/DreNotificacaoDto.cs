using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class DreNotificacaoDto
    {
        public DreNotificacaoDto()
        {
            UEs = new List<UeNotificacaoDto>();
        }

        public string Nome { get; set; }

        public List<UeNotificacaoDto> UEs { get; set; }
    }
}
