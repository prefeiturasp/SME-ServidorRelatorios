using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SME.SR.Application.Interfaces;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.RelatorioPaginado;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Application
{
    public class RelatorioPlanosAeeUseCase : IRelatorioPlanosAeeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanosAeeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioPlanosAeeDto>();
            var planosAee = await mediator.Send(new ObterPlanosAEEQuery(filtroRelatorio));
            var planosAgrupados = planosAee.GroupBy(g => new
            { 
                DreNome = g.DreAbreviacao, 
                UeNome = $"{g.UeCodigo} - {g.TipoEscola.ShortName()} {g.UeNome}",
            }, (key, group) => 
                new AgrupamentoDreUeDto() { 
                    DreNome = key.DreNome, 
                    UeNome = key.UeNome, 
                    Detalhes = group.Select(s=>
                    new DetalhePlanosAeeDto()
                    {
                        Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                        Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}",
                        Situacao = ((SituacaoPlanoAee)s.SituacaoPlano).Name(),
                        Responsavel = !string.IsNullOrEmpty(s.ResponsavelNome) ? $"{s.ResponsavelNome} ({s.ResponsavelLoginRf})" : string.Empty,
                        Versao = $"v{s.VersaoPlano} - {s.DataVersaoPlano:dd/MM/yyyy}",
                        ResponsavelPAAI = !string.IsNullOrEmpty(s.ResponsavelPaaiNome) ? $"{s.ResponsavelPaaiNome} ({s.ResponsavelPaaiLoginRf})" : string.Empty,
                    }).OrderBy(oAluno=> oAluno.Aluno).ToList()
                }).OrderBy(oUe=> oUe.UeNome).ToList();

            var cabecalho = new CabecalhoPlanosAeeDto()
            {
                DreNome = filtroRelatorio.DreCodigo.Equals("-99") ? "TODAS" : planosAgrupados.FirstOrDefault().DreNome,
                UeNome = filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : planosAgrupados.FirstOrDefault().UeNome,
                UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
            };

            await mediator.Send(new GerarRelatorioHtmlPDFPlanosAeeCommand(cabecalho, planosAgrupados, request.CodigoCorrelacao));
        }
    }
}