using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjProfessorDto
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string NomeProfessorCj { get; set; }
        public string ComponenteCurricular { get; set; }
        public string NomeProfessorTitular { get; set; }
        public string DataAtribuicao { get; set; }
        public string TipoProfessorCj { get; set; }
        public List<AtribuicaoCjAulaDto> Aulas { get; set; }
    }
}
