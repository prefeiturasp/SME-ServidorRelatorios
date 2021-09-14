namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoConselhoClasseConsolidadoDto
    {
        public RelatorioAcompanhamentoConselhoClasseConsolidadoDto(int naoIniciado, int emAndamento, int concluido)
        {
            NaoIniciado = naoIniciado;
            EmAndamento = emAndamento;
            Concluido = concluido;
        }

        public int NaoIniciado { get; set; }
        public int EmAndamento { get; set; }
        public int Concluido { get; set; }
    }
}
