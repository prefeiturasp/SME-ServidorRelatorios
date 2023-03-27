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
    public class ObterRelatorioFrequenciaPdfQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfQuery, RelatorioFrequenciaDto>
    {
        private readonly IRelatorioFrequenciaRepository relatorioFaltasFrequenciaRepository;
        private readonly IMediator mediator;
        private readonly ITurmaRepository turmaRepository;

        public ObterRelatorioFrequenciaPdfQueryHandler(IRelatorioFrequenciaRepository relatorioFaltasFrequenciaRepository,
                                                             IMediator mediator,
                                                             ITurmaRepository turmaRepository)
        {
            this.relatorioFaltasFrequenciaRepository = relatorioFaltasFrequenciaRepository ?? throw new ArgumentNullException(nameof(relatorioFaltasFrequenciaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<RelatorioFrequenciaDto> Handle(ObterRelatorioFrequenciaPdfQuery request, CancellationToken cancellationToken)
        {
            var relatorioFrequenciaDto = new RelatorioFrequenciaDto();
            var filtro = request.Filtro;

            filtro.AnosEscolares ??= new[] { new string("-99") };

            if (filtro.TurmasPrograma)
                filtro.AnosEscolares.Concat(new[] { new string("0") });

            filtro.CodigosTurma ??= new List<string> { new string("-99") };


            var dres = await relatorioFaltasFrequenciaRepository.ObterFrequenciaPorAno(filtro.AnoLetivo,
                                                                                       filtro.CodigoDre,
                                                                                       filtro.CodigoUe,
                                                                                       filtro.Modalidade,
                                                                                       filtro.AnosEscolares,
                                                                                       filtro.ComponentesCurriculares,
                                                                                       filtro.Bimestres,
                                                                                       filtro.TipoRelatorio,
                                                                                       filtro.CodigosTurma);

            if (dres == null || !dres.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var componentes = await mediator.Send(new ObterListaComponentesCurricularesQuery());

            var codigosTurmas = dres.SelectMany(d => d.Ues)
                .SelectMany(u => u.TurmasAnos)
                .SelectMany(u => u.Bimestres)
                .SelectMany(u => u.Componentes)
                .SelectMany(u => u.Alunos)
                .Select(u => int.Parse(u.CodigoTurma))
                .Distinct()
                .ToArray();

            var alunos = await mediator
                .Send(new ObterAlunosMatriculasPorTurmasQuery(codigosTurmas));

            var turmas = await turmaRepository
                .ObterTurmasPorAnoEModalidade(filtro.AnoLetivo, filtro.AnosEscolares.ToArray(), filtro.Modalidade);

            if (turmas == null || !turmas.Any())
                throw new NegocioException("Turmas não localizadas para os anos informados.");

            var deveAdicionarFinal = filtro.Bimestres.Any(c => c == 0);
            var mostrarSomenteFinal = deveAdicionarFinal && filtro.Bimestres.Count() == 1;

            var tipoCalendarioId = await mediator
                .Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(filtro.AnoLetivo, filtro.ModalidadeTipoCalendario, filtro.Semestre));

            var periodosEscolares = await mediator
                .Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));

            foreach (var dre in dres)
            {
                relatorioFrequenciaDto.Dres.Add(await mediator.Send(new ObterRelatorioFrequenciaPdfPorDreQuery(request.Filtro, dre.Ues, periodosEscolares, componentes, alunos, turmas, deveAdicionarFinal, mostrarSomenteFinal)));
                // var ultimoAluno = 
                // if (!string.IsNullOrEmpty(ultimoAluno))
                //     relatorioFrequenciaDto.UltimoAluno = ultimoAluno;
                
                relatorioFrequenciaDto.UltimoAluno = $"{dre.NomeDre}{relatorioFrequenciaDto.UltimoAluno}";
            }

            DefinirCabecalho(request, relatorioFrequenciaDto, filtro, dres, componentes);
            relatorioFrequenciaDto.Dres = FiltrarFaltasFrequencia(relatorioFrequenciaDto.Dres, filtro);

            if (relatorioFrequenciaDto.Dres == null || !relatorioFrequenciaDto.Dres.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            return await Task.FromResult(relatorioFrequenciaDto);
        }

        private async Task<List<RelatorioFrequenciaAlunoDto>> NovaBuscaAlunosSemFrequencia(RelatorioFrequenciaComponenteDto componente,
                                                                                           List<string> codigosTurmas,
                                                                                           List<Turma> turmas,
                                                                                           int bimestre,
                                                                                           List<AlunoTurma> alunosSemFrequencia,
                                                                                           Modalidade Modalidade)
        {
            List<RelatorioFrequenciaAlunoDto> novosAlunos = new List<RelatorioFrequenciaAlunoDto>();
            var frequencias = await mediator.Send(new ObterFrequenciasAlunosConsolidadoQuery(codigosTurmas.ToArray(), componente.CodigoComponente, bimestre.ToString()));
            if (frequencias != null && frequencias.Any())
            {
                var alunosComFrequencia = alunosSemFrequencia.Where(asf => frequencias.Any(f => f.AlunoCodigo == asf.CodigoAluno.ToString()));
                foreach (var aluno in alunosComFrequencia)
                {
                    var frequenciaAluno = frequencias.FirstOrDefault(f => f.AlunoCodigo == aluno.CodigoAluno.ToString() && f.TurmaCodigo == aluno.TurmaCodigo);

                    var turmaFiltrada = turmas.FirstOrDefault(a => a.Codigo == frequenciaAluno.TurmaCodigo);

                    novosAlunos.Add(new RelatorioFrequenciaAlunoDto()
                    {
                        CodigoAluno = aluno.CodigoAluno,
                        NomeAluno = aluno.NomeFinal,
                        NumeroChamada = aluno.NumeroChamada ?? "0",
                        TotalPresenca = frequenciaAluno.TotalPresencas,
                        TotalRemoto = frequenciaAluno.TotalRemotos,
                        TotalAusencias = frequenciaAluno.TotalAusencias,
                        NomeTurma = turmaFiltrada == null ? "" : $"{Modalidade.ShortName()}-{turmaFiltrada.Nome}",
                        CodigoTurma = aluno.TurmaCodigo,
                        TotalAulas = frequenciaAluno.TotalAulas
                    });
                }
            }
            return novosAlunos;
        }

        private List<RelatorioFrequenciaDreDto> FiltrarFaltasFrequencia(List<RelatorioFrequenciaDreDto> dres, FiltroRelatorioFrequenciasDto filtro)
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
                        foreach (var ano in ue.TurmasAnos)
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
                        ue.TurmasAnos.RemoveAll(c => !c.Bimestres.Any());
                    }
                    dre.Ues.RemoveAll(c => !c.TurmasAnos.Any());
                }
                dres.RemoveAll(c => !c.Ues.Any());
                dres = dres.OrderBy(c => c.NomeDre).ToList();
            }
            return dres;
        }

        private static void DefinirCabecalho(ObterRelatorioFrequenciaPdfQuery request, RelatorioFrequenciaDto model, FiltroRelatorioFrequenciasDto filtro, IEnumerable<RelatorioFrequenciaDreDto> dres, IEnumerable<Data.ComponenteCurricular> componentes)
        {
            var selecionouTodasDres = string.IsNullOrWhiteSpace(filtro.CodigoDre) || filtro.CodigoDre == "-99";
            var selecionouTodasUes = string.IsNullOrWhiteSpace(filtro.CodigoUe) || filtro.CodigoUe == "-99";
            var selecionoutodasTurmas = filtro.CodigosTurma != null ? filtro.CodigosTurma.Any(c => c == "-99") : false;



            model.Dres = dres.ToList();
            model.Dres.RemoveAll(c => !c.Ues.Any());
            model.Cabecalho.Dre = selecionouTodasDres ? "Todas" : dres.FirstOrDefault().NomeDre;
            model.Cabecalho.Ue = selecionouTodasUes ? "Todas" : dres.FirstOrDefault().Ues.FirstOrDefault().NomeUe;
            model.Cabecalho.Turma = selecionoutodasTurmas
                ?
                "Todas"
                :
                filtro.CodigosTurma.Count() > 1
                ?
                ""
                :
                dres.FirstOrDefault()
                        .Ues.FirstOrDefault()
                            .TurmasAnos.FirstOrDefault()
                              .Bimestres.FirstOrDefault()
                                .Componentes.FirstOrDefault()
                                   .Alunos.FirstOrDefault().NomeTurma;


            var selecionouTodosAnos = filtro.AnosEscolares.Any(c => c == "-99");
            var ano = filtro.AnosEscolares.FirstOrDefault();

            if (request.Filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
            {
                model.Cabecalho.Ano = selecionouTodosAnos && !(filtro.Modalidade == Modalidade.EJA) ?
                "Todos"
                :
                    filtro.AnosEscolares.Count() > 1 ?
                    string.Empty
                :
                    ano == "-99" ? "Todos" : ano;
            }
            else
                model.Cabecalho.Ano = string.Empty;

            DefinirNomeBimestre(model, filtro);
            DefinirNomeComponente(model, filtro, componentes);

            model.Cabecalho.Usuario = request.Filtro.NomeUsuario;
            model.Cabecalho.RF = request.Filtro.CodigoRf;
        }

        private static void DefinirNomeComponente(RelatorioFrequenciaDto model, FiltroRelatorioFrequenciasDto filtro, IEnumerable<Data.ComponenteCurricular> componentes)
        {
            var selecionouTodosComponentes = filtro.ComponentesCurriculares.Any(c => c == "-99");
            var primeiroComponente = filtro.ComponentesCurriculares.FirstOrDefault();

            model.Cabecalho.ComponenteCurricular = selecionouTodosComponentes ?
                "Todos"
                :
                filtro.ComponentesCurriculares.Count() == 1 ?
                componentes.FirstOrDefault(c => c.Codigo.ToString() == primeiroComponente).Descricao
                :
                string.Empty;
        }

        private static void DefinirNomeBimestre(RelatorioFrequenciaDto model, FiltroRelatorioFrequenciasDto filtro)
        {
            var selecionouTodosBimestres = false;
            if (filtro.Modalidade != Modalidade.EJA)
                selecionouTodosBimestres = filtro.Bimestres.Count() == 5;
            else
                selecionouTodosBimestres = filtro.Bimestres.Count() == 3;

            var selecionouBimestreFinal = filtro.Bimestres.Any(c => c == 0);

            model.Cabecalho.Bimestre = selecionouTodosBimestres ?
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

        private static void OrdenarAlunos(FiltroRelatorioFrequenciasDto filtro, Dictionary<CondicoesRelatorioFaltasFrequencia, Func<double, double, bool>> operacao, RelatorioFrequenciaComponenteDto componente)
        {
            if (filtro.Condicao != CondicoesRelatorioFaltasFrequencia.TodosEstudantes)
            {
                componente.Alunos = (from a in componente.Alunos
                                     where
                                     ((filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ano) ?
                                        operacao[filtro.Condicao](a.NumeroFaltasNaoCompensadas, filtro.QuantidadeAusencia)
                                     :
                                        operacao[filtro.Condicao](a.TotalAusencias, filtro.QuantidadeAusencia))
                                     select a)
                                     .Where(a => !string.IsNullOrWhiteSpace(a.NomeAluno) && !string.IsNullOrWhiteSpace(a.NumeroChamada))
                                     .OrderByDescending(c => !string.IsNullOrWhiteSpace(c.NumeroChamada))
                                     .ThenBy(c => c.NomeTurma)
                                     .ThenBy(c => c.NomeAluno)
                                     .DistinctBy(c => c.CodigoAluno)
                                     .ToList();
            }
            else
            {
                componente.Alunos = (from a in componente.Alunos select a)
                                 .Where(a => !string.IsNullOrWhiteSpace(a.NomeAluno) && !string.IsNullOrWhiteSpace(a.NumeroChamada))
                                 .OrderByDescending(c => !string.IsNullOrWhiteSpace(c.NumeroChamada))
                                 .ThenBy(c => c.NomeTurma)
                                 .ThenBy(c => c.NomeAluno)
                                 .DistinctBy(c => c.CodigoAluno)
                                 .ToList();
            }
        }

        private async Task AjustarBimestresSemFaltas(int anoLetivo, int semestre, List<RelatorioFrequenciaComponenteDto> componentes, Modalidade modalidade, List<RelatorioFrequenciaBimestreDto> bimestres)
        {
            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(anoLetivo,
                                                                                                        modalidade == Modalidade.EJA ? ModalidadeTipoCalendario.EJA : ModalidadeTipoCalendario.FundamentalMedio,
                                                                                                        semestre));

            for (int numeroBimestre = 1; numeroBimestre <= (modalidade == Modalidade.EJA ? 2 : 4); numeroBimestre++)
            {
                var bimestreAtual = bimestres.FirstOrDefault(c => c.Numero == numeroBimestre.ToString());
                if (bimestreAtual == null)
                {
                    bimestreAtual = new RelatorioFrequenciaBimestreDto
                    {
                        Numero = numeroBimestre.ToString(),
                        NomeBimestre = $"{numeroBimestre}° Bimestre".ToUpper()
                    };

                    foreach (var componente in componentes)
                    {
                        var novoComponente = new RelatorioFrequenciaComponenteDto();
                        novoComponente.CodigoComponente = componente.CodigoComponente;
                        novoComponente.NomeComponente = componente.NomeComponente.ToUpper();
                        var totalAulas = await mediator.Send(new ObterAulasDadasNoBimestreQuery(componente.Alunos.FirstOrDefault(c => c.CodigoTurma != null).CodigoTurma, tipoCalendarioId, long.Parse(componente.CodigoComponente), numeroBimestre));
                        foreach (var aluno in componente.Alunos)
                        {
                            var novoAluno = new RelatorioFrequenciaAlunoDto();
                            novoAluno.TotalAulas = totalAulas;
                            novoAluno.TotalAusencias = 0;
                            novoAluno.TotalCompensacoes = 0;
                            novoAluno.NomeAluno = aluno.NomeAluno;
                            novoAluno.NomeTurma = aluno.NomeTurma;
                            novoAluno.NumeroChamada = aluno.NumeroChamada ?? "0";
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
