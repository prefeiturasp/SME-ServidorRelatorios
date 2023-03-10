using System;

namespace SME.SR.Infra
{
    public class CabecalhoEncaminhamentoNAAPADetalhadoDto
    {
        public int AnoLetivo { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string TurmaNome { get; set; }
        public string Aluno { get; set; }
        public string SituacaoEncaminhamento { get; set; }
        public string UsuarioNome { get; set; }
        public string DataCriacao { get; set; }
        public string DataNascimento { get; set; }
        public string DataImpressao { get; set; }
    }
}
