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
        public string DataUltimoRegistroFrequencia { get; set; }
        public int PlanosAulaPendentes { get; set; }
        public string DataUltimoRegistroPlanoAula { get; set; }
    }
}
