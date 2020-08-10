namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisDoAlunoDto
    {
        public RelatorioNotasEConceitosFinaisDoAlunoDto(string turmaNome, string numeroChamadaAluno, string nomeCompletoAluno, string notaConceito)
        {
            TurmaNome = turmaNome;
            NumeroChamadaAluno = numeroChamadaAluno;
            NomeCompletoAluno = nomeCompletoAluno;
            NotaConceito = notaConceito;
        }

        public string TurmaNome { get; set; }
        public string NumeroChamadaAluno { get; set; }
        public string NomeCompletoAluno { get; set; }
        public string NotaConceito { get; set; }
    }
}
