using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
   public class PeriodoFechamentoBimestre
    {
        public PeriodoFechamentoBimestre(long fechamentoId,
                                  PeriodoEscolar periodoEscolar,
                                  DateTime? inicioDoFechamento,
                                  DateTime? finalDoFechamento)
        {
            PeriodoFechamentoId = fechamentoId;
            if (inicioDoFechamento.HasValue)
                InicioDoFechamento = inicioDoFechamento.Value;
            if (finalDoFechamento.HasValue)
                FinalDoFechamento = finalDoFechamento.Value;
            PeriodoEscolar = periodoEscolar;
            PeriodoEscolarId = periodoEscolar.Id;
        }

        protected PeriodoFechamentoBimestre()
        {
        }

        public long PeriodoFechamentoId { get; set; }
        public DateTime FinalDoFechamento { get; set; }
        public long Id { get; set; }
        public DateTime InicioDoFechamento { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
        public long PeriodoEscolarId { get; set; }
    }
}
