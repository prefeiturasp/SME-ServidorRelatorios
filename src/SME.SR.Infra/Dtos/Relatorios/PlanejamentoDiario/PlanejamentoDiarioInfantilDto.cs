using System;

namespace SME.SR.Infra
{
    public class PlanejamentoDiarioInfantilDto
    {
        public long AulaId { get; set; }
        public string ComponenteCurricular { get; set; }
        public bool AulaCJ { get; set; }
        public string DataAula { get; set; }        
        public bool PlanejamentoRealizado { get; set; }
        public string DateRegistro { get; set; }
        public string Usuario { get; set; }
        public string SecoesPreenchidas { get; set; }
        public string Planejamento { get; set; }
    }
}
