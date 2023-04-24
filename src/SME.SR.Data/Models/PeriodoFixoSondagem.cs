using System;

namespace SME.SR.Data.Models
{
    public class PeriodoFixoSondagem
    {
        public long Id { get; set; }
        public int Ano { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int TipoPeriodo { get; set; }
        public string PeriodoId { get; set; }
    }
}