using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
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
