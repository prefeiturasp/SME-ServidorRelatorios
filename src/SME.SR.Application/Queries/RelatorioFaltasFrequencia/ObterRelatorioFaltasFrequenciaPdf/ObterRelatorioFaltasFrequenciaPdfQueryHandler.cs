using DocumentFormat.OpenXml.Spreadsheet;
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
                        var faltasFrequenciaFinal = new List<RelatorioFaltaFrequenciaBimestreDto>();

                        foreach (var bimestre in ano.Bimestres)
                        {
                            bimestre.NomeBimestre = $"{bimestre.NomeBimestre}º Bimestre";
                            foreach (var componente in bimestre.Componentes)
                            {
                                var componenteAtual = componentes.FirstOrDefault(c => c.Codigo.ToString() == componente.CodigoComponente);
                                if (componenteAtual != null)
                                    componente.NomeComponente = componenteAtual.Descricao;

                                IEnumerable<AlunoTurma> alunosSemFrequenciaNaTurma;

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

                                alunosSemFrequenciaNaTurma = alunos.Where(a => !componente.Alunos.Any(c => c.CodigoAluno == a.CodigoAluno && c.CodigoTurma == a.TurmaCodigo));
                                if (alunosSemFrequenciaNaTurma != null && alunosSemFrequenciaNaTurma.Any())
                                {
                                    componente.Alunos.AddRange(alunosSemFrequenciaNaTurma.Select(c => new RelatorioFaltaFrequenciaAlunoDto
                                    {
                                        CodigoAluno = c.CodigoAluno,
                                        NomeTurma = turmas.FirstOrDefault(a => a.Codigo == c.TurmaCodigo)?.Nome,
                                        NomeAluno = c.NomeFinal,
                                        NumeroChamada = c.NumeroChamada,
                                        TotalAusencias = 0,
                                        TotalCompensacoes = 0,
                                        TotalAulas = componente.Alunos.FirstOrDefault().TotalAulas
                                    }));
                                }
                                OrdenarAlunos(filtro, operacao, componente);
                            }
                        }
                        if (deveAdicionarFinal)
                        {
                            var final = new RelatorioFaltaFrequenciaBimestreDto
                            {
                                NomeBimestre = "Final",
                            };

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

                                    OrdenarAlunos(filtro, operacao, componenteAtual);
                                }

                            }
                            ano.Bimestres.Add(final);
                        }
                        if (mostrarSomenteFinal)
                            ano.Bimestres.RemoveAll(c => c.NomeBimestre != "Final");
                    }
                }
            }

            DefinirCabecalho(request, model, filtro, dres, componentes);

            return await Task.FromResult(model);
        }

        private static void DefinirCabecalho(ObterRelatorioFaltasFrequenciaPdfQuery request, RelatorioFaltasFrequenciaDto model, FiltroRelatorioFaltasFrequenciasDto filtro, IEnumerable<RelatorioFaltaFrequenciaDreDto> dres, IEnumerable<Data.ComponenteCurricular> componentes)
        {
            var selecionouTodasDres = string.IsNullOrWhiteSpace(filtro.CodigoDre) || filtro.CodigoDre == "-99";
            var selecionouTodasUes = string.IsNullOrWhiteSpace(filtro.CodigoUe) || filtro.CodigoUe == "-99";

            model.Dres = dres.ToList();
            model.Dre = selecionouTodasDres ? "Todas" : dres.FirstOrDefault().NomeDre;
            model.Ue = selecionouTodasUes ? "Todas" : dres.FirstOrDefault().Ues.FirstOrDefault().NomeUe;
            model.Ano = filtro.AnosEscolares.Count() > 1 ? string.Empty : filtro.AnosEscolares.FirstOrDefault();

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

        private async Task<IEnumerable<FrequenciaAluno>> ObterFaltasEFrequencias(IEnumerable<string> turmas, IEnumerable<int> bimestresFiltro, IEnumerable<long> componentesCurriculares, Modalidade modalidade, int anoLetivo, int semestre)
        {
            var faltasFrequenciasAlunos = new List<FrequenciaAluno>();

            // Obter bimestres diferentes de "Final"
            var bimestres = bimestresFiltro.Where(c => c != 0);
            if (bimestres != null && bimestres.Any())
            {
                faltasFrequenciasAlunos.AddRange(await mediator.Send(new ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(turmas, bimestres, componentesCurriculares)));
            }

            // Verifica se foi solicitado bimestre final
            if (bimestresFiltro.Any(c => c == 0))
            {
                var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(anoLetivo,
                                                                                                                 modalidade == Modalidade.EJA ? ModalidadeTipoCalendario.EJA : ModalidadeTipoCalendario.FundamentalMedio,
                                                                                                                 semestre)); ;

                faltasFrequenciasAlunos.AddRange(await mediator.Send(new ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery(turmas, componentesCurriculares, modalidade, tipoCalendarioId)));
            }

            return faltasFrequenciasAlunos;
        }
    }
}
