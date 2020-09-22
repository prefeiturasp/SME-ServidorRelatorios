using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioResumoPAPUseCase : IRelatorioResumoPAPUseCase
    {
        private readonly IMediator mediator;

        public RelatorioResumoPAPUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioResumoPAPDto>();

            // Obter dados de dre e ue
            var dreUe = await ObterDadosDreUe(filtros);

            // Obter dados de dre e ue
            var turma = await ObterDadosTurma(filtros);

            var ciclo = await ObterCiclo(filtros);

            var periodo = await ObterPeriodo(filtros);

            var relatorioResumoPAPDto = new ResumoPAPDto();

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioRecuperacaoParalela", relatorioResumoPAPDto, request.CodigoCorrelacao));
        }

        private async Task<RecuperacaoParalelaPeriodoDto> ObterPeriodo(FiltroRelatorioResumoPAPDto filtros)
        {
            if (filtros.Periodo.HasValue && filtros.Periodo.Value > 0)
            {
                var periodo = await mediator.Send(new ObterRecuperacaoParalelaPeriodoPorIdQuery() { RecuperacaoParalelaPeriodoId = (long)filtros.Periodo.Value });

                if (periodo == null)
                    throw new NegocioException($"Não foi possível localizar dados do período");
                return periodo;
            }
            else
            {
                return new RecuperacaoParalelaPeriodoDto() { Nome = "Todos" };
            }
        }

        private async Task<TipoCiclo> ObterCiclo(FiltroRelatorioResumoPAPDto filtros)
        {
            if (filtros.CicloId.HasValue && filtros.CicloId.Value > 0)
            {
                var ciclo = await mediator.Send(new ObterCicloPorIdQuery() { CicloId = (long)filtros.CicloId.Value });

                if (ciclo == null)
                    throw new NegocioException($"Não foi possível localizar dados do ciclo");
                return ciclo;
            }
            else
            {
                return new TipoCiclo() { Descricao = "Todos" };
            }
        }

        private async Task<Turma> ObterDadosTurma(FiltroRelatorioResumoPAPDto filtros)
        {
            if (string.IsNullOrEmpty(filtros.TurmaId) && filtros.TurmaId != "0")
            {
                var turma = await mediator.Send(new ObterTurmaQuery() { CodigoTurma = filtros.TurmaId });

                if (turma == null)
                    throw new NegocioException($"Não foi possível localizar dados da turma");
                return turma;
            }
            else
            {
                return new Turma() { Nome = "Todas" };
            }
        }

        private async Task<DreUe> ObterDadosDreUe(FiltroRelatorioResumoPAPDto filtros)
        {
            DreUe dreUe = new DreUe();

            if (string.IsNullOrEmpty(filtros.DreId) && filtros.DreId != "0")
            {
                var dre = await mediator.Send(new ObterDrePorCodigoQuery() { DreCodigo = filtros.DreId });

                if (dre != null)
                {
                    dreUe.DreCodigo = dre.Codigo;
                    dreUe.DreId = dre.Id;
                    dreUe.DreNome = dre.Nome;
                }
            }
            else
            {
                dreUe.DreNome = "Todas";
            }

            if (string.IsNullOrEmpty(filtros.UeId) && filtros.UeId != "0")
            {
                var ue = await mediator.Send(new ObterUePorCodigoQuery(filtros.UeId));

                if (ue != null)
                {
                    dreUe.UeCodigo = ue.Codigo;
                    dreUe.UeId = ue.Id;
                    dreUe.UeNome = ue.Nome;
                }
            }
            else
            {
                dreUe.UeNome = "Todas";
            }

            if (dreUe == null)
                throw new NegocioException($"Não foi possível localizar dados do Dre e Ue");
            return dreUe;
        }
    }
}
