﻿using SME.SR.Infra;

namespace SME.SR.Data
{
    public class TipoNotaCicloAno
    {
        public int Ciclo { get; set; }

        public int Ano { get; set; }

        public Modalidade Modalidade { get; set; }

        public string TipoNota { get; set; }
    }
}
