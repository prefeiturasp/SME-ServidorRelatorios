using System;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ConsultaRelatorioFrequenciaControleMensalDto
    {
        public string DisciplinaId { get; set; }
        public string NomeComponente { get; set; }
        public int TotalAula { get; set; }
        public int TotalTipoFrequencia { get; set; }
        public int TipoFrequencia { get; set; }
        public DateTime DataAula { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public string DiaSemana { get; set; }
        public int? TotalCompensacao { get; set; }
        public DateTime DataCompensacao { get; set; }
        public int OrdemExibicaoComponente { get; set; }
    }
}