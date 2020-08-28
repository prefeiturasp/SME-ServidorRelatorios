using Microsoft.VisualBasic;
using SME.SR.Infra;
using System;

namespace SME.SR.Data
{
    public class AlunoHistoricoEscolar : Aluno
    {
        public string CidadeNatal { get; set; }
        public string EstadoNatal { get; set; }
        public string Nacionalidade { get; set; }
        public string RG { get; set; }
        public string ExpedicaoOrgaoEmissor { get; set; }
        public string ExpedicaoUF { get; set; }
        public DateTime ExpedicaoData { get; set; }
    }
}
