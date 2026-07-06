using MediatR;

namespace SME.SR.Application.Commands.Conecta.GerarPlanilhaCodafSuplementar
{
    public class GerarPlanilhaCodafSuplementarCommand : IRequest<byte[]>
    {
        public long CodafListaPresencaId { get; set; }

        public GerarPlanilhaCodafSuplementarCommand(long codafListaPresencaId)
        {
            CodafListaPresencaId = codafListaPresencaId;
        }
    }
}