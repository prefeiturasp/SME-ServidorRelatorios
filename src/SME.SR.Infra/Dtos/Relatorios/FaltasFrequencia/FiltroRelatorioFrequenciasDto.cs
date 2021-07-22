﻿using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FiltroRelatorioFrequenciasDto
    {
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public Modalidade Modalidade { get; set; }
        public int Semestre { get; set; }
        public TipoRelatorioFaltasFrequencia TipoRelatorio { get; set; }
        public IEnumerable<string> AnosEscolares { get; set; }
        public bool TurmasPrograma { get; set; }
        public string CodigoTurma { get; set; }
        public IEnumerable<string> ComponentesCurriculares { get; set; }
        public List<int> Bimestres { get; set; }
        public CondicoesRelatorioFaltasFrequencia Condicao { get; set; }
        public int QuantidadeAusencia { get; set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
        public string NomeUsuario { get; set; }
        public string CodigoRf { get; set; }
    }
}
