using MediatR;

namespace SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf
{
    public class GerarPlanilhaCodafCommand : IRequest<byte[]>
    {
        public long CodafId { get; set; }

        public GerarPlanilhaCodafCommand(long codafId)
        {
            CodafId = codafId;
        }
    }
}