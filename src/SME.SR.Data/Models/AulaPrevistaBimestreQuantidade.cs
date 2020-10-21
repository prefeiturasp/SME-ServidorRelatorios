﻿using System;

namespace SME.SR.Data
{
    public class AulaPrevistaBimestreQuantidade : AulaPrevistaBimestre
    {
        public int CriadasTitular { get; set; }

        public int CriadasCJ { get; set; }

        public int Cumpridas { get; set; }

        public DateTime Inicio { get; set; }

        public DateTime Fim { get; set; }

        public int Reposicoes { get; set; }
    }
}
