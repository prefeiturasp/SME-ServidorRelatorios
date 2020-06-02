using System;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Workers.SGP.Commands;

namespace SME.SR.Workers.SGP.UseCases
{
    public class RelatorioDiarioDeClasseUseCase
    {
        public static async Task<bool> Executar(IMediator mediator)
        {
            try
            {
                return await mediator.Send(new RelatorioDiarioDeClasseCommand(
                    "filtroExemplo"
                ));
            }
            catch (Exception ex)
            {
                // TODO Add sentry
                // SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
