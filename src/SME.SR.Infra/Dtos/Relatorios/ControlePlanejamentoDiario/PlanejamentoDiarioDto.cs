using System;

namespace SME.SR.Infra
{
    public class PlanejamentoDiarioDto
    {       
        public DateTime DataAula { get; set; }
        public long QuantidadeAulas { get; set; }
        public string PlanejamentoRealizado { get; set; }
        public DateTime DateRegistro { get; set; }
        public string Usuario { get; set; }
        public string SecoesPreenchidas { get; set; }
        public string ObjetivosSelecionados { get; set; }
        public string MeusObjetivosEspecificos { get; set; }
        public string DesenvolvimentoAula { get; set; }        
    }
}
