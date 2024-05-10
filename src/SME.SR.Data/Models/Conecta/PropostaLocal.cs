using System;

namespace SME.SR.Data.Models.Conecta
{
    public class PropostaLocal
    {
        public int TotalTurmas { get; set; }
        public string Local { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string Turma { get; set; }
        public TipoEncontro TipoEncontro { get; set; }
    }
}
