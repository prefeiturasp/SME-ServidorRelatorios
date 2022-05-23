using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroRelatorioPlanejamentoDiarioDto
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public int Semestre { get; set; }
        public string CodigoTurma { get; set; }
        public long ComponenteCurricular { get; set; }
        public int Bimestre { get; set; }
        public bool ListarDataFutura { get; set; }
        public bool ExibirDetalhamento { get; set; }
        public string UsuarioNome { get; set; }
        public long[] ComponentesCurricularesDisponiveis { get; set; }
    }
}
