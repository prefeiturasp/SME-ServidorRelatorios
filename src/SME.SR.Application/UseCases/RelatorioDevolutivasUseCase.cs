﻿using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var turmas = ObterTurmas(parametros.Turmas);
            var bimestres = ObterBimestresFiltro(parametros.Bimestres);

            relatorioDto.Turmas = await mediator.Send(new ObterDevolutivasQuery(parametros.UeId, turmas, bimestres, parametros.Ano));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioDevolutivas", relatorioDto, request.CodigoCorrelacao));
        }

        private IEnumerable<int> ObterBimestresFiltro(IEnumerable<int> bimestres)
        {
            if (bimestres.Count() == 1 && (bimestres.First() == -99))
                return new List<int>() { 1, 2, 3, 4 };

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
            var ue = await mediator.Send(new ObterUeComDrePorIdQuery(parametros.UeId));

            var turmas = ObterTurmas(parametros.Turmas);
            var bimestres = ObterBimestresFiltro(parametros.Bimestres);

            relatorioDto.Dre = ue.Dre.Abreviacao;
            relatorioDto.Ue = ue.NomeRelatorio;
            relatorioDto.Turma = await ObterTurma(turmas);
            relatorioDto.Bimestre = ObterBimestres(bimestres);
            relatorioDto.Usuario = parametros.UsuarioNome;
            relatorioDto.RF = parametros.UsuarioRF;
            relatorioDto.ExibeConteudoDevolutivas = parametros.ExibirDetalhes;            
        }

        private string ObterBimestres(IEnumerable<int> bimestres)
        {
            if (bimestres.Any(t => t == -99))
                return "";

            var bimestresDto = new List<string>();

            foreach (var bimestre in bimestres)
                bimestresDto.Add($"{bimestre}º");

            return string.Join(',', bimestresDto);
        }

        private async Task<string> ObterTurma(IEnumerable<long> turmas)
        {
            if (turmas.Any(t => t == -99))
                return "Todas";

            if (turmas.Count() == 1 && !turmas.Any(t => t == -99))
            {
                var turmaDto = await mediator.Send(new ObterTurmaPorIdQuery(turmas.First()));
                return turmaDto.NomeRelatorio;
            }

            return "";
        }
    }
}
