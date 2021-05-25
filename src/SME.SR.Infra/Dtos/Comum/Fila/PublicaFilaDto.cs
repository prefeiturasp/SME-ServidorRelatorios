using System;

namespace SME.SR.Infra
{
    public class PublicaFilaDto
    {
        public PublicaFilaDto(object dados, string nomeFila, string rota, string exchange = null, Guid codigoCorrelacao = default, string codigoRfUsuario = default)
        {
            Dados = dados;

            Rota = rota;
            if (!string.IsNullOrWhiteSpace(exchange))
                Exchange = exchange;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioLogadoRF = codigoRfUsuario;
        }

        public string NomeFila { get; set; }
        public object Dados { get; set; }
        public string Rota { get; }
        public string Exchange { get; set; }
        public string UsuarioLogadoRF { get; }

        public Guid CodigoCorrelacao { get; set; }
    }
}
