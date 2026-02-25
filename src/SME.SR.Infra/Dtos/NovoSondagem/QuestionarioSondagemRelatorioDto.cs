using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.NovoSondagem
{
    public class QuestionarioSondagemRelatorioDto
    {
        public int AnoLetivo { get; set; }
        public string Dre { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public string Turma { get; set; } = string.Empty;
        public string UnidadeEducacional { get; set; } = string.Empty;
        public string Modalidade { get; set; } = string.Empty;
        public string Proficiencia { get; set; } = string.Empty;
        public DateTime DataImpressao { get; set; } = DateTime.Now;
        public string Usuario { get; set; } = string.Empty;

        public string TituloTabelaRespostas { get; set; } = string.Empty;

        public IEnumerable<EstudanteQuestionarioDto>? Estudantes { get; set; }
    }
}