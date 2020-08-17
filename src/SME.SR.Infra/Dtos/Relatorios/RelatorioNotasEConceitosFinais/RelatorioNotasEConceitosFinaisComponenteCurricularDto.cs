using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisComponenteCurricularDto
    {
        public RelatorioNotasEConceitosFinaisComponenteCurricularDto(string nome)
        {
            Nome = nome;
            NotaConceitoAlunos = new List<RelatorioNotasEConceitosFinaisDoAlunoDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioNotasEConceitosFinaisDoAlunoDto> NotaConceitoAlunos { get; set; }
    }
}
