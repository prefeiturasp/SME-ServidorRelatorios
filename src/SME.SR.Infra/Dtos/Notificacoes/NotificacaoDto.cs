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
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}
