using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentosAeeUseCase : IRelatorioEncaminhamentosAeeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioEncaminhamentosAeeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentosAeeDto>();
            var encaminhamentosAee = await mediator.Send(new ObterEncaminhamentosAEEQuery(filtroRelatorio));

            if (encaminhamentosAee == null || !encaminhamentosAee.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var encaminhamentosAgrupados = encaminhamentosAee.GroupBy(g => new
            {
                DreId = g.DreId,
                DreNome = g.DreAbreviacao,
                UeCodigo = g.UeCodigo,
                UeNome = $"{g.TipoEscola.ShortName()} {g.UeNome}",
            }, (key, group) =>
            new AgrupamentoEncaminhamentoAeeDreUeDto()
            {
                DreId = key.DreId,
                DreNome = key.DreNome,
                UeNome = $"{key.UeCodigo} - {key.UeNome}",
                UeOrdenacao = key.UeNome,
                Detalhes = group.Select(s =>
                new DetalheEncaminhamentoAeeDto()
                {
                    Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                    Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}",
                    Situacao = ((SituacaoEncaminhamentoAEE)s.Situacao).Name(),
                    ResponsavelPAAI = !string.IsNullOrEmpty(s.ResponsavelPaaiNome) ? $"{s.ResponsavelPaaiNome} ({s.ResponsavelPaaiLoginRf})" : string.Empty,
                }).OrderBy(oAluno => oAluno.Aluno).ToList()
            }).OrderBy(oDre => oDre.DreId).ThenBy(oUe => oUe.UeOrdenacao).ToList();

            var cabecalho = new CabecalhoEncaminhamentoAeeDto()
            {
                DreNome = filtroRelatorio.DreCodigo.Equals("-99") ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().DreNome,
                UeNome = filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().UeNome,
                UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
            };

            await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentosAeeCommand(cabecalho, encaminhamentosAgrupados, request.CodigoCorrelacao));
        }
    }
}
