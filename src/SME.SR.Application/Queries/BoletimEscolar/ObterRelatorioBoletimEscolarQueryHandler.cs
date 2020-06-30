using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, RelatorioBoletimEscolarDto>
    {
        private IMediator _mediator;

        public ObterRelatorioBoletimEscolarQueryHandler(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<RelatorioBoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            var dre = await ObterDrePorCodigo(request.DreCodigo);
            var ue = await ObterUePorCodigo(request.UeCodigo);

            return new RelatorioBoletimEscolarDto(new BoletimEscolarDto());
        }

        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await _mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await _mediator.Send(new ObterUePorCodigoQuery()
            {
                UeCodigo = ueCodigo
            });
        }
    }
}
