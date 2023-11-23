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
    public class ObterDadosPlanejamentoDiarioBordoComComponenteQueryHandler : IRequestHandler<ObterDadosPlanejamentoDiarioBordoComComponenteQuery, IEnumerable<TurmaPlanejamentoDiarioInfantilDto>>
    {
        private readonly IDiarioBordoRepository diarioBordoRepository;

        public ObterDadosPlanejamentoDiarioBordoComComponenteQueryHandler(IDiarioBordoRepository diarioBordoRepository)
        {
            this.diarioBordoRepository = diarioBordoRepository ?? throw new ArgumentNullException(nameof(diarioBordoRepository));
        }

        public async Task<IEnumerable<TurmaPlanejamentoDiarioInfantilDto>> Handle(ObterDadosPlanejamentoDiarioBordoComComponenteQuery request, CancellationToken cancellationToken)
        {
            var modalidadeCalendario = request.Parametros.ModalidadeTurma.ObterModalidadeTipoCalendario();

            var aulas = await diarioBordoRepository.ObterAulasDiarioBordoComComponenteCurricular(
                request.Parametros.AnoLetivo,
                request.Parametros.Bimestre,
                request.Parametros.CodigoUe,
                request.Parametros.ComponentesCurricularesDisponiveis,
                request.Parametros.ListarDataFutura,
                request.Parametros.CodigoTurma,
                request.Parametros.ModalidadeTurma,
                modalidadeCalendario,
                request.Parametros.Semestre);

            if (aulas == null || !aulas.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var turmas = AgrupaTurma(aulas, request.Parametros.ExibirDetalhamento);            

            return turmas;
        } 

        private IEnumerable<TurmaPlanejamentoDiarioInfantilDto> AgrupaTurma(IEnumerable<AulaDiarioBordoDto> aulas, bool exibirDetalhamento)
        {
            foreach (var agrupamentoTurma in aulas.GroupBy(c => c.Turma))
            {
                var turma = new TurmaPlanejamentoDiarioInfantilDto() { Nome = agrupamentoTurma.Key };
                turma.Bimestres = AgrupaAulasBimestre(agrupamentoTurma, exibirDetalhamento).ToList();

                yield return turma;
            }
        }

        private IEnumerable<BimestrePlanejamentoDiarioInfantilDto> AgrupaAulasBimestre(IGrouping<string, AulaDiarioBordoDto> aulasTurma, bool exibirDetalhamento)
        {
            foreach (var agrupamentoBimestre in aulasTurma.GroupBy(c => c.Bimestre))
            {
                var bimestre = new BimestrePlanejamentoDiarioInfantilDto();
                if (agrupamentoBimestre.Key.HasValue)
                {
                    foreach (var agrupamentoBimestreData in agrupamentoBimestre.ToList())
                        bimestre.Nome = $"{agrupamentoBimestre.Key}º Bimestre";
                }

                bimestre.Planejamento = AgrupaAulas(agrupamentoBimestre, exibirDetalhamento).ToList();

                yield return bimestre;
            }
        }

        private IEnumerable<PlanejamentoDiarioInfantilDto> AgrupaAulas(IGrouping<int?, AulaDiarioBordoDto> aulasBimestre, bool exibirDetalhamento)
        {
            foreach (var aula in aulasBimestre.ToList())
            {
                var aulaPlanejamento = new PlanejamentoDiarioInfantilDto();

                aulaPlanejamento.AulaId = aula.AulaId;
                aulaPlanejamento.AulaCJ = aula.AulaCJ;
                aulaPlanejamento.DataAula = aula.DataAula.ToString("dd/MM/yyyy");
                aulaPlanejamento.PlanejamentoRealizado = aula.DataPlanejamento.HasValue;
                aulaPlanejamento.ComponenteCurricular = string.IsNullOrEmpty(aula?.ComponenteCurricular) ? "" : aula?.ComponenteCurricular;
                aulaPlanejamento.EhReposicao = aula.TipoAula == TipoAula.Reposicao;

                if (aula.DataPlanejamento.HasValue)
                {
                    aulaPlanejamento.DateRegistro = aula.DataPlanejamento.Value.ToString("dd/MM/yyyy HH:mm");
                    aulaPlanejamento.Usuario = $"{aula.Usuario} ({aula.UsuarioRf})";
                    aulaPlanejamento.SecoesPreenchidas = ObterSecoesPreenchidas(aula);
                    if (exibirDetalhamento)
                        aulaPlanejamento.Planejamento = string.IsNullOrEmpty(aula.Planejamento) ? "" : aula.Planejamento;
                }

                yield return aulaPlanejamento;
            }
        }

        private string ObterSecoesPreenchidas(AulaDiarioBordoDto aula)
        {
            var secoes = "";

            if (!string.IsNullOrEmpty(aula.Planejamento))
                secoes += "- Planejamento<br/>";

            if (aula.DevolutivaId.HasValue)
                secoes += "- Devolutiva<br/>";

            return secoes;
        }
    }
}
