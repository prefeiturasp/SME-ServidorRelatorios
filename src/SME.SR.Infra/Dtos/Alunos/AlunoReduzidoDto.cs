namespace SME.SR.Infra
{
    public class AlunoReduzidoDto
    {
        public int TurmaCodigo { get; set; }
        public int AlunoCodigo { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }

        public string NomeRelatorio =>
             $"{(NomeSocialAluno ?? NomeAluno)} ({AlunoCodigo})".ToUpper();
    }
}
