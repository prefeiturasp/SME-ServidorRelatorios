using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaMensalDto
    {
        public int Ano { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string Nome_Crianca_Estudante { get; set; }
        public string Codigo_Crianca_Estudante { get; set; }
        public string Usuario { get; set; }
        public string FrequenciaGlobal { get; set; }
        public List<ControleFrequenciaPorMesDto> FrequenciaMes { get; set; } 
    }
}