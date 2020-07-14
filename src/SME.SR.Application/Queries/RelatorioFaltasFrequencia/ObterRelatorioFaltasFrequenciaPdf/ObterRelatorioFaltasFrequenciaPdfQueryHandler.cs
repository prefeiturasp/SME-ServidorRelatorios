using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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


                                foreach (var aluno in componente.Alunos)
                                {
                                    var alunoAtual = alunos.SingleOrDefault(c => c.CodigoAluno == aluno.CodigoAluno && c.TurmaCodigo == aluno.CodigoTurma);
                                    if (alunoAtual != null)
                                    {
                                        aluno.NomeAluno = alunoAtual.Nome;
                                        aluno.NumeroChamada = alunoAtual.NumeroChamada;
                                    }
                                }

                                alunosSemFrequenciaNaTurma = alunos.Where(a => !componente.Alunos.Any(c => c.CodigoAluno == a.CodigoAluno && c.CodigoTurma == a.TurmaCodigo));
                                if (alunosSemFrequenciaNaTurma != null && alunosSemFrequenciaNaTurma.Any())
                                {
                                    componente.Alunos.AddRange(alunosSemFrequenciaNaTurma.Select(c => new RelatorioFaltaFrequenciaAlunoDto
                                    {
                                        CodigoAluno = c.CodigoAluno,
                                        NomeTurma = componente.Alunos.FirstOrDefault(a => a.CodigoTurma == c.TurmaCodigo)?.NomeTurma,
                                        NomeAluno = c.Nome,
                                        NumeroChamada = c.NumeroChamada,
                                        TotalAusencias = 0,
                                        TotalCompensacoes = 0,
                                        TotalAulas = componente.Alunos.FirstOrDefault().TotalAulas
                                    }));
                                }


                                componente.Alunos = (from a in componente.Alunos
                                                     where
                                                     ((filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Faltas || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos) ?
                                                        operacao[filtro.Condicao](a.NumeroFaltasNaoCompensadas, filtro.ValorCondicao)
                                                     :
                                                        operacao[filtro.Condicao](a.Frequencia, filtro.ValorCondicao))
                                                     orderby string.IsNullOrWhiteSpace(a.NumeroChamada), a.NumeroChamada
                                                     select a).ToList();
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

                                    componenteAtual.Alunos = (from a in componenteAtual.Alunos
                                                              where
                                                              ((filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Faltas || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos) ?
                                                                 operacao[filtro.Condicao](a.NumeroFaltasNaoCompensadas, filtro.ValorCondicao)
                                                              :
                                                                 operacao[filtro.Condicao](a.Frequencia, filtro.ValorCondicao))
                                                              orderby string.IsNullOrWhiteSpace(a.NumeroChamada), a.NumeroChamada
                                                              select a).ToList();
                                }

                            }
                            ano.Bimestres.Add(final);
                        }
                        if (mostrarSomenteFinal)
                            ano.Bimestres.RemoveAll(c => c.NomeBimestre != "Final");
                    }
                }
            }

            var selecionouTodasDres = string.IsNullOrWhiteSpace(filtro.CodigoDre) || filtro.CodigoDre == "-99";
            var selecionouTodasUes = string.IsNullOrWhiteSpace(filtro.CodigoUe) || filtro.CodigoUe == "-99";

            model.Dres = dres.ToList();
            model.Dre = selecionouTodasDres ? "Todas" : dres.FirstOrDefault().NomeDre;
            model.Ue = selecionouTodasUes ? "Todas" : dres.FirstOrDefault().Ues.FirstOrDefault().NomeUe;
            model.Ano = filtro.AnosEscolares.Count() > 1 ? string.Empty : filtro.AnosEscolares.FirstOrDefault();

            model.Bimestre = filtro.Bimestres.Count() > 1 ? string.Empty : filtro.Bimestres.FirstOrDefault() == 0 ? "Todos" : $"{filtro.Bimestres.FirstOrDefault()}º Bimestre";

            var selecionouTodosComponentes = filtro.ComponentesCurriculares.Any(c => c == "-99");

            model.ComponenteCurricular = selecionouTodosComponentes ? "Todos" : string.Empty;

            model.Usuario = request.Filtro.NomeUsuario;
            model.RF = request.Filtro.CodigoRf;
            model.Data = DateTime.Now.ToString("dd/MM/yyyy");
            model.ExibeFaltas = filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Faltas || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos;
            model.ExibeFrequencia = filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Frequencia || filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ambos;


            return await Task.FromResult(model);
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
