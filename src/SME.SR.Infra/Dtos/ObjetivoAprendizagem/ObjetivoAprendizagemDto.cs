using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ObjetivoAprendizagemDto
    {
        public long Id { get; set; }
        public long PlanoAulaId { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
