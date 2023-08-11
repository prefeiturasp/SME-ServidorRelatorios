using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BimestreDescricaoObjetivosPlanoAnualDto
    {
        public int Bimestre { get; set; }
        public string DescricaoPlanejamento { get; set; }
        public IEnumerable<ObjetivoAprendizagemPlanoAnualDto> Objetivos { get; set; }
    }
}
