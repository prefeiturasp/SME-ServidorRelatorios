using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico
{
    public class GerarRelatorioControleLivrosEmprestadosAnaliticoCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioControleLivrosEmprestadosAnaliticoCommand(FiltroRelatorioControleLivro filtro)
        {
            Filtros = filtro ?? throw new ArgumentNullException(nameof(filtro));
        }
        public FiltroRelatorioControleLivro Filtros { get; }
    }
}
