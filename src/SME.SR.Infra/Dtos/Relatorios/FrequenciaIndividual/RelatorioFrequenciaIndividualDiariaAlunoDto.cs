using System;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualDiariaAlunoDto
    {
        public long? Id { get; set; }



        public DateTime DataAula { get; set; }
        public long QuantidadeAulas { get; set; }
        public int QuantidadePresenca { get; set; }
        public int QuantidadeRemoto { get; set; }
        public long QuantidadeAusencia { get; set; }
        public string Motivo { get; set; }
        public int Bimestre { get; set; }
        public string AlunoCodigo { get; set; }
    }
}
