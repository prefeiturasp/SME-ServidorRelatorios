using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomesUsuariosPorRfsQuery : IRequest<IEnumerable<Usuario>>
    {
        public ObterNomesUsuariosPorRfsQuery(string[] codigosRfs)
        {
            CodigosRfs = codigosRfs;
        }

        public string[] CodigosRfs { get; set; }
    }
}
