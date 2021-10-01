using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterArquivoPorCodigoQuery : IRequest<ArquivoDto>
    {
        public ObterArquivoPorCodigoQuery(Guid codigo)
        {
            Codigo = codigo;
        }

        public Guid Codigo { get; set; }
    }
}
