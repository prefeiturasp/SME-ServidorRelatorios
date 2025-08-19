using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleDevolucao
{
    public class GerarRelatorioControleDevolucaoCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioControleDevolucaoCommand(FiltroRelatorioControleDevolucaoLivro filtros)
        {
            Filtros = filtros;
        }
        public FiltroRelatorioControleDevolucaoLivro Filtros { get; set; }
    }
}
