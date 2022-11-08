using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Application
{
    public class RelatorioPlanoAeeUseCase : IRelatorioPlanoAeeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanoAeeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            //var parametros = request.ObterObjetoFiltro<FiltroRelatorioPlanoAeeDto>();
            
            var filtroRelatorio = new FiltroRelatorioPlanoAeeDto()
            {
                VersaoPlanoId = 38532
            };            

            var relatorioPlanoAee = new RelatorioPlanoAeeDto();
            await ObterCabecalho(filtroRelatorio.VersaoPlanoId, relatorioPlanoAee);

            var questoes = await mediator.Send(new ObterQuestoesPlanoAEEPorVersaoPlanoIdQuery(filtroRelatorio.VersaoPlanoId));
            Console.WriteLine(questoes.ToString());
            //relatorioPlanoAee.Questoes = questoes;
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPlanoAee", relatorioPlanoAee, request.CodigoCorrelacao, diretorioComplementar: "planoaee"));
        }

        private async Task ObterCabecalho(long versaoPlanoId, RelatorioPlanoAeeDto relatorioPlanoAee)
        {
            var dadosPlanoAee = await mediator.Send(new ObterPlanoAEEPorVersaoPlanoIdQuery(versaoPlanoId));

            var descricaoModalidade = dadosPlanoAee.Modalidade.ShortName();
            var descricaoTipoEscola = dadosPlanoAee.TipoEscola.ShortName();

            relatorioPlanoAee.Cabecalho = new CabecalhoPlanoAeeDto
            {
                AlunoCodigo = dadosPlanoAee.AlunoCodigo,
                AlunoNome = dadosPlanoAee.AlunoNome,
                AnoLetivo = dadosPlanoAee.AnoLetivo,
                DreNome = dadosPlanoAee.DreAbreviacao,
                SituacaoPlano = dadosPlanoAee.SituacaoPlano.Name(),
                TurmaNome = string.Concat(descricaoModalidade, " ", dadosPlanoAee.UeNome),
                UeNome = string.Concat(descricaoTipoEscola, " ", dadosPlanoAee.UeNome),
                VersaoPlano = string.Concat("v", dadosPlanoAee.VersaoPlano.ToString(), " - ", dadosPlanoAee.DataVersaoPlano.ToString("dd/MM/yyyy"))
            };
        }
    }
}