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

            try
            {
                var relatorioDto = new RelatorioDevolutivasDto();

                await ObterFiltrosRelatorio(relatorioDto, parametros);

                var turmas = ObterTurmas(parametros.Turmas);
                var bimestres = ObterBimestresFiltro(parametros.Bimestres);

                relatorioDto.Turmas = await mediator.Send(new ObterDevolutivasQuery(parametros.UeId, turmas, bimestres, parametros.Ano));

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioDevolutivas", relatorioDto, request.CodigoCorrelacao));

            }
            catch (Exception e)
            {

                throw;
            }        
        }

        private IEnumerable<int> ObterBimestresFiltro(IEnumerable<int> bimestres)
        {
            if (bimestres.Count() == 1 && (bimestres.First() == -99))
                return new List<int>() { 1,2,3,4 };

            return bimestres;
        }

        private IEnumerable<long> ObterTurmas(IEnumerable<long> turmas)
        {
            if (turmas.Count() == 1 && (turmas.First() == -99))
                return Enumerable.Empty<long>();

            return turmas;
        }

        private async Task ObterFiltrosRelatorio(RelatorioDevolutivasDto relatorioDto, FiltroRelatorioDevolutivasDto parametros)
        {
            var ue = await mediator.Send(new ObterUePorIdQuery(parametros.UeId));
            var dre = await mediator.Send(new ObterDrePorIdQuery(long.Parse(ue.DreId)));

            var turmas = ObterTurmas(parametros.Turmas);
            var bimestres = ObterBimestresFiltro(parametros.Bimestres);

            relatorioDto.Dre = dre.Abreviacao;
            relatorioDto.Ue = $"{ue.Codigo} - {ue.NomeComTipoEscola}";
            relatorioDto.Turma = await ObterTurma(turmas);
            relatorioDto.Bimestre = ObterBimestres(bimestres);
            relatorioDto.Usuario = parametros.UsuarioNome;
            relatorioDto.RF = parametros.UsuarioRF;
            relatorioDto.ExibeConteudoDevolutivas = parametros.ExibirDetalhes;
            relatorioDto.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
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
                var turmaDto = await mediator.Send(new ObterTurmaPorCodigoQuery(turmas.First().ToString()));
                return turmaDto.NomeRelatorio;
            }

            return "Todas";
        }
    }
}
