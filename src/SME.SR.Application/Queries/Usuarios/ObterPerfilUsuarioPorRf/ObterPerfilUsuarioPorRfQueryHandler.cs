using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPerfilUsuarioPorRfQueryHandler : IRequestHandler<ObterPerfilUsuarioPorRfQuery, IEnumerable<PerfilUsuarioDto>>
    {
        private readonly Iperfilrepository
    }
}
