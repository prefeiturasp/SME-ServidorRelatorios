﻿using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentosNAAPAUseCase : IRelatorioEncaminhamentosNAAPAUseCase
    {
        private readonly IMediator mediator;

        public RelatorioEncaminhamentosNAAPAUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoNAAPADto>();
            var encaminhamentosNAAPA = await mediator.Send(new ObterResumoEncaminhamentosNAAPAQuery(filtroRelatorio));

            if (encaminhamentosNAAPA == null || !encaminhamentosNAAPA.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var encaminhamentosAgrupados = encaminhamentosNAAPA.GroupBy(g => new
            {
                g.DreId,
                DreNome = g.DreAbreviacao,
                g.UeCodigo,
                UeNome = $"{g.TipoEscola.ShortName()} {g.UeNome}",
            }, (key, group) =>
            new AgrupamentoEncaminhamentoNAAPADreUeDto()
            {
                DreId = key.DreId,
                DreNome = key.DreNome,
                UeNome = $"{key.UeCodigo} - {key.UeNome}",
                UeOrdenacao = key.UeNome,
                Detalhes = group.Select(s =>
                new DetalheEncaminhamentoNAAPADto()
                {
                    Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                    Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}{s.TurmaTipoTurno.NomeTipoTurnoEol(" - ")}",
                    PortaEntrada = s.PortaEntrada,
                    DataEntradaQueixa = s.DataEntradaQueixa,
                    DataUltimoAtendimento = s.DataUltimoAtendimento,
                    FluxosAlerta = String.Join("|", s.FluxosAlerta),
                    Situacao = s.Situacao.Name()
                }).OrderByDescending(oAluno => oAluno.DataEntradaQueixa).ToList()
            }).OrderBy(oDre => oDre.DreId).ThenBy(oUe => oUe.UeOrdenacao).ToList();

            var relatorio = new RelatorioEncaminhamentosNAAPADto()
            {
                DreNome = !string.IsNullOrEmpty(filtroRelatorio.DreCodigo) && filtroRelatorio.DreCodigo.Equals("-99") || string.IsNullOrEmpty(filtroRelatorio.DreCodigo) ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().DreNome,
                UeNome = !string.IsNullOrEmpty(filtroRelatorio.UeCodigo) && filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().UeNome,
                UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
            };

            relatorio.EncaminhamentosDreUe = encaminhamentosAgrupados;
            await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad(relatorio, request.CodigoCorrelacao));
        }
    }
}
