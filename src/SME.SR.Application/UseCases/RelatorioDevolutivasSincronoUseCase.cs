using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.Utilitarios;
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
        public async Task<Guid> GerarRelatorioSincrono(FiltroRelatorioDevolutivasSincronoDto dto)
        {
            var codigoCorrelacao = Guid.NewGuid();
            var relatorioDto = new RelatorioDevolutivasSincronoDto();
            var devolutiva = await devolutivaRepository.ObterDevolutiva(dto.DevolutivaId);
            var turmaDto = await mediator.Send(new ObterTurmaPorIdQuery(dto.TurmaId));

            await ObterFiltrosRelatorio(relatorioDto, dto, devolutiva.Bimestre, turmaDto.NomeRelatorio);

            relatorioDto.Turmas = await MapearTurma(devolutiva, turmaDto.NomeRelatorio);
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioDevolutivaSincrono", relatorioDto, codigoCorrelacao, relatorioSincrono: true));

            return codigoCorrelacao;
        }


        private async Task<IEnumerable<TurmaDevolutivaSincronoDto>> MapearTurma(DevolutivaSincronoDto devolutiva, string nomeTurma)
        {
            var retorno = new List<TurmaDevolutivaSincronoDto>();
            var turmaDevolutivaSincronoDto = new TurmaDevolutivaSincronoDto
            {
                NomeTurma = nomeTurma,
            };
            var bimestreDevolutivaSincronoDto = new BimestreDevolutivaSincronoDto
            {
                NomeBimestre = $"{devolutiva.Bimestre} º BIMESTRE"
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
        private async Task<string> FormatarHtml(string html)
                => await mediator.Send(new ObterHtmlComImagensBase64Query(html));
        private async Task ObterFiltrosRelatorio(RelatorioDevolutivasSincronoDto relatorioDto, FiltroRelatorioDevolutivasSincronoDto dto, long bimestre, string nomeTurma)
        {
            var ue = await mediator.Send(new ObterUePorIdQuery(dto.UeId));
            var dre = await mediator.Send(new ObterDrePorIdQuery(long.Parse(ue.DreId)));

            relatorioDto.Dre = dre.Abreviacao;
            relatorioDto.Ue = $"{ue.Codigo} - {ue.NomeComTipoEscola}";
            relatorioDto.Turma = nomeTurma;
            relatorioDto.Bimestre = $"{bimestre}º";
            relatorioDto.Usuario = dto.UsuarioNome;
            relatorioDto.RF = dto.UsuarioRF;
            relatorioDto.ExibeConteudoDevolutivas = true;
            relatorioDto.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
