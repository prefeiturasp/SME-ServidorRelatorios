using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPlanejamentoDiarioPlanoAulaQueryHandler : IRequestHandler<ObterPlanejamentoDiarioPlanoAulaQuery, IEnumerable<TurmaPlanejamentoDiarioDto>>
    {
        private readonly IPlanoAulaRepository planoAulaRepository;

        public ObterPlanejamentoDiarioPlanoAulaQueryHandler(IPlanoAulaRepository planoAulaRepository)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
        }

        public async Task<IEnumerable<TurmaPlanejamentoDiarioDto>> Handle(ObterPlanejamentoDiarioPlanoAulaQuery request, CancellationToken cancellationToken)
        {
            var modalidadeCalendario = request.Parametros.ModalidadeTurma == Modalidade.EJA ?
                                                ModalidadeTipoCalendario.EJA : request.Parametros.ModalidadeTurma == Modalidade.Infantil ?
                                                    ModalidadeTipoCalendario.Infantil : ModalidadeTipoCalendario.FundamentalMedio;
            var aulas = await planoAulaRepository.ObterPlanejamentoDiarioPlanoAula(
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
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            return AgrupaAulasTurma(aulas, request.Parametros.ExibirDetalhamento);            
        }

        private IEnumerable<TurmaPlanejamentoDiarioDto> AgrupaAulasTurma(IEnumerable<AulaPlanoAulaDto> aulas, bool exibirDetalhamento)
        {
            foreach (var agrupamentoTurma in aulas.GroupBy(c => c.Turma))
            {
                var turma = new TurmaPlanejamentoDiarioDto() { Nome = agrupamentoTurma.Key };
                turma.Bimestres = AgrupaAulasBimestre(agrupamentoTurma, exibirDetalhamento);

                yield return turma;
            }
        }

        private IEnumerable<BimestrePlanejamentoDiarioDto> AgrupaAulasBimestre(IGrouping<string, AulaPlanoAulaDto> aulasTurma, bool exibirDetalhamento)
        {
            foreach (var agrupamentoBimestre in aulasTurma.GroupBy(c => c.Bimestre))
            {
                var bimestre = new BimestrePlanejamentoDiarioDto();
                if (agrupamentoBimestre.Key.HasValue)
                    bimestre.Nome = $"{agrupamentoBimestre.Key}º Bimestre";

                bimestre.ComponentesCurriculares = AgrupaAulasComponentes(agrupamentoBimestre, exibirDetalhamento);

                yield return bimestre;
            }
        }

        private IEnumerable<ComponenteCurricularPlanejamentoDiarioDto> AgrupaAulasComponentes(IGrouping<int?, AulaPlanoAulaDto> aulasBimestre, bool exibirDetalhamento)
        {
            foreach (var agrupamentoComponente in aulasBimestre.GroupBy(c => c.ComponenteCurricular))
            {
                var componente = new ComponenteCurricularPlanejamentoDiarioDto();

                componente.Nome = agrupamentoComponente.Key;
                componente.PlanejamentoDiario = ObterDadosAulasComponente(agrupamentoComponente, exibirDetalhamento);

                yield return componente;
            }
        }

        private IEnumerable<PlanejamentoDiarioDto> ObterDadosAulasComponente(IGrouping<string, AulaPlanoAulaDto> aulasComponenteCurricular, bool exibirDetalhamento)
        {
            foreach (var aula in aulasComponenteCurricular.OrderByDescending(o => o.DataAula))
            {
                var aulaPlanejamento = new PlanejamentoDiarioDto();

                aulaPlanejamento.DataAula = aula.DataAula.ToString("dd/MM/yyyy");
                aulaPlanejamento.QuantidadeAulas = aula.QuantidadeAula;
                aulaPlanejamento.PlanejamentoRealizado = aula.DataPlanejamento.HasValue ? "Sim" : "Não";

                if (aula.DataPlanejamento.HasValue)
                {
                    aulaPlanejamento.DateRegistro = aula.DataPlanejamento.Value.ToString("dd/MM/yyyy HH:mm");
                    aulaPlanejamento.Usuario = $"{aula.Usuario} ({aula.UsuarioRf})";
                    aulaPlanejamento.SecoesPreenchidas = ObterSecoesPreenchidas(aula);
                    aulaPlanejamento.QtdObjetivosEspecificos = aula.QtdObjetivosSelecionados;
                    aulaPlanejamento.QtdSecoesPreenchidas = aula.QtdSecoesPreenchidas;

                    if (exibirDetalhamento)
                    {
                        aulaPlanejamento.ObjetivosSelecionados = aula.ObjetivosSalecionados;
                        aulaPlanejamento.MeusObjetivosEspecificos = aula.ObjetivosEspecificos;
                        aulaPlanejamento.DesenvolvimentoAula = aula.DesenvolvimentoAula;
                    }
                }

                yield return aulaPlanejamento;
            }
        }

        private string ObterSecoesPreenchidas(AulaPlanoAulaDto aula)
        {
            var secoes = "";
            int qtdSecoesPreenchidas = 0;

            if (!string.IsNullOrEmpty(aula.ObjetivosSalecionados))
            {
                secoes += $"- Objetivos de Aprendizagem e Desenvolvimento {aula.QtdObjetivosSelecionados} objetivos selecionados<br/>";
                qtdSecoesPreenchidas++;
            }
                
            if (!string.IsNullOrEmpty(aula.ObjetivosEspecificos))
            {
                secoes += "- Meus Objetivos específicos<br/>";
                qtdSecoesPreenchidas++;
            }                

            if (!string.IsNullOrEmpty(aula.DesenvolvimentoAula))
            {
                secoes += "- Desenvolvimento da aula<br/>";
                qtdSecoesPreenchidas++;
            }                

            if (!string.IsNullOrEmpty(aula.LicaoCasa))
            {
                secoes += "- Lição de casa<br/>";
                qtdSecoesPreenchidas++;
            }               

            aula.QtdSecoesPreenchidas = qtdSecoesPreenchidas;

            return secoes;
        }
    }
}
