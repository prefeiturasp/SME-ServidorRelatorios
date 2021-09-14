using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterParametroSistemaPorTipoAnoQueryHandler : IRequestHandler<ObterParametroSistemaPorTipoAnoQuery, string>
    {
        private readonly IParametroSistemaRepository repositorio;

        public ObterParametroSistemaPorTipoAnoQueryHandler(IParametroSistemaRepository repositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public async Task<string> Handle(ObterParametroSistemaPorTipoAnoQuery request, CancellationToken cancellationToken)
        {
            return await repositorio.ObterValorPorAnoTipo(request.Ano, request.Tipo);
        }
    }
}
