using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto
    {
        public string Nome { get; set; }
        public int QuantidadeAulas { get; set; }
        public int FrequenciasPendentes { get; set; }
        public DateTime? DataUltimoRegistroFrequencia { get; set; }
        public int PlanosAulaPendentes { get; set; }
        public DateTime? DataUltimoRegistroPlanoAula { get; set; }
    }
}
