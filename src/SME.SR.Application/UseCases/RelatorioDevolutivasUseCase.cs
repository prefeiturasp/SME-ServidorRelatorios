using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioDevolutivasUseCase : IRelatorioDevolutivasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioDevolutivasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var parametros = request.ObterObjetoFiltro<FiltroRelatorioDevolutivasDto>();

            var relatorioDto = new RelatorioDevolutivasDto();

            await ObterFiltrosRelatorio(relatorioDto, parametros);

            relatorioDto.Turmas = await mediator.Send(new ObterDevolutivasQuery(parametros.UeId, parametros.Turmas, parametros.Bimestres));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioDevolutivas", relatorioDto, request.CodigoCorrelacao));
        }

        private async Task ObterFiltrosRelatorio(RelatorioDevolutivasDto relatorioDto, FiltroRelatorioDevolutivasDto parametros)
        {
            var ue = await mediator.Send(new ObterUePorIdQuery(parametros.UeId));
            var dre = await mediator.Send(new ObterDrePorIdQuery(long.Parse(ue.DreId)));

            relatorioDto.Dre = dre.Nome;
            relatorioDto.Ue = ue.NomeComTipoEscola;
            relatorioDto.Turma = await ObterTurma(parametros.Turmas);
            relatorioDto.Bimestre = ObterBimestres(parametros.Bimestres);
            relatorioDto.Usuario = parametros.UsuarioNome;
            relatorioDto.RF = parametros.UsuarioRF;
        }

        private string ObterBimestres(IEnumerable<int> bimestres)
        {
            var bimestresDto = new List<string>();

            foreach (var bimestre in bimestres)
                bimestresDto.Add($"{bimestre}º");

            return string.Join(',', bimestresDto);
        }

        private async Task<string> ObterTurma(IEnumerable<long> turmas)
        {
            if (turmas.Count() == 1)
            {
                var turmaDto = await mediator.Send(new ObterTurmaPorIdQuery(turmas.First()));
                return turmaDto.NomeRelatorio;
            }

            return "";
        }
    }
}
