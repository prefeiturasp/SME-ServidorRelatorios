using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAtribuicaoCjDto
    {
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Modalidade { get; set; }
        public string Semestre { get; set; }
        public string Turma { get; set; }
        public string Professor { get; set; }
        public string Usuario { get; set; }
        public string DataImpressao { get { return DateTime.Today.ToString("dd/MM/yyyy"); } }
        public List<AtribuicaoCjPorTurmaDto> AtribuicoesCjPorTurma { get; set; }
        public List<AtribuicaoCjPorProfessorDto> AtribuicoesCjPorProfessor { get; set; }
        public List<AtribuicaoEsporadicaDto> AtribuicoesEsporadicas { get; set; }

        public RelatorioAtribuicaoCjDto()
        {
            AtribuicoesCjPorTurma = new List<AtribuicaoCjPorTurmaDto>();
            AtribuicoesCjPorProfessor = new List<AtribuicaoCjPorProfessorDto>();
            AtribuicoesEsporadicas = new List<AtribuicaoEsporadicaDto>();
        }
    }
}
