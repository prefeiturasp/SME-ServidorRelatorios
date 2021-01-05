﻿using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosPlanejamentoDiarioBordoQueryHandler : IRequestHandler<ObterDadosPlanejamentoDiarioBordoQuery, IEnumerable<TurmaPlanejamentoDiarioDto>>
    {
        private readonly IDiarioBordoRepository diarioBordoRepository;

        public ObterDadosPlanejamentoDiarioBordoQueryHandler(IDiarioBordoRepository diarioBordoRepository)
        {
            this.diarioBordoRepository = diarioBordoRepository ?? throw new ArgumentNullException(nameof(diarioBordoRepository));
        }

        public async Task<IEnumerable<TurmaPlanejamentoDiarioDto>> Handle(ObterDadosPlanejamentoDiarioBordoQuery request, CancellationToken cancellationToken)
        {
            var modalidadeCalendario = request.Parametros.ModalidadeTurma == Modalidade.EJA ?
                                                ModalidadeTipoCalendario.EJA : request.Parametros.ModalidadeTurma == Modalidade.Infantil ?
                                                    ModalidadeTipoCalendario.Infantil : ModalidadeTipoCalendario.FundamentalMedio;
            var aulas = await diarioBordoRepository.ObterAulasDiarioBordo(
                request.Parametros.AnoLetivo,
                request.Parametros.Bimestre,
                request.Parametros.CodigoUe,
                request.Parametros.ComponenteCurricular,
                request.Parametros.ListarDataFutura,
                request.Parametros.CodigoTurma,
                request.Parametros.ModalidadeTurma,
                modalidadeCalendario,
                request.Parametros.Semestre);

            if (aulas == null || !aulas.Any())
                return null;

            return AgrupaAulasTurma(aulas, request.Parametros.ExibirDetalhamento);
        }

        private IEnumerable<TurmaPlanejamentoDiarioDto> AgrupaAulasTurma(IEnumerable<AulaDiarioBordoDto> aulas, bool exibirDetalhamento)
        {
            foreach (var agrupamentoTurma in aulas.GroupBy(c => c.Turma))
            {
                var turma = new TurmaPlanejamentoDiarioDto() { Nome = agrupamentoTurma.Key };
                turma.Bimestres = AgrupaAulasBimestre(agrupamentoTurma, exibirDetalhamento);

                yield return turma;
            }
        }

        private IEnumerable<BimestrePlanejamentoDiarioDto> AgrupaAulasBimestre(IGrouping<string, AulaDiarioBordoDto> aulasTurma, bool exibirDetalhamento)
        {
            foreach(var agrupamentoBimestre in aulasTurma.GroupBy(c => c.Bimestre))
            {
                var bimestre = new BimestrePlanejamentoDiarioDto();
                if (agrupamentoBimestre.Key.HasValue)
                    bimestre.Nome = $"{agrupamentoBimestre.Key}º Bimestre";

                bimestre.ComponentesCurriculares = AgrupaAulasComponentes(agrupamentoBimestre, exibirDetalhamento);

                yield return bimestre;
            }
        }

        private IEnumerable<ComponenteCurricularPlanejamentoDiarioDto> AgrupaAulasComponentes(IGrouping<int?, AulaDiarioBordoDto> aulasBimestre, bool exibirDetalhamento)
        {
            foreach (var agrupamentoComponente in aulasBimestre.GroupBy(c => c.ComponenteCurricular))
            {
                var componente = new ComponenteCurricularPlanejamentoDiarioDto();

                componente.Nome = agrupamentoComponente.Key;
                componente.PlanejamentoDiarioInfantil = ObterDadosAulasComponente(agrupamentoComponente, exibirDetalhamento);

                yield return componente;
            }
        }

        private IEnumerable<PlanejamentoDiarioInfantilDto> ObterDadosAulasComponente(IGrouping<string, AulaDiarioBordoDto> aulasComponenteCurricular, bool exibirDetalhamento)
        {
            foreach(var aula in aulasComponenteCurricular.OrderByDescending(o => o.DataAula))
            {
                var aulaPlanejamento = new PlanejamentoDiarioInfantilDto();

                aulaPlanejamento.DataAula = aula.DataAula.ToString("dd/MM/yyyy");
                aulaPlanejamento.PlanejamentoRealizado = aula.DataPlanejamento.HasValue ? "Sim" : "Não";


                if (aula.DataPlanejamento.HasValue)
                {
                    aulaPlanejamento.DateRegistro = aula.DataPlanejamento.Value.ToString("dd/MM/yyyy HH:mm");
                    aulaPlanejamento.Usuario = $"{aula.Usuario} ({aula.UsuarioRf})";
                    aulaPlanejamento.SecoesPreenchidas = ObterSecoesPreenchidas(aula);

                    if (exibirDetalhamento)
                        aulaPlanejamento.Planejamento = aula.Planejamento; 
                }

                yield return aulaPlanejamento;
            }
        }

        private string ObterSecoesPreenchidas(AulaDiarioBordoDto aula)
        {
            var secoes = "";

            if (!string.IsNullOrEmpty(aula.Planejamento))
                secoes += "- Planejamento<br/>";

            if (!string.IsNullOrEmpty(aula.Reflexoes))
                secoes += "- Reflexões e Replanejamento<br/>";

            if (aula.DevolutivaId.HasValue)
                secoes += "- Devolutiva<br/>";

            return secoes;
        }
    }
}
