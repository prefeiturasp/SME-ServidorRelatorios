using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class DevolutivaDto
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataRegistro { get; set; }
        public string RegistradoPor { get; set; }
        public string RegistradoRF { get; set; }
        public string Descricao { get; set; }
        public DataAulaDto Aula { get; set; }
        public string ComponenteCurricular { get; set; }
    }
}
