using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDto
    {
        public string Nome { get; set; }
        public List<string> Professores { get; set; }
        public int Aulas { get; set; }
        public int FrequenciasPendentes { get; set; }
        public string DataUltimoRegistroFrequencia { get; set; }
        public int DiarioBordoPendentes { get; set; }
        public string DataUltimoRegistroDiarioBordo { get; set; }
    }
}
