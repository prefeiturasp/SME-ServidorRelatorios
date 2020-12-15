using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class AgruparRelatorioAtribuicaoCJPorDreUeQuery : IRequest<RelatorioAtribuicaoCjDto>
    {
        public RelatorioAtribuicaoCjDto RelatorioAtribuicaoCJ { get; set; }

        public AgruparRelatorioAtribuicaoCJPorDreUeQuery(RelatorioAtribuicaoCjDto relatorioAtribuicaoCJ)
        {
            RelatorioAtribuicaoCJ = relatorioAtribuicaoCJ;
        }
    }
}
