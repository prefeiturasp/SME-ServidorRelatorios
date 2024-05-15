using MediatR;
using SME.SR.Data.Models.Conecta;
using SME.SR.Infra.Dtos.Relatorios.Conecta;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFLaudaCompletaCommand : IRequest<string>
    {
        public GerarRelatorioHtmlPDFLaudaCompletaCommand(RelatorioPaginadoLaudaCompletaDto relatorioPaginado)
        {
            RelatorioPaginado = relatorioPaginado;
        }

        public RelatorioPaginadoLaudaCompletaDto RelatorioPaginado { get; set; }
    }
}
