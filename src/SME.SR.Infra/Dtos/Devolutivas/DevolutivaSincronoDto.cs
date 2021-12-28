using System;

namespace SME.SR.Infra
{
    public class DevolutivaSincronoDto
    {
        public long DevolutivaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataRegistro { get; set; }
        public DateTime DataAula { get; set; }
        public string RegistradoPor { get; set; }
        public string RegistradoRF { get; set; }
        public string Descricao { get; set; }
        public long Bimestre { get; set; }
    }
}
