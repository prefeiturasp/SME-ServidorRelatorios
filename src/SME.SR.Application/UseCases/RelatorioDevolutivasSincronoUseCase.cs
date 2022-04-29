using MediatR;
using Sentry;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioDevolutivasSincronoUseCase : IRelatorioDevolutivasSincronoUseCase
    {
        private readonly IMediator mediator;
        private readonly IDevolutivaRepository devolutivaRepository;
        public RelatorioDevolutivasSincronoUseCase(IMediator mediator, IDevolutivaRepository devolutivaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.devolutivaRepository = devolutivaRepository ?? throw new ArgumentNullException(nameof(devolutivaRepository));
        }
        public async Task GerarRelatorioSincrono(FiltroRelatorioDto request)
        {
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroRelatorioDevolutivasSincronoDto>();
                var relatorioDto = new RelatorioDevolutivasSincronoDto();
                
                var devolutiva = await devolutivaRepository.ObterDevolutiva(parametros.DevolutivaId);
                if (devolutiva == null)
                    new NegocioException("Devolutiva não encontrada!!");

                var turmaDto = await mediator.Send(new ObterTurmaPorIdQuery(parametros.TurmaId));
                if (turmaDto == null)
                    new NegocioException("Turma não encontrada!!");

                await ObterFiltrosRelatorio(relatorioDto, parametros, devolutiva.Bimestre, turmaDto.NomeRelatorio);

                relatorioDto.Turmas = await MapearTurma(devolutiva, turmaDto.NomeRelatorio);
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioDevolutivaSincrono", relatorioDto, request.CodigoCorrelacao, relatorioSincrono: true));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"Devolutiva Sincrono - Erro na geração: {ex.Message}, [{ex.StackTrace}]");
                SentrySdk.CaptureException(ex);
            }

        }


        private async Task<IEnumerable<TurmaDevolutivaSincronoDto>> MapearTurma(DevolutivaSincronoDto devolutiva, string nomeTurma)
        {
            try
            {
                var retorno = new List<TurmaDevolutivaSincronoDto>();
                var turmaDevolutivaSincronoDto = new TurmaDevolutivaSincronoDto
                {
                    NomeTurma = nomeTurma,
                };
                var bimestreDevolutivaSincronoDto = new BimestreDevolutivaSincronoDto
                {
                    NomeBimestre = $"{devolutiva.Bimestre}º BIMESTRE"
                };

                var devolutivaRelatorioSincronoDto = new DevolutivaRelatorioSincronoDto
                {
                    DiasIntervalo = devolutiva.DataAula.ToString("dd/MM"),
                    IntervaloDatas = $"{devolutiva.DataInicio:dd/MM/yyyy} até {devolutiva.DataFim:dd/MM/yyyy}",
                    DataRegistro = devolutiva.DataRegistro.ToString("dd/MM/yyyy"),
                    ResgistradoPor = $"{devolutiva.RegistradoPor} ({devolutiva.RegistradoRF})",
                    Descricao = await FormatarHtml(devolutiva.Descricao)
                };
                bimestreDevolutivaSincronoDto.Devolutivas = new List<DevolutivaRelatorioSincronoDto> { devolutivaRelatorioSincronoDto };

                turmaDevolutivaSincronoDto.Bimestres = new List<BimestreDevolutivaSincronoDto> { bimestreDevolutivaSincronoDto };
                retorno.Add(turmaDevolutivaSincronoDto);
                return retorno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"Devolutiva Sincrono: {ex.Message}, [{ex.StackTrace}]");
                SentrySdk.CaptureException(ex);
                throw;
            }
        }
        private async Task<string> FormatarHtml(string html)
                => await mediator.Send(new ObterHtmlComImagensBase64Query(html));
        private async Task ObterFiltrosRelatorio(RelatorioDevolutivasSincronoDto relatorioDto, FiltroRelatorioDevolutivasSincronoDto parametros, long bimestre, string nomeTurma)
        {
            try
            {
                var ue = await mediator.Send(new ObterUePorIdQuery(parametros.UeId));
                if (ue == null)
                    new NegocioException("UE não encontrada!!");

                var dre = await mediator.Send(new ObterDrePorIdQuery(ue.DreId));
                if (dre == null)
                    new NegocioException("DRE não encontrada!!");

                relatorioDto.Dre = dre.Abreviacao;
                relatorioDto.Ue = $"{ue.Codigo} - {ue.NomeComTipoEscola}";
                relatorioDto.Turma = nomeTurma;
                relatorioDto.Bimestre = $"{bimestre}º";
                relatorioDto.Usuario = parametros.UsuarioNome;
                relatorioDto.RF = parametros.UsuarioRF;
                relatorioDto.ExibeConteudoDevolutivas = true;
                relatorioDto.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"Devolutiva Sincrono - Erro na geração: {ex.Message}, [{ex.StackTrace}]");
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
