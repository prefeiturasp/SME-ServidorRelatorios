using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Codaf
{
    public class GrupoAlunosRelatorioCodafDto
    {
        public string TituloBloco { get; set; } // Ex: "4.1 ALUNOS APROVADOS - REDE MUNICIPAL"
        public List<AlunoRelatorioCodafDto> Alunos { get; set; }
    }
}
