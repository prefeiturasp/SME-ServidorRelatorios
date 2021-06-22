namespace SME.SR.Infra
{
    public class AlunoReduzidoDto
    {
        public long TurmaCodigo { get; set; }
        public long AlunoCodigo { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }

        public string NomeRelatorio =>
             $"{(NomeSocialAluno ?? NomeAluno)} ({AlunoCodigo})".ToUpper();
    }
}
