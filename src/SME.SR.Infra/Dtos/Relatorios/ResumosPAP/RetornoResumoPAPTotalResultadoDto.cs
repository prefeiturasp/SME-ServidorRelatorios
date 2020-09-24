using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RetornoResumoPAPTotalResultadoDto : RetornoResumoPAPTotalAlunosAnoDto
    {
        public string Eixo { get; set; }
        public int EixoId { get; set; }
        public string Objetivo { get; set; }
        public int ObjetivoEixoId { get; set; }
        public int ObjetivoId { get; set; }
        public string Resposta { get; set; }
        public int RespostaId { get; set; }
        public int Ordem { get; set; }
    }
}
