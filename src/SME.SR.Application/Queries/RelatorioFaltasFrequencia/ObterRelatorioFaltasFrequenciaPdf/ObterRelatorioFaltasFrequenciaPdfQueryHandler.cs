﻿using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaPdfQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciaPdfQuery, RelatorioFaltasFrequenciaDto>
    {
        private readonly IRelatorioFaltasFrequenciaRepository relatorioFaltasFrequenciaRepository;
        private readonly IMediator mediator;
        private readonly ITurmaRepository turmaRepository;

        public ObterRelatorioFaltasFrequenciaPdfQueryHandler(IRelatorioFaltasFrequenciaRepository relatorioFaltasFrequenciaRepository,
                                                             IMediator mediator,
                                                             ITurmaRepository turmaRepository)
        {
            this.relatorioFaltasFrequenciaRepository = relatorioFaltasFrequenciaRepository ?? throw new ArgumentNullException(nameof(relatorioFaltasFrequenciaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<RelatorioFaltasFrequenciaDto> Handle(ObterRelatorioFaltasFrequenciaPdfQuery request, CancellationToken cancellationToken)
        {
            var model = new RelatorioFaltasFrequenciaDto();
            var filtro = request.Filtro;
            if (filtro.TurmasPrograma)
                filtro.AnosEscolares = filtro.AnosEscolares.Concat(new[] { new string("0") });            

            var dres = await relatorioFaltasFrequenciaRepository.ObterFaltasFrequenciaPorAno(filtro.AnoLetivo,
                                                                                          filtro.CodigoDre,
                                                                                          filtro.CodigoUe,
                                                                                          filtro.Modalidade,
                                                                                          filtro.AnosEscolares,
                                                                                          filtro.ComponentesCurriculares,
                                                                                          filtro.Bimestres);

            if (dres == null || !dres.Any())
            {
                throw new NegocioException("Nenhuma informação para os filtros informados.");
            }

            var componentes = await mediator.Send(new ObterListaComponentesCurricularesQuery());

            var codigosTurmas = dres.SelectMany(d => d.Ues)
                .SelectMany(u => u.Anos)
                .SelectMany(u => u.Bimestres)
                .SelectMany(u => u.Componentes)
                .SelectMany(u => u.Alunos)
                .Select(u => u.CodigoTurma)
                .Distinct();

            var alunos = await mediator.Send(new ObterAlunosPorAnoQuery(codigosTurmas));

            var turmas = await turmaRepository.ObterTurmasPorAnoEModalidade(filtro.AnoLetivo, filtro.AnosEscolares.ToArray(), filtro.Modalidade);
            if (turmas == null || !turmas.Any())
                throw new NegocioException("Turmas não localizadas para os anos informados.");

            var deveAdicionarFinal = filtro.Bimestres.Any(c => c == 0);
            var mostrarSomenteFinal = deveAdicionarFinal && filtro.Bimestres.Count() == 1;

            foreach (var dre in dres)
            {
                foreach (var ue in dre.Ues)
                {
                    foreach (var ano in ue.Anos)
                    {
                        ano.NomeAno = ano.NomeAno.Equals("0º ano") ? "Turma de Programa" : ano.NomeAno;
                        foreach (var bimestre in ano.Bimestres)
                        {
                            bimestre.NomeBimestre = $"{bimestre.NomeBimestre}º Bimestre";
                            foreach (var componente in bimestre.Componentes)
                            {
                                var componenteAtual = componentes.FirstOrDefault(c => c.Codigo.ToString() == componente.CodigoComponente);
                                if (componenteAtual != null)
                                    componente.NomeComponente = componenteAtual.Descricao;


                                for (int a = 0; a < componente.Alunos.Count; a++)
                                {
                                    var aluno = componente.Alunos[a];
                                    var alunoAtual = alunos.FirstOrDefault(c => c.CodigoAluno == aluno.CodigoAluno && c.TurmaCodigo == aluno.CodigoTurma);
                                    if (alunoAtual != null)
                                    {
                                        aluno.NomeAluno = alunoAtual.NomeFinal;
                                        aluno.NumeroChamada = alunoAtual.NumeroChamada;
                                    }
                                }

                                var turmasccc = componente.Alunos.Select(c => c.CodigoTurma).Distinct().ToList();
                                var alunosSemFrequenciaNaTurma = alunos
                                    .Where(a => a.Ativo)
                                    .Where(a => turmasccc.Contains(a.TurmaCodigo))
                                    .Where(a => !componente.Alunos.Any(c => c.CodigoAluno == a.CodigoAluno));

                                if (alunosSemFrequenciaNaTurma != null && alunosSemFrequenciaNaTurma.Any())
                                {
                                    var sem = alunosSemFrequenciaNaTurma.Select(c => new RelatorioFaltaFrequenciaAlunoDto
                                    {
                                        CodigoAluno = c.CodigoAluno,
                                        NomeTurma = turmas.FirstOrDefault(a => a.Codigo == c.TurmaCodigo)?.Nome,
                                        NomeAluno = c.NomeFinal,
                                        NumeroChamada = c.NumeroChamada,
                                        TotalAusencias = 0,
                                        TotalCompensacoes = 0,
                                        TotalAulas = componente.Alunos.FirstOrDefault()?.TotalAulas ?? 0
                                    }).ToList();
                                    componente.Alunos.AddRange(sem);
                                }

                                model.UltimoAluno = componente.Alunos.LastOrDefault().CodigoAluno.ToString();
                            }
                        }

                        if (deveAdicionarFinal)
                        {
                            var final = new RelatorioFaltaFrequenciaBimestreDto
                            {
                                NomeBimestre = "Final",
                            };

                            var componentesFinal = ano.Bimestres.SelectMany(c => c.Componentes).DistinctBy(c => c.CodigoComponente).ToList();
                            await AjustarBimestresSemFaltas(filtro.AnoLetivo, filtro.Semestre, componentesFinal, filtro.Modalidade, ano.Bimestres);
                            foreach (var bimestre in ano.Bimestres.ToList())
                            {
                                for (int c = 0; c < bimestre.Componentes.Count; c++)
                                {
                                    var componente = bimestre.Componentes[c];

                                    var componenteAtual = final.Componentes.FirstOrDefault(c => c.CodigoComponente == componente.CodigoComponente);
                                    if (componenteAtual == null)
                                    {
                                        componenteAtual = new RelatorioFaltaFrequenciaComponenteDto();
                                        componenteAtual.NomeComponente = componente.NomeComponente;
                                        componenteAtual.CodigoComponente = componente.CodigoComponente;
                                        final.Componentes.Add(componenteAtual);
                                    }

                                    for (int a = 0; a < componente.Alunos.Count; a++)
                                    {
                                        var aluno = componente.Alunos[a];
                                        var codigoAluno = aluno.CodigoAluno;
                                        var alunoAtual = componenteAtual.Alunos.FirstOrDefault(c => c.CodigoAluno == codigoAluno);
                                        if (alunoAtual == null)
                                        {
                                            alunoAtual = new RelatorioFaltaFrequenciaAlunoDto();
                                            alunoAtual.CodigoAluno = aluno.CodigoAluno;
                                            alunoAtual.NomeAluno = aluno.NomeAluno;
                                            alunoAtual.NomeTurma = aluno.NomeTurma;
                                            alunoAtual.NumeroChamada = aluno.NumeroChamada;
                                            componenteAtual.Alunos.Add(alunoAtual);
                                        }
                                        alunoAtual.TotalAulas += aluno.TotalAulas;
                                        alunoAtual.TotalAusencias += aluno.TotalAusencias;
                                        alunoAtual.TotalCompensacoes += aluno.TotalCompensacoes;
                                        alunoAtual.NomeAluno = aluno.NomeAluno;
                                        alunoAtual.NomeTurma = aluno.NomeTurma;
                                        alunoAtual.NumeroChamada = aluno.NumeroChamada;
                                    }
                                }

                            }
                            ano.Bimestres.Add(final);
                        }
                        if (mostrarSomenteFinal)
                            ano.Bimestres.RemoveAll(c => c.NomeBimestre != "Final");

                        ano.Bimestres = ano.Bimestres.OrderBy(c => c.NomeBimestre).ToList();
                    }
                }
                model.UltimoAluno = $"{dre.NomeDre}{model.UltimoAluno}";
            }

            DefinirCabecalho(request, model, filtro, dres, componentes);

            model.Dres = FiltrarFaltasFrequencia(model.Dres, filtro);
            if (model.Dres == null || !model.Dres.Any())
            {
                throw new NegocioException("Nenhuma informação para os filtros informados.");
            }
            return await Task.FromResult(model);
        }

        private List<RelatorioFaltaFrequenciaDreDto> FiltrarFaltasFrequencia(List<RelatorioFaltaFrequenciaDreDto> dres, FiltroRelatorioFaltasFrequenciasDto filtro)
        {
            Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>> operacao = new Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>>();
            operacao.Add(CondicoesRelatorioFaltasFrequencia.Igual, (valor, valorFiltro) => valor == valorFiltro);
            operacao.Add(CondicoesRelatorioFaltasFrequencia.Maior, (valor, valorFiltro) => valor > valorFiltro);
            operacao.Add(CondicoesRelatorioFaltasFrequencia.Menor, (valor, valorFiltro) => valor < valorFiltro);

            if (dres != null)
            {
                foreach (var dre in dres)
                {
                    if (dre.Ues != null)
                        dre.Ues = dre.Ues.OrderBy(c => c.NomeUe).ToList();
                    foreach (var ue in dre.Ues)
                    {
                        foreach (var ano in ue.Anos)
                        {
                            foreach (var bimestre in ano.Bimestres)
                            {
                                foreach (var componente in bimestre.Componentes)
                                {
                                    OrdenarAlunos(filtro, operacao, componente);
                                }
                                bimestre.Componentes.RemoveAll(c => !c.Alunos.Any());
                                bimestre.Componentes = bimestre.Componentes.OrderBy(c => c.NomeComponente).ToList();
                            }
                            ano.Bimestres.RemoveAll(c => !c.Componentes.Any());
                        }
                        ue.Anos.RemoveAll(c => !c.Bimestres.Any());
                    }
                    dre.Ues.RemoveAll(c => !c.Anos.Any());
                }
                dres.RemoveAll(c => !c.Ues.Any());
                dres = dres.OrderBy(c => c.NomeDre).ToList();
            }
            return dres;
        }

        private static void DefinirCabecalho(ObterRelatorioFaltasFrequenciaPdfQuery request, RelatorioFaltasFrequenciaDto model, FiltroRelatorioFaltasFrequenciasDto filtro, IEnumerable<RelatorioFaltaFrequenciaDreDto> dres, IEnumerable<Data.ComponenteCurricular> componentes)
        {
            var selecionouTodasDres = string.IsNullOrWhiteSpace(filtro.CodigoDre) || filtro.CodigoDre == "-99";
            var selecionouTodasUes = string.IsNullOrWhiteSpace(filtro.CodigoUe) || filtro.CodigoUe == "-99";

            model.Dres = dres.ToList();
            model.Dres.RemoveAll(c => !c.Ues.Any());
            model.Dre = selecionouTodasDres ? "Todas" : dres.FirstOrDefault().NomeDre;
            model.Ue = selecionouTodasUes ? "Todas" : dres.FirstOrDefault().Ues.FirstOrDefault().NomeUe;

            var selecionouTodosAnos = filtro.AnosEscolares.Any(c => c == "-99");
            var ano = filtro.AnosEscolares.FirstOrDefault();
            model.Ano = selecionouTodosAnos && !(filtro.Modalidade == Modalidade.EJA) ?
                "Todos"
                :
                    filtro.AnosEscolares.Count() > 1 ?
                    string.Empty
                :
                    ano == "-99" ? "Todos" : ano;

            DefinirNomeBimestre(model, filtro);
            DefinirNomeComponente(model, filtro, componentes);

            model.Usuario = request.Filtro.NomeUsuario;
            model.RF = request.Filtro.CodigoRf;
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.ExibeFaltas = filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Faltas || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos;
            model.ExibeFrequencia = filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Frequencia || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos;

            var semestreEja = "";
            if (filtro.Modalidade == Modalidade.EJA)
                semestreEja = $"{filtro.Semestre} Semestre";

            model.Modalidade = $"{filtro.Modalidade.Name()} {semestreEja}";
        }

        private static void DefinirNomeComponente(RelatorioFaltasFrequenciaDto model, FiltroRelatorioFaltasFrequenciasDto filtro, IEnumerable<Data.ComponenteCurricular> componentes)
        {
            var selecionouTodosComponentes = filtro.ComponentesCurriculares.Any(c => c == "-99");
            var primeiroComponente = filtro.ComponentesCurriculares.FirstOrDefault();

            model.ComponenteCurricular = selecionouTodosComponentes ?
                "Todos"
                :
                filtro.ComponentesCurriculares.Count() == 1 ?
                componentes.FirstOrDefault(c => c.Codigo.ToString() == primeiroComponente).Descricao
                :
                string.Empty;
        }

        private static void DefinirNomeBimestre(RelatorioFaltasFrequenciaDto model, FiltroRelatorioFaltasFrequenciasDto filtro)
        {
            var selecionouTodosBimestres = filtro.Bimestres.Any(c => c == -99);
            var selecionouBimestreFinal = filtro.Bimestres.Any(c => c == 0);

            model.Bimestre = selecionouTodosBimestres ?
                "Todos"
                :
                filtro.Bimestres.Count() > 1 ?
                string.Empty
                :
                selecionouBimestreFinal ?
                "Final"
                :
                filtro.Bimestres.FirstOrDefault().ToString();
        }

        private static void OrdenarAlunos(FiltroRelatorioFaltasFrequenciasDto filtro, Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>> operacao, RelatorioFaltaFrequenciaComponenteDto componente)
        {
            if (filtro.Condicao != CondicoesRelatorioFaltasFrequencia.TodosEstudantes)
            {
                componente.Alunos = (from a in componente.Alunos
                                     where
                                     ((filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Faltas || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos) ?
                                        operacao[filtro.Condicao](a.NumeroFaltasNaoCompensadas, filtro.ValorCondicao)
                                     :
                                        operacao[filtro.Condicao](a.Frequencia, filtro.ValorCondicao))
                                     select a)
                                     .OrderByDescending(c => !string.IsNullOrWhiteSpace(c.NumeroChamada))
                                     .ThenBy(c => c.NomeTurma)
                                     .ThenBy(c => c.NomeAluno)
                                     .ToList();
            }
            else
            {
                componente.Alunos = (from a in componente.Alunos select a).OrderByDescending(c => !string.IsNullOrWhiteSpace(c.NumeroChamada))
                                 .ThenBy(c => c.NomeTurma)
                                 .ThenBy(c => c.NomeAluno)
                                 .ToList();
            }
        }

        private async Task AjustarBimestresSemFaltas(int anoLetivo, int semestre, List<RelatorioFaltaFrequenciaComponenteDto> componentes, Modalidade modalidade, List<RelatorioFaltaFrequenciaBimestreDto> bimestres)
        {
            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(anoLetivo,
                                                                                                        modalidade == Modalidade.EJA ? ModalidadeTipoCalendario.EJA : ModalidadeTipoCalendario.FundamentalMedio,
                                                                                                        semestre));

            for (int numeroBimestre = 1; numeroBimestre <= (modalidade == Modalidade.EJA ? 2 : 4); numeroBimestre++)
            {
                var bimestreAtual = bimestres.FirstOrDefault(c => c.Numero == numeroBimestre.ToString());
                if (bimestreAtual == null)
                {
                    bimestreAtual = new RelatorioFaltaFrequenciaBimestreDto
                    {
                        Numero = numeroBimestre.ToString(),
                        NomeBimestre = $"{numeroBimestre}° Bimestre"
                    };

                    foreach (var componente in componentes)
                    {
                        var novoComponente = new RelatorioFaltaFrequenciaComponenteDto();
                        novoComponente.CodigoComponente = componente.CodigoComponente;
                        novoComponente.NomeComponente = componente.NomeComponente;
                        foreach (var aluno in componente.Alunos)
                        {
                            var novoAluno = new RelatorioFaltaFrequenciaAlunoDto();
                            novoAluno.TotalAulas = await mediator.Send(new ObterAulasDadasNoBimestreQuery(aluno.CodigoTurma, tipoCalendarioId, long.Parse(componente.CodigoComponente), numeroBimestre));
                            novoAluno.TotalAusencias = 0;
                            novoAluno.TotalCompensacoes = 0;
                            novoAluno.NomeAluno = aluno.NomeAluno;
                            novoAluno.NomeTurma = aluno.NomeTurma;
                            novoAluno.NumeroChamada = aluno.NumeroChamada;
                            novoAluno.CodigoAluno = aluno.CodigoAluno;

                            if (novoAluno.TotalAulas > 0)
                                novoComponente.Alunos.Add(novoAluno);
                        }
                        if (novoComponente.Alunos.Any())
                            bimestreAtual.Componentes.Add(novoComponente);
                    }
                    if (bimestreAtual.Componentes.Any())
                        bimestres.Add(bimestreAtual);
                }
            }
        }
    }
}
