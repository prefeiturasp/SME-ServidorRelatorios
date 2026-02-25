using System;

namespace SME.SR.Infra.Dtos.NovoSondagem
{
    public class FiltroRelatorioSondagemQuestionarioDto
    {
        public string DreNome { get; set; } = string.Empty;
        public string UeNome { get; set; } = string.Empty;
        public string TurmaNome { get; set; } = string.Empty;
        public string ModalidadeNome { get; set; } = string.Empty;
        public int TurmaId { get; set; }
        public int ProficienciaId { get; set; }
        public int ComponenteCurricularId { get; set; }
        public int Modalidade { get; set; }
        public int Ano { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public string UeCodigo { get; set; } = string.Empty;
        public int? BimestreId { get; set; }
    }
}