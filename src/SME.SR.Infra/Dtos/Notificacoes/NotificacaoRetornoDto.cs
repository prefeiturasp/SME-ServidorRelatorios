using System;

namespace SME.SR.Infra
{
    public class NotificacaoRetornoDto
    {
        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public NotificacaoCategoria Categoria { get; set; }
        public NotificacaoTipo Tipo { get; set; }
        public NotificacaoStatus Situacao { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime? DataLeitura { get; set; }
    }
}
