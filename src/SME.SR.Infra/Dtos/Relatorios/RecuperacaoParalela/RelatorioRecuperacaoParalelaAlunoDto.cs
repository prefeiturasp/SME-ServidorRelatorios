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
    }
}
