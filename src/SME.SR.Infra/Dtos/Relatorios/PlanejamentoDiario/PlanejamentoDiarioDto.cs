using System;

namespace SME.SR.Infra
{
    public class PlanejamentoDiarioDto
    {
        public long AulaId { get; set; }
        public bool AulaCJ { get; set; }
        public string DataAula { get; set; }
        public long QuantidadeAulas { get; set; }
        public bool PlanejamentoRealizado { get; set; }
        public string DateRegistro { get; set; }
        public string Usuario { get; set; }
        public string SecoesPreenchidas { get; set; }
        public string ObjetivosSelecionados { get; set; }
        public string MeusObjetivosEspecificos { get; set; }
        public int QtdObjetivosEspecificos { get; set; }
        public int QtdSecoesPreenchidas { get; set; }

    }
}
