using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaBimestreAlunoDto
    {
        public FrequenciaBimestreAlunoDto()
        {}

        public string CodigoAluno { get; set; }
        public int Bimestre { get; set; }
        public int QuantidadeAusencias { get; set; }
        public int QuantidadeCompensacoes { get; set; }
        public int TotalAulas { get; set; }
        public double Frequencia { get; set; }

    }
}