using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemMatematicaConsolidadoUseCase : IRelatorioSondagemMatematicaConsolidadoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemMatematicaConsolidadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
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

            if (filtros.DreCodigo > 0)
            {
                dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreCodigo.ToString() });
                if (dre == null)
                    throw new NegocioException("Não foi possível obter a DRE.");
            }


            if (!string.IsNullOrEmpty(filtros.UsuarioRf))
            {
                usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRf });
                if (usuario == null)
                    throw new NegocioException("Não foi possível obter o usuário.");
            }
            var dataReferencia = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(filtros.Semestre, filtros.AnoLetivo));
            
            var quantidadeTotalAlunosUeAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(filtros.Ano, ue?.Codigo, filtros.AnoLetivo, dataReferencia, filtros.DreCodigo));

            var relatorio = await mediator.Send(new ObterSondagemMatNumAutoralConsolidadoQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                Dre = dre,
                Ue = ue,
                Semestre = filtros.Semestre,
                TurmaAno = int.Parse(filtros.Ano),
                Usuario = usuario,
                QuantidadeTotalAlunos = quantidadeTotalAlunosUeAno
            });


            string mensagemDaNotificacao, mensagemTitulo;

            if (!string.IsNullOrEmpty(filtros.UeCodigo))
            {
                mensagemDaNotificacao = $"O Relatório de Sondagem de Matemática ({relatorio.Proficiencia}) do {relatorio.Ano}º ano da {relatorio.Ue} ({relatorio.Dre})";
                mensagemTitulo = $"Relatório de Sondagem (Matemática) - {relatorio.Ue} ({relatorio.Dre}) - {relatorio.Ano}º ano";
            }
                
            else if (filtros.DreCodigo > 0)
            {
                mensagemDaNotificacao = $"O Relatório de Sondagem de Matemática ({relatorio.Proficiencia}) do {relatorio.Ano}º ano da {relatorio.Dre}";
                mensagemTitulo = $"Relatório de Sondagem (Matemática) - {relatorio.Dre.Replace("-", "")} - {relatorio.Ano}º ano";
            } else
            {
                mensagemDaNotificacao = $"O Relatório de Sondagem de Matemática ({relatorio.Proficiencia}) do {relatorio.Ano}º ano da SME";
                mensagemTitulo = $"Relatório de Sondagem (Matemática) - SME - {relatorio.Ano}º ano";
            }            

            return (await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidado", relatorio, Guid.NewGuid(), mensagemDaNotificacao, mensagemTitulo)));
        }
    }
}
