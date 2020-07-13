using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaAlunoDto
    {
        public string Numero { get; set; }
        public string Nome { get; set; }
        public string NomeTurma { get; set; }
        public string Faltas { get; set; }
        public double Frequencia { get; set; }
    }
}
