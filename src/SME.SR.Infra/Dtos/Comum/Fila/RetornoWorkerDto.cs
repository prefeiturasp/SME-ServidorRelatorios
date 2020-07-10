using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RetornoWorkerDto
    {
        public RetornoWorkerDto(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Mensagem { get; set; }
    }
}
