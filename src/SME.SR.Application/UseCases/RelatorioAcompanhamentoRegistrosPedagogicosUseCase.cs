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
                var relatorioQueryInfantil = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery>();
                var relatorioInfantilDto = await mediator.Send(relatorioQueryInfantil);
                if (relatorioInfantilDto.Bimestre.Any())
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicosInfantil", relatorioInfantilDto, request.CodigoCorrelacao, "", "Relatório de Acompanhamento de registros pedagógicos", true, "RELATÓRIO DE ACOMPANHAMENTO DOS REGISTROS PEDAGÓGIGOS"));
                else    
                    throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");     
            }
            else
            {
                var relatorioDto = await mediator.Send(relatorioQuery);
                if (relatorioDto.Bimestre.Any())
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicos", relatorioDto, request.CodigoCorrelacao,"", "Relatório de Acompanhamento de registros pedagógicos",true,"RELATÓRIO DE ACOMPANHAMENTO DOS REGISTROS PEDAGÓGICOS"));
                else
                    throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
            }
        }
    }
}
