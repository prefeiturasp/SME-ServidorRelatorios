using MediatR;
using SME.SR.Infra;
using System;
using System.Linq;
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
                var utilizarNovoLayout = (await mediator.Send(new ObterParametroSistemaPorTipoAnoQuery(relatorioQuery.AnoLetivo, TipoParametroSistema.Devolutiva)) != null);

                if (utilizarNovoLayout)
                {
                    var relatorioQueryInfantil = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteQuery>();

                    var relatorioInfantilComponenteDto = await mediator.Send(relatorioQueryInfantil);

                    if (relatorioInfantilComponenteDto.Bimestres.Any())
                    {
                        await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponente", relatorioInfantilComponenteDto,
                        request.CodigoCorrelacao, "", "Relatório de Acompanhamento de registros pedagógicos", true));
                    }
                    else
                    {
                        throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
                    }
                }
                else
                {
                    var relatorioQueryInfantil = request.ObterObjetoFiltro<ObterRelatorioAcompanhamentoRegistrosPedagogicosInfantilQuery>();
                    var relatorioInfantilDto = await mediator.Send(relatorioQueryInfantil);

                    if (relatorioInfantilDto.Bimestre.Any())
                    {
                        await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicosInfantil", relatorioInfantilDto,
                            request.CodigoCorrelacao, "", "Relatório de Acompanhamento de registros pedagógicos", true));
                    }
                    else
                    {
                        throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
                    }
                }
            }
            else
            {                
                var relatorioDto = await mediator.Send(relatorioQuery);

                if (relatorioDto.Bimestre.Any())
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoRegistrosPedagogicos", relatorioDto, request.CodigoCorrelacao,"", "Relatório de Acompanhamento de registros pedagógicos",true));
                else
                    throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
            }
        }
    }
}
