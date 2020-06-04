using System;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Workers.SGP.Commands;

namespace SME.SR.Workers.SGP.UseCases
{
    public class RelatorioDadosAlunoUseCase
    {
        public static async Task<bool> Executar(IMediator mediator)
        {
            try
            {
                return await mediator.Send(new RelatorioDadosAlunoCommand(
                    "filtroExemplo"
                ));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
