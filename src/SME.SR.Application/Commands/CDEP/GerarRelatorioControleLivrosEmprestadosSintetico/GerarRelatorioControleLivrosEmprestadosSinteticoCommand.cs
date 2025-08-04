using MediatR;

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
