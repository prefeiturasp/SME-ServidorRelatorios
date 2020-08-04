using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioRecuperacaoParalelaAlunoDto
    {
        public RelatorioRecuperacaoParalelaAlunoDto(string alunoNome, string turma, string dataNascimento, string codigoEol, string turmaRegular, string situacao)
        {
            AlunoNome = alunoNome;
            Turma = turma;
            DataNascimento = dataNascimento;
            CodigoEol = codigoEol;
            TurmaRegular = turmaRegular;
            Situacao = situacao;
            Secoes = new List<RelatorioRecuperacaoParalelaAlunoSecaoDto>();
        }

        public string AlunoNome { get; set; }
        public string Turma { get; set; }
        public string DataNascimento { get; set; }
        public string CodigoEol { get; set; }
        public string TurmaRegular { get; set; }
        public string Situacao { get; set; }

        public List<RelatorioRecuperacaoParalelaAlunoSecaoDto> Secoes { get; set; }

    }
}