using MediatR;
using System;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico
{
    public class GerarRelatorioControleLivrosEmprestadosAnaliticoCommand : IRequest<string>
    {
        public GerarRelatorioControleLivrosEmprestadosAnaliticoCommand(FiltroRelatorioControleLivro filtro)
        {
            Filtros = filtro ?? throw new ArgumentNullException(nameof(filtro));
        }
        public FiltroRelatorioControleLivro Filtros { get; }
    }
}
