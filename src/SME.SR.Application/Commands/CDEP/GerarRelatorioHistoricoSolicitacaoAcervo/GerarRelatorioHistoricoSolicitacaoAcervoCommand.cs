using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.IO;

namespace SME.SR.Application.Commands.CDEP.GerarRelatorioHistoricoSolicitacaoAcervo
{
    public class GerarRelatorioHistoricoSolicitacaoAcervoCommand : IRequest<MemoryStream>
    {
        public GerarRelatorioHistoricoSolicitacaoAcervoCommand(FiltroRelatorioHistoricoSolicitacaoAcervo filtros)
        {
            Filtros = filtros;
        }
        public FiltroRelatorioHistoricoSolicitacaoAcervo Filtros { get; set; }
    }
}
