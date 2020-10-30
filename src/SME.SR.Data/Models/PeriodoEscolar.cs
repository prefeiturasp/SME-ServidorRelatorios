using System;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class PeriodoEscolar
    {
        public long Id { get; set; }
        public int Bimestre { get; set; }
        public DateTime PeriodoFim { get; set; }
        public DateTime PeriodoInicio { get; set; }

        public IEnumerable<DateTime> ObterIntervaloDatas()
        {
            var datas = new List<DateTime>();
            for (var dia = PeriodoInicio.Date; dia <= PeriodoFim.Date; dia = dia.AddDays(1))
                datas.Add(dia);
            return datas;
        }        
    }
}
