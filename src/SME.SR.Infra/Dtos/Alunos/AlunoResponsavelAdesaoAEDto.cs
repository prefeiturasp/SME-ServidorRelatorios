namespace SME.SR.Infra
{
    public class AlunoResponsavelAdesaoAEDto
    {
        public long TurmaCodigo { get; set; }
        public long AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public string AlunoNomeSocial { get; set; }
        public string AlunoNumeroChamada { get; set; }
        public string ResponsavelCpf { get; set; }
        public string ResponsavelNome { get; set; }
        public string ResponsavelDDD { get; set; }
        public string ResponsavelCelular { get; set; }
        public string NomeAlunoParaVisualizar()
        {
            if (string.IsNullOrEmpty(AlunoNomeSocial))
                return AlunoNome;
            else return AlunoNomeSocial;
        }
        public string ResponsavelCelularFormatado()
        {
            return $"({ResponsavelDDD} {ResponsavelCelular})";
        }

    }
}
