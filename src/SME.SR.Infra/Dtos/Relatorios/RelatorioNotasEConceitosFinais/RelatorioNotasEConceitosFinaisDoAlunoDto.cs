namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisDoAlunoDto
    {
        public RelatorioNotasEConceitosFinaisDoAlunoDto(string turmaNome, string alunoNumeroChamada, string alunoNomeCompleto, string notaConceito)
        {
            TurmaNome = turmaNome;
            AlunoNumeroChamada = alunoNumeroChamada;
            AlunoNomeCompleto = alunoNomeCompleto;
            NotaConceito = notaConceito;
        }

        public string TurmaNome { get; set; }
         
        public string AlunoNumeroChamada { get; set; }
        public string AlunoNomeCompleto { get; set; }
        public string NotaConceito { get; set; }
    }
}
