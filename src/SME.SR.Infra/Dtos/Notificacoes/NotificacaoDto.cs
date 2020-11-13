using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class NotificacaoDto
    {
        public NotificacaoDto()
        {
        }

        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public NotificacaoCategoria Categoria { get; set; }
        public NotificacaoTipo Tipo { get; set; }
        public NotificacaoStatus Situacao { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime DataLeitura { get; set; }
        public long DreId { get; set; }
        public string DreNome { get; set; }
        public long UeId { get; set; }
        public string UeNome { get; set; }
    }
}
