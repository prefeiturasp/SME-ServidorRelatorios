using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioBuscasAtivasUseCase : IRelatorioBuscasAtivasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioBuscasAtivasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioBuscasAtivasDto>();
            var registrosAcaoBuscaAtiva = await mediator.Send(new ObterResumoBuscasAtivasQuery(filtroRelatorio));

            if (registrosAcaoBuscaAtiva == null || !registrosAcaoBuscaAtiva.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var registrosAcaoAgrupados = registrosAcaoBuscaAtiva.GroupBy(g => new
            {
                g.DreCodigo,
                DreNome = g.DreAbreviacao,
                g.UeCodigo,
                UeNome = $"{g.TipoEscola.ShortName()} {g.UeNome}",
            }, (key, group) =>
            new AgrupamentoBuscaAtivaDreUeDto()
            {
                DreCodigo = key.DreCodigo,
                DreNome = key.DreNome,
                UeNome = $"{key.UeCodigo} - {key.UeNome}",
                UeOrdenacao = key.UeNome,
                MostrarAgrupamento = TodasDreFiltro(filtroRelatorio) || TodasUeFiltro(filtroRelatorio),
                Detalhes = group.Select(s =>
                new DetalheBuscaAtivaDto()
                {
                    Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                    Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}{s.TurmaTipoTurno.NomeTipoTurnoEol(" - ")}",
                    DataRegistroAcao = s.DataRegistroAcao,
                    Questoes = ObterQuestoes(s, !string.IsNullOrEmpty(filtroRelatorio.AlunoCodigo))
                    
                }).OrderBy(oAluno => oAluno.Turma)
                .ThenBy(oAluno => oAluno.Aluno)
                .ThenByDescending(oAluno => oAluno.DataRegistroAcao)
                .ToList()
            }).OrderBy(oDre => oDre.DreCodigo).ThenBy(oUe => oUe.UeOrdenacao).ToList();

            var relatorio = new RelatorioBuscaAtivaDto()
            {
                DreNome = TodasDreFiltro(filtroRelatorio) ? "TODAS" : registrosAcaoAgrupados.FirstOrDefault().DreNome,
                UeNome = TodasUeFiltro(filtroRelatorio) ? "TODAS" : registrosAcaoAgrupados.FirstOrDefault().UeNome,
                AnoLetivo = filtroRelatorio.AnoLetivo,
                Modalidade = filtroRelatorio.Modalidade,
                Semestre = filtroRelatorio.Semestre,
                Turma = filtroRelatorio.TurmasCodigo.Count() == 1 
                        ? registrosAcaoAgrupados.FirstOrDefault().Detalhes.FirstOrDefault().Turma
                        : filtroRelatorio.TurmasCodigo.Count() == 0  
                          ? "TODAS" 
                          : "" ,
                UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
            };

            relatorio.RegistrosAcaoDreUe = registrosAcaoAgrupados;
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioBuscaAtiva", relatorio, request.CodigoCorrelacao));
        }

        private List<ItemQuestaoDetalheBuscaAtivaDto> ObterQuestoes(BuscaAtivaSimplesDto buscaAtiva, bool apresentarQuestaoObservacoes = false)
        {
            var retorno = new List<ItemQuestaoDetalheBuscaAtivaDto>();
            retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("DATA DE REGISTRO DA AÇÃO:", buscaAtiva.DataRegistroAcao.ToString("dd/MM/yyyy")));
            retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("Procedimento realizado:", buscaAtiva.ProcedimentoRealizado));
            retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("Conseguiu contato com o responsável?", buscaAtiva.ConseguiuContatoResponsavel));

            if (!string.IsNullOrEmpty(buscaAtiva.QuestoesObsDuranteVisita))
                retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("Questões observadas durante a realização das visitas:", buscaAtiva.QuestoesObsDuranteVisita));
            if (!string.IsNullOrEmpty(buscaAtiva.JustificativaMotivoFalta))
                retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("A família/responsável justificou a falta da criança por motivo de:", buscaAtiva.JustificativaMotivoFalta));
            if (!string.IsNullOrEmpty(buscaAtiva.JustificativaMotivoFaltaOpcaoOutros))
                retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("Descreva a justificativa da família:", buscaAtiva.JustificativaMotivoFaltaOpcaoOutros));
            if (apresentarQuestaoObservacoes &&!string.IsNullOrEmpty(buscaAtiva.ObsGeralAoContatarOuNaoResponsavel))
                retorno.Add(new ItemQuestaoDetalheBuscaAtivaDto("Observação:", buscaAtiva.ObsGeralAoContatarOuNaoResponsavel));
            return retorno;
        }

        private bool TodasDreFiltro(FiltroRelatorioBuscasAtivasDto filtroRelatorio)
        => !string.IsNullOrEmpty(filtroRelatorio.DreCodigo) && filtroRelatorio.DreCodigo.Equals("-99") || string.IsNullOrEmpty(filtroRelatorio.DreCodigo);

        private bool TodasUeFiltro(FiltroRelatorioBuscasAtivasDto filtroRelatorio)
        => !string.IsNullOrEmpty(filtroRelatorio.UeCodigo) && filtroRelatorio.UeCodigo.Equals("-99");

    }
}
