using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroRelatorioAnaliticoSondagemDto
    {
        public int AnoLetivo { get; set; }
        public string AnoTurma { get; set; }
        public int Periodo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string LoginUsuarioLogado { get; set; }
        public Guid PerfilUsuarioLogado { get; set; }
        public TipoSondagem TipoSondagem { get; set; }
        public bool ApresentarTurmasUesDresSemLancamento { get; set; }
    }
}
