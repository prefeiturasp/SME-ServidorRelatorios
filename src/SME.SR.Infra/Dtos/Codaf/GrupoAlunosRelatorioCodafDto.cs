using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Codaf
{
    public class GrupoAlunosRelatorioCodafDto
    {
        public string TituloBloco { get; set; }
        public bool EhRedeParceira { get; set; }
        public List<AlunoRelatorioCodafDto> Alunos { get; set; }
    }
}
