using System.Collections.Generic;
using System.Data;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class RelatorioControleFrequenciaMensalExcelDto
    {
        public int Ano { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string NomeCriancaEstudante { get; set; }
        public string CodigoCriancaEstudante { get; set; }
        public string Usuario { get; set; }
        public string DataImpressao { get; set; }
        public List<FrequenciaPorMesExcelDto> FrequenciasMeses { get; set; }
    }

    public class FrequenciaPorMesExcelDto
    {
        public string Mes { get; set; }
        public string FrequenciaGlobal { get; set; }
        public DataTable TabelaDeDado { get; set; }
        public IEnumerable<string> ColunasDiasNaoLetivosFinaisSemana { get; set; }
    }
}
