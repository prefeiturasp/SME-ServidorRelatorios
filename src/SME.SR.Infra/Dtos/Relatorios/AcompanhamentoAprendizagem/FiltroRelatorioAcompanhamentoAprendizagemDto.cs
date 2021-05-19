namespace SME.SR.Infra
{
    public class FiltroRelatorioAcompanhamentoAprendizagemDto
    {
        public long TurmaId { get; set; }
        public long? AlunoCodigo { get; set; }
        public int Semestre { get; set; }
        public string SemestreFormatado(int anoLetivo)
        {
            if (Semestre == 0)
                return string.Empty;

            return $"{Semestre}º SEMESTRE {anoLetivo}";
        }
    }
}
