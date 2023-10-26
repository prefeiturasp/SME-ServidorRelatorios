using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
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
        private readonly IMediator mediator;


        public ObterPlanejamentoDiarioPlanoAulaQueryHandler(IPlanoAulaRepository planoAulaRepository, IMediator mediator)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

            aulas = await TratarInformacoesComponentesCurriculares(aulas.ToList());

            return AgrupaAulasTurma(aulas, request.Parametros.ExibirDetalhamento);            
        }

        private async Task<IEnumerable<AulaPlanoAulaDto>> TratarInformacoesComponentesCurriculares(List<AulaPlanoAulaDto> planejamentosDiarios)
        {

            var idsComponentesSemNome = planejamentosDiarios.Where(ff => string.IsNullOrEmpty(ff.ComponenteCurricular)).Select(ff => ff.ComponenteCurricularId).Distinct();
            if (!idsComponentesSemNome.Any())
                return planejamentosDiarios;

            var informacoesComponentesCurriculares = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(idsComponentesSemNome.ToArray()));
            foreach (var planejamentoDiario in planejamentosDiarios.Where(ff => string.IsNullOrEmpty(ff.ComponenteCurricular)))
            {
                var componenteCurricular = informacoesComponentesCurriculares.Where(cc => cc.CodDisciplina == planejamentoDiario.ComponenteCurricularId).FirstOrDefault();
                if (!(componenteCurricular is null))
                {
                    planejamentoDiario.ComponenteCurricular = componenteCurricular.Disciplina;
                }
            }
            return planejamentosDiarios;
        }

        private IEnumerable<TurmaPlanejamentoDiarioDto> AgrupaAulasTurma(IEnumerable<AulaPlanoAulaDto> aulas, bool exibirDetalhamento)
        {
            foreach (var agrupamentoTurma in aulas.GroupBy(c => c.Turma))
            {
                var turma = new TurmaPlanejamentoDiarioDto() { Nome = agrupamentoTurma.Key};
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
                
                aulaPlanejamento.AulaId = aula.AulaId;
                aulaPlanejamento.AulaCJ = aula.AulaCJ;
                aulaPlanejamento.DataAula = aula.DataAula.ToString("dd/MM/yyyy");
                aulaPlanejamento.QuantidadeAulas = aula.QuantidadeAula;
                aulaPlanejamento.PlanejamentoRealizado = aula.DataPlanejamento.HasValue;

                if (aula.DataPlanejamento.HasValue)
                {
                    aulaPlanejamento.DateRegistro = aula.DataPlanejamento.Value.ToString("dd/MM/yyyy HH:mm");
                    aulaPlanejamento.Usuario = $"{aula.Usuario} ({aula.UsuarioRf})";
                    aulaPlanejamento.SecoesPreenchidas = ObterSecoesPreenchidas(aula);
                    aulaPlanejamento.QtdObjetivosEspecificos = aula.QtdObjetivosSelecionados;
                    aulaPlanejamento.QtdSecoesPreenchidas = aula.QtdSecoesPreenchidas;                   

                    if (exibirDetalhamento)
                    {
                        string ObjetivosSalecionados = "";
                        if (!string.IsNullOrEmpty(aula.ObjetivosSalecionados))
                        {
                            var ObjSplit = aula.ObjetivosSalecionados.Split("<br/>");                            

                            foreach (var obj in ObjSplit.OrderBy(c => c))
                            {
                                ObjetivosSalecionados += $"{obj} <br/>";
                            }
                        }                       

                        aulaPlanejamento.ObjetivosSelecionados = ObjetivosSalecionados;
                        aulaPlanejamento.MeusObjetivosEspecificos = string.IsNullOrEmpty(aula.ObjetivosEspecificos) ? "" : aula.ObjetivosEspecificos;
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
                secoes += "- Objetivos específicos e desenvolvimento da aula<br/>";
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
