using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoExcelDto
    {
        public string Dre { get; set; }
        public string DreSigla { get; set; }
        public string DescricaoTipoSondagem { get; set; }
        public DataTable TabelaDeDado { get; set; }
        public int Periodo { get; set; }
        public int AnoLetivo { get; set; }
        public List<MergeColunaDto> MergeColunas { get; set; }
    }
}
