using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervo
{
    public class GerarRelatorioControleAcervoCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioControleAcervoCommand(FiltroRelatorioControleAcervo filtros)
        {
            Filtros = filtros;
        }
        public FiltroRelatorioControleAcervo Filtros { get; set; }
    }
}
