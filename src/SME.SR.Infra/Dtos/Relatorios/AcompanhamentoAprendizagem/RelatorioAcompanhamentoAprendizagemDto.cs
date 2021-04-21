using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoAprendizagemDto
    {
        public RelatorioAcompanhamentoAprendizagemDto()
        {
            Alunos = new List<RelatorioAcompanhamentoAprendizagemAlunoDto>();
        }
        public RelatorioAcompanhamentoAprendizagemCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoAprendizagemAlunoDto> Alunos { get; set; }

    }
}
