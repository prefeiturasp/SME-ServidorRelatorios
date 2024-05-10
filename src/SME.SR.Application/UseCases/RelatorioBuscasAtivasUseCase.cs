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
            var registrosAcaoBuscaAtiva = Enumerable.Empty<BuscaAtivaSimplesDto>().ToList(); //await mediator.Send(new ObterResumoEncaminhamentosNAAPAQuery(filtroRelatorio));

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
                Detalhes = group.Select(s =>
                new DetalheBuscaAtivaDto()
                {
                    Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                    Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}{s.TurmaTipoTurno.NomeTipoTurnoEol(" - ")}"
                }).OrderByDescending(oAluno => oAluno.DataEntradaQueixa).ToList()
            }).OrderBy(oDre => oDre.DreCodigo).ThenBy(oUe => oUe.UeOrdenacao).ToList();

            var relatorio = new RelatorioBuscaAtivaDto()
            {
                DreNome = !string.IsNullOrEmpty(filtroRelatorio.DreCodigo) && filtroRelatorio.DreCodigo.Equals("-99") || string.IsNullOrEmpty(filtroRelatorio.DreCodigo) ? "TODAS" : registrosAcaoAgrupados.FirstOrDefault().DreNome,
                UeNome = !string.IsNullOrEmpty(filtroRelatorio.UeCodigo) && filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : registrosAcaoAgrupados.FirstOrDefault().UeNome,
                AnoLetivo = filtroRelatorio.AnoLetivo,
                Modalidade = filtroRelatorio.Modalidade,
                Semestre = filtroRelatorio.Semestre,
                Turma = filtroRelatorio.TurmasCodigo.Count() != 1 ? "TODAS" : registrosAcaoAgrupados.FirstOrDefault().Detalhes.FirstOrDefault().Turma,
                UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
            };

            relatorio.RegistrosAcaoDreUe = registrosAcaoAgrupados;
            //await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad(relatorio, request.CodigoCorrelacao));
        }
    }
}
