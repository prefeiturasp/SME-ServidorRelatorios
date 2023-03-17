using System;

namespace SME.SR.Infra
{
    public class DetalheEncaminhamentoNAAPADto
    {
        public string Aluno { get; set; }
        public string Turma { get; set; }
        public DateTime? DataEntradaQueixa { get; set; }
        public string PortaEntrada { get; set; }
        public string FluxosAlerta { get; set; }
        public DateTime? DataUltimoAtendimento { get; set; }
        public string Situacao { get; set; }

    }
}
