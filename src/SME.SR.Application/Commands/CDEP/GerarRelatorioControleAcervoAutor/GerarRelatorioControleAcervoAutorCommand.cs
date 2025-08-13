using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervoAutor
{
    public class GerarRelatorioControleAcervoAutorCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioControleAcervoAutorCommand(FiltroRelatorioControleAcervoAutor filtros)
        {
            Filtros = filtros;
        }

        public FiltroRelatorioControleAcervoAutor Filtros { get; set; }
    }
}
