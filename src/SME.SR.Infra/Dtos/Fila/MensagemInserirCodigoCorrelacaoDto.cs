namespace SME.SR.Infra
{
    public class MensagemInserirCodigoCorrelacaoDto
    {
        public MensagemInserirCodigoCorrelacaoDto(TipoRelatorio tipoRelatorio, TipoFormatoRelatorio formato)
        {
            TipoRelatorio = tipoRelatorio;
            Formato = formato;
        }

        public TipoRelatorio TipoRelatorio { get; set; }
        public TipoFormatoRelatorio Formato { get; set; }
    }
}
