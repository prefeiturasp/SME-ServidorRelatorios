using MediatR;
using SME.SR.Application.Queries;
using SME.SR.Data;
using SME.SR.Infra;
using System;
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

            PeriodoCompletoSondagemDto periodo;

            if ((filtros.AnoLetivo < 2022 || filtros.AnoLetivo >= 2023) && filtros.Semestre > 0)
                periodo = await mediator.Send(new ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery(filtros.Semestre, filtros.AnoLetivo));
            else 
                periodo = await mediator.Send(new ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery(filtros.Bimestre, filtros.AnoLetivo));

            var quantidadeTotalAlunosUeAno = await mediator.Send(new ObterTotalAlunosPorUeAnoSondagemQuery(filtros.Ano, ue?.Codigo, filtros.AnoLetivo, periodo.PeriodoFim, filtros.DreCodigo, filtros.Modalidades, true, periodo.PeriodoInicio));

            var relatorio = await mediator.Send(new ObterSondagemMatNumAutoralConsolidadoQuery()
            {
                AnoLetivo = filtros.AnoLetivo,
                Dre = dre,
                Ue = ue,
                Semestre = filtros.Semestre,
                Bimestre = filtros.Bimestre,
                TurmaAno = int.Parse(filtros.Ano),
                Usuario = usuario,
                QuantidadeTotalAlunos = quantidadeTotalAlunosUeAno
            });

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidado", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }
    }
}
