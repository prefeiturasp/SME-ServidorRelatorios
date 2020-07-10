using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class AlunoTurmasHistoricoEscolarDto
    {
        public AlunoTurmasHistoricoEscolarDto()
        {
            Turmas = new List<Turma>();
        }
        public InformacoesAlunoDto Aluno { get; set; }
        public List<Turma> Turmas { get; set; }
}
}
