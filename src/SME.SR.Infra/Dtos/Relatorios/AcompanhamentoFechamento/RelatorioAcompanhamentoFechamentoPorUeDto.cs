using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoPorUeDto
    {
        public RelatorioAcompanhamentoFechamentoPorUeDto()
        {
            Turmas = new List<RelatorioAcompanhamentoFechamentoTurmaDto>();
        }

        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Turma { get; set; }
        public string Bimestre { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Data { get; set; }

        public List<RelatorioAcompanhamentoFechamentoTurmaDto> Turmas { get; set; }

    }
}
