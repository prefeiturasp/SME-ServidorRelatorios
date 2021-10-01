namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisDoAlunoDto
    {
        public RelatorioNotasEConceitosFinaisDoAlunoDto(string turmaNome, int alunoCodigo, string alunoNumeroChamada, string alunoNomeCompleto, string notaConceito)
        {
            TurmaNome = turmaNome;
            AlunoCodigo = alunoCodigo;
            AlunoNumeroChamada = alunoNumeroChamada;
            AlunoNomeCompleto = alunoNomeCompleto;
            NotaConceito = notaConceito;
            EmAprovacao = false;
        }

        public string TurmaNome { get; set; }
        public int AlunoCodigo { get; set; }
        public string AlunoNumeroChamada { get; set; }
        public string AlunoNomeCompleto { get; set; }
        public string NotaConceito { get; set; }
        public bool EmAprovacao { get; set; }
    }
}
