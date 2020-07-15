using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IMediator mediator;

        public ObterRelatorioFaltasFrequenciaPdfQueryHandler(IRelatorioFaltasFrequenciaRepository relatorioFaltasFrequenciaRepository,
                                                             IComponenteCurricularRepository componenteCurricularRepository,
                                                             IMediator mediator)
        {
            this.relatorioFaltasFrequenciaRepository = relatorioFaltasFrequenciaRepository ?? throw new ArgumentNullException(nameof(relatorioFaltasFrequenciaRepository));
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFaltasFrequenciaDto> Handle(ObterRelatorioFaltasFrequenciaPdfQuery request, CancellationToken cancellationToken)
        {
            var model = new RelatorioFaltasFrequenciaDto();
            var filtro = request.Filtro;

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

            var componentes = await componenteCurricularRepository.ListarComponentes();

            var codigosTurmas = dres.SelectMany(d => d.Ues)
                .SelectMany(u => u.Anos)
                .SelectMany(u => u.Bimestres)
                .SelectMany(u => u.Componentes)
                .SelectMany(u => u.Alunos)
                .Select(u => u.CodigoTurma)
                .Distinct();

            var alunos = await mediator.Send(new ObterAlunosPorAnoQuery(codigosTurmas));

            var turmas = await mediator.Send(new ObterTurmasPorAnoQuery(filtro.AnoLetivo, filtro.AnosEscolares));
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
                                    var alunoAtual = alunos.SingleOrDefault(c => c.CodigoAluno == aluno.CodigoAluno && c.TurmaCodigo == aluno.CodigoTurma);
                                    if (alunoAtual != null)
                                    {
                                        aluno.NomeAluno = alunoAtual.NomeFinal;
                                        aluno.NumeroChamada = alunoAtual.NumeroChamada;
                                    }
                                }
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
            }

            DefinirCabecalho(request, model, filtro, dres, componentes);

            if (model.Dres == null || !model.Dres.Any())
            {
                throw new NegocioException("Nenhuma informação para os filtros informados.");
            }

            FiltrarFaltasFrequencia(model.Dres, filtro);
            return await Task.FromResult(model);
        }

        private void FiltrarFaltasFrequencia(List<RelatorioFaltaFrequenciaDreDto> dres, FiltroRelatorioFaltasFrequenciasDto filtro)
        {
            Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>> operacao = new Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>>();
            operacao.Add(CondicoesRelatorioFaltasFrequencia.Igual, (valor, valorFiltro) => valor == valorFiltro);
            operacao.Add(CondicoesRelatorioFaltasFrequencia.Maior, (valor, valorFiltro) => valor > valorFiltro);
            operacao.Add(CondicoesRelatorioFaltasFrequencia.Menor, (valor, valorFiltro) => valor < valorFiltro);

            foreach (var dre in dres)
            {
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
                        }
                        ano.Bimestres.RemoveAll(c => !c.Componentes.Any());
                    }
                    ue.Anos.RemoveAll(c => !c.Bimestres.Any());
                }
                dre.Ues.RemoveAll(c => !c.Anos.Any());
            }
            dres.RemoveAll(c => !c.Ues.Any());
        }

        private static void DefinirCabecalho(ObterRelatorioFaltasFrequenciaPdfQuery request, RelatorioFaltasFrequenciaDto model, FiltroRelatorioFaltasFrequenciasDto filtro, IEnumerable<RelatorioFaltaFrequenciaDreDto> dres, IEnumerable<Data.ComponenteCurricular> componentes)
        {
            var selecionouTodasDres = string.IsNullOrWhiteSpace(filtro.CodigoDre) || filtro.CodigoDre == "-99";
            var selecionouTodasUes = string.IsNullOrWhiteSpace(filtro.CodigoUe) || filtro.CodigoUe == "-99";

            model.Dres = dres.ToList();
            model.Dres.RemoveAll(c => !c.Ues.Any());
            model.Dre = selecionouTodasDres ? "Todas" : dres.FirstOrDefault().NomeDre;
            model.Ue = selecionouTodasUes ? "Todas" : dres.FirstOrDefault().Ues.FirstOrDefault().NomeUe;
            model.Ano = filtro.AnosEscolares.Count() > 1 ?
                filtro.AnosEscolares.Any(c => c == "-99") ?
                        "Todos"
                    :
                        string.Empty
                :
                    $"{filtro.AnosEscolares.FirstOrDefault()} Ano";

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

            model.Modalidade = $"{filtro.Modalidade.Name()}{semestreEja}";
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
                $"{filtro.Bimestres.FirstOrDefault()}º Bimestre";
        }

        private static void OrdenarAlunos(FiltroRelatorioFaltasFrequenciasDto filtro, Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>> operacao, RelatorioFaltaFrequenciaComponenteDto componente)
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
                        Componentes = componentes.Select(c => new RelatorioFaltaFrequenciaComponenteDto
                        {
                            Alunos = c.Alunos.Select(a => new RelatorioFaltaFrequenciaAlunoDto
                            {
                                CodigoAluno = a.CodigoAluno,
                                CodigoTurma = a.CodigoTurma,
                                NomeAluno = a.NomeAluno,
                                NomeTurma = a.NomeTurma,
                                NumeroChamada = a.NumeroChamada,
                                TotalAulas = 0,
                                TotalAusencias = 0,
                                TotalCompensacoes = 0
                            }).ToList(),
                            CodigoComponente = c.CodigoComponente,
                            NomeComponente = c.NomeComponente
                        }).ToList(),
                        Numero = numeroBimestre.ToString(),
                        NomeBimestre = $"{numeroBimestre}° Bimestre"
                    };

                    foreach (var componente in bimestreAtual.Componentes)
                    {
                        foreach (var aluno in componente.Alunos)
                        {
                            aluno.TotalAulas = await mediator.Send(new ObterAulasDadasNoBimestreQuery(aluno.CodigoTurma, tipoCalendarioId, long.Parse(componente.CodigoComponente), numeroBimestre));
                            aluno.TotalAusencias = 0;
                            aluno.TotalCompensacoes = 0;
                        }
                    }
                    bimestres.Add(bimestreAtual);
                }
            }
        }
    }
}
