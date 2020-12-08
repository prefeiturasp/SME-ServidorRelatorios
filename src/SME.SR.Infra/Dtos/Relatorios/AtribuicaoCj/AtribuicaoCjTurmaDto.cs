using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjTurmaDto
    {
        public string NomeTurma { get; set; }
        public string ComponenteCurricular { get; set; }
        public string NomeProfessorTitular { get; set; }
        public string DataAtribuicao { get; set; }
        public List<AtribuicaoCjAulaDto> Aulas { get; set; }
    }
}
