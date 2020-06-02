using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SME.SR.Workers.SGP.Commands
{
    public class RelatorioDiarioDeClasseCommand : IRequest<bool>
    {
        public string FiltroExemplo { get; set; }

        public RelatorioDiarioDeClasseCommand(string filtroExemplo)
        {
            this.FiltroExemplo = filtroExemplo;
        }
    }

    public class RelatorioDiarioDeClasseCommandHandler : IRequestHandler<RelatorioDiarioDeClasseCommand, bool>
    {
        public RelatorioDiarioDeClasseCommandHandler()
        {

        }

        public async Task<bool> Handle(RelatorioDiarioDeClasseCommand request, CancellationToken cancellationToken)
        {
            // TODO Call report service
            return true;
        }
    }
}
