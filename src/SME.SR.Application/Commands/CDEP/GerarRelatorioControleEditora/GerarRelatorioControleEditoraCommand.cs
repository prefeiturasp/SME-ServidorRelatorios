using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleEditora
{
    public class GerarRelatorioControleEditoraCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioControleEditoraCommand(FiltroRelatorioControleEditora filtros)
        {
            Filtros = filtros;
        }
        public FiltroRelatorioControleEditora Filtros { get; set; }
    }
}
