using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFuncionarioUePorCargoQuery : IRequest<IEnumerable<FuncionarioDto>>
    {
        public string CodigoUe { get; set; }

        public string CodigoCargo { get; set; }
    }
}
