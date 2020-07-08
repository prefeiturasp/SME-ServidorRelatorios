using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAtaFinalCabecalhoQuery: IRequest<ConselhoClasseAtaFinalCabecalhoDto>
    {
        public ObterAtaFinalCabecalhoQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }
    }
}
