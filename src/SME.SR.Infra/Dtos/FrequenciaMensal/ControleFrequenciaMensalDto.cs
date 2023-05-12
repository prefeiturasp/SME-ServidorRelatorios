using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaMensalDto
    {
        public ControleFrequenciaMensalDto()
        {
            FrequenciaMes = new List<ControleFrequenciaPorMesDto>();
        }
        public int Ano { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public string NomeCriancaEstudante { get; set; }
        public string CodigoCriancaEstudante { get; set; }
        public string Usuario { get; set; }
        public string FrequenciaGlobal { get; set; }
        public string DataImpressao { get; set; }
        public List<ControleFrequenciaPorMesDto> FrequenciaMes { get; set; } 
    }
}