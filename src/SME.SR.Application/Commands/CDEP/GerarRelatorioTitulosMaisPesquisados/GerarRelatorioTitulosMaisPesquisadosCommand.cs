using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioTitulosMaisPesquisados
{
    public class GerarRelatorioTitulosMaisPesquisadosCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioTitulosMaisPesquisadosCommand(FiltroRelatorioTitulosMaisPesquisados filtros)
        {
            Filtros = filtros;
        }
        public FiltroRelatorioTitulosMaisPesquisados Filtros { get; set; }
    }
}