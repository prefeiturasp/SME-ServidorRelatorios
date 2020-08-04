using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioRecuperacaoParalelaAlunoDto
    {
        public RelatorioRecuperacaoParalelaAlunoDto(string alunoNome, string turma, string dataNascimento, string codigoEol, string turmaRegular, string situacao,
            string historico, string dificuldades, string encaminhamentos, string avancos, string outros)
        {
            AlunoNome = alunoNome;
            Turma = turma;
            DataNascimento = dataNascimento;
            CodigoEol = codigoEol;
            TurmaRegular = turmaRegular;
            Situacao = situacao;
            Historico = historico;
            Dificuldades = dificuldades;
            Encaminhamentos = encaminhamentos;
            Avancos = avancos;
            Outros = outros;
        }

        public string AlunoNome { get; set; }
        public string Turma { get; set; }
        public string DataNascimento { get; set; }
        public string CodigoEol { get; set; }
        public string TurmaRegular { get; set; }
        public string Situacao { get; set; }
        public string Historico { get; set; }
        public string Dificuldades { get; set; }
        public string Encaminhamentos { get; set; }
        public string Avancos { get; set; }
        public string Outros { get; set; }

        public string[] HistoricoArray { get { return SplitInParts(LimparString(Historico)).ToArray(); } }
        public string[] DificuldadesArray { get { return SplitInParts(LimparString(Dificuldades)).ToArray(); } }
        public string[] EncaminhamentosArray { get { return SplitInParts(LimparString(Encaminhamentos)).ToArray(); } }
        public string[] AvancosArray { get { return SplitInParts(LimparString(Avancos)).ToArray(); } }
        public string[] OutrosArray { get { return SplitInParts(LimparString(Outros)).ToArray(); } }

        public static string LimparString(string str)
        {
            str = str.ToLower().Replace("<br>", " ");
            str = str.ToLower().Replace("<p>", "");
            str = str.ToLower().Replace("</p>", "");
            return str;
        }

        public IEnumerable<string> SplitInParts(string s)
        {
            for (var i = 0; i < s.Length; i += 124)
                yield return s.Substring(i, Math.Min(124, s.Length - i));
        }

    }
}