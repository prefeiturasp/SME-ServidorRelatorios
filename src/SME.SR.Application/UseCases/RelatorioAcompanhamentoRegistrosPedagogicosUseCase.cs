using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosUseCase : IRelatorioAcompanhamentoRegistrosPedagogicosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoRegistrosPedagogicosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroRelatorioAcompanhamentoRegistrosPedagogicos;

            var relatorioQuery = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosQuery>();

            if (relatorioQuery.Modalidade == Modalidade.Infantil)
            {
                // faz a query de consolidação infantil e monta o relatório
                //await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicos", relatorioDto, request.CodigoCorrelacao));
            }
            else
            {
                // faz a query de consolidação dos restantes
                //await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicos", relatorioDto, request.CodigoCorrelacao));
            }


        }
    }
}
