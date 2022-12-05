using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
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
            // var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioPlanosAeeDto>();
            var filtroRelatorio = new FiltroRelatorioPlanosAeeDto()
            {
                AnoLetivo = 2022,
                //DreCodigo = "109200",
                DreCodigo = "-99",
                // UeCodigo = "099792",
                UeCodigo = "-99",
                Modalidade = -99,
                //Semestre = 1,
                //CodigosTurma = new []{"2476297","2355626","2355676"},
                CodigosTurma = new []{"-99"},
                //ExibirEncerrados = true,
                //Situacao = 9,
                // CodigosResponsavel = new []{"8030308","7941331"},
                // CodigosResponsavel = new []{"6943314"},
                // PAAIResponsavel = "8150168",
                UsuarioNome = "Vinícius Nyari",
                UsuarioRf = "97910201087"
            };

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

            var relatorioPlanosAEE = new RelatorioPlanosAeeDto()
            {
                Cabecalho = new CabecalhoPlanosAeeDto()
                {
                    DreNome = filtroRelatorio.DreCodigo.Equals("-99") ? "TODAS" : planosAgrupados.FirstOrDefault().DreNome,
                    UeNome = filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : planosAgrupados.FirstOrDefault().UeNome,
                    UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
                },
                AgrupamentosDreUe = planosAgrupados
            };

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPlanosAEE", relatorioPlanosAEE, request.CodigoCorrelacao, "", "Relatório do Plano AEE", true));
        }
    }
}