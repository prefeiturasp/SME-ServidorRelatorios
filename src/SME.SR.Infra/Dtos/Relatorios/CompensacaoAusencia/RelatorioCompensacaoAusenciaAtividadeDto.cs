using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaAtividadeDto
    {
        public RelatorioCompensacaoAusenciaAtividadeDto()
        {
            CompensacoesAluno = new List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioCompensacaoAusenciaCompensacaoAlunoDto> CompensacoesAluno { get; set; }

    }
}


