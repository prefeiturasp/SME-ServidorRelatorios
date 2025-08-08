using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosSintetico
{
    public class GerarRelatorioControleLivrosEmprestadosSinteticoCommand : IRequest<string>
    {
        public GerarRelatorioControleLivrosEmprestadosSinteticoCommand(FiltroRelatorioControleLivro filtros)
        {
            Filtros = filtros;
        }

        public FiltroRelatorioControleLivro Filtros { get; set; }
    }
}
