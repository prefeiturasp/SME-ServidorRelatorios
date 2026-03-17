using System;

namespace SME.SR.Infra
{
    public class MensagemInserirCodigoCorrelacaoDto
    {
        public MensagemInserirCodigoCorrelacaoDto(Guid? codigoCorrelacao, string usuarioRf, TipoRelatorio tipoRelatorio, TipoFormatoRelatorio tipoFormatoRelatorio)
        {
            CodigoCorrelacao = codigoCorrelacao ?? Guid.Empty;
            UsuarioRf = usuarioRf;
            TipoRelatorio = tipoRelatorio;
            Formato = tipoFormatoRelatorio;
        }

        public MensagemInserirCodigoCorrelacaoDto(TipoRelatorio tipoRelatorio, TipoFormatoRelatorio tipoFormatoRelatorio)
        {
            TipoRelatorio = tipoRelatorio;
            Formato = tipoFormatoRelatorio;
        }

        public Guid? CodigoCorrelacao { get; set; } = Guid.Empty;
        public string UsuarioRf { get; set; } = string.Empty;
        public TipoRelatorio TipoRelatorio { get; set; }
        public TipoFormatoRelatorio Formato { get; set; }
    }
}
