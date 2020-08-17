﻿using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FiltroRelatorioNotasEConceitosFinaisDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public Modalidade? Modalidade { get; set; }
        public int? Semestre { get; set; }
        public string[] Anos { get; set; }
        public long[] ComponentesCurriculares { get; set; }
        public List<int> Bimestres { get; set; }
        public string UsuarioRf { get; set; }
        public string UsuarioNome { get; set; }
        public CondicoesRelatorioNotasEConceitosFinais Condicao { get; set; }
        public int ValorCondicao { get; set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
    }
}