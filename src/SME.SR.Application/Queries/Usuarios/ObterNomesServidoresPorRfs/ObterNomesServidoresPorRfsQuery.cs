using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNomesServidoresPorRfsQuery : IRequest<IEnumerable<Funcionario>>
    {
        public ObterNomesServidoresPorRfsQuery(string[] codigosRfs)
        {
            CodigosRfs = codigosRfs;
        }

        public string[] CodigosRfs { get; set; }
    }
}
