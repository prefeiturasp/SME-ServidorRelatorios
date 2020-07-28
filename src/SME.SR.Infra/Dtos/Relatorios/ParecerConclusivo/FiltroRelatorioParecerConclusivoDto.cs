﻿namespace SME.SR.Infra
{
    public class FiltroRelatorioParecerConclusivoDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public Modalidade Modalidade { get; set; }
        public int? Semestre { get; set; }
        public string Ciclo { get; set; }
        public string AnoEscolar { get; set; }
        public long ParecerConclusivoId { get; set; }
    }
}
