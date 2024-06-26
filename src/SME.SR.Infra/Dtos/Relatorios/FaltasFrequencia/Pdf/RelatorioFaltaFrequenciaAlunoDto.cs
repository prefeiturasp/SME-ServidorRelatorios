﻿using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaAlunoDto
    {
        public string NumeroChamada { get; set; }
        public int CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public string NomeTurma { get; set; }
        public string CodigoTurma { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalRemoto { get; set; }
        public int TotalCompensacoes { get; set; }
        public int TotalAulas { get; set; }
        public int TotalPresenca { get; set; }
        public int NumeroFaltasNaoCompensadas { get => TotalAusencias - TotalCompensacoes; }
        public double Frequencia
        {
            get
            {
                if (TotalAulas == 0)
                    return 0;

                var porcentagem = 100 - ((double)NumeroFaltasNaoCompensadas / TotalAulas) * 100;

                return Math.Round(porcentagem, 2);
            }
        }
    }
}

