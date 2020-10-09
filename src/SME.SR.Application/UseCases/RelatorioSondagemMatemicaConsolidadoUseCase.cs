using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemMatemicaConsolidadoUseCase : IRelatorioSondagemMatemicaConsolidadoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemMatemicaConsolidadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto>();

            Dre dre = null;
            Ue ue = null;
            Usuario usuario = null;

            if (!string.IsNullOrEmpty(filtros.UeCodigo))
            {
                ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeCodigo));
                if (ue == null)
                    throw new NegocioException("Não foi possível obter a UE.");
            }

            if (!string.IsNullOrEmpty(filtros.DreCodigo))
            {
                dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo });
                if (dre == null)
                    throw new NegocioException("Não foi possível obter a DRE.");
            }


            if (!string.IsNullOrEmpty(filtros.UsuarioRf))
            {
                usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRf });
                if (usuario == null)
                    throw new NegocioException("Não foi possível obter o usuário.");
            }

            var relatorio = await mediator.Send(new ObterRelatorioSondagemMatematicaNumerosAutoralConsolidadoQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                Dre = dre,
                Ue = ue,
                Semestre = filtros.Semestre,
                TurmaAno = filtros.Ano,
                Usuario = usuario
            });

            var mensagemDaNotificacao = $"O Relatório de Sondagem de Matemática ({relatorio.Proficiencia}) do {relatorio.Ano}º ano da {relatorio.Ue} ({relatorio.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem (Matemática) - {relatorio.Ue} ({relatorio.Dre}) - {relatorio.Ano}º ano";

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidado", relatorio, request.CodigoCorrelacao, mensagemDaNotificacao, mensagemTitulo));
        }
    }
}
