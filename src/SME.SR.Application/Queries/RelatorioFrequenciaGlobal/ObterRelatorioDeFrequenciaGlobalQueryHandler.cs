using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioDeFrequenciaGlobalQueryHandler : IRequestHandler<ObterRelatorioDeFrequenciaGlobalQuery, List<FrequenciaGlobalDto>>
    {
        private readonly VariaveisAmbiente variaveisAmbiente;


        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;
        private readonly IMediator mediator;

        public ObterRelatorioDeFrequenciaGlobalQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository, IMediator mediator, VariaveisAmbiente variaveisAmbiente)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<List<FrequenciaGlobalDto>> Handle(ObterRelatorioDeFrequenciaGlobalQuery request, CancellationToken cancellationToken)
        {
            var retornoQuery = await frequenciaAlunoRepository.ObterFrequenciaAlunoMensal(request.Filtro.ExibirHistorico, request.Filtro.AnoLetivo,
                request.Filtro.CodigoDre, request.Filtro.CodigoUe, request.Filtro.Modalidade, request.Filtro.Semestre, request.Filtro.CodigosTurmas.Select(c => c).ToArray(),
                request.Filtro.MesesReferencias.Select(c => Convert.ToInt32(c)).ToArray(), request.Filtro.ApenasAlunosPercentualAbaixoDe);
            if (retornoQuery == null || !retornoQuery.Any())
                return Enumerable.Empty<FrequenciaGlobalDto>().ToList();
            return await MapearRetornoQuery(request.Filtro, retornoQuery);
        }

        private async Task<List<FrequenciaGlobalDto>> MapearRetornoQuery(FiltroFrequenciaGlobalDto filtro, IEnumerable<FrequenciaAlunoMensalConsolidadoDto> retornoQuery)
        {
            var retornoMapeado = new ConcurrentBag<FrequenciaGlobalDto>();
            var codigosAlunosParaFiltrar = retornoQuery.Select(r => r.CodigoEol).Distinct().ToArray();
            var dadosMatriculaAlunos = await ObterMatriculasAlunos(filtro.CodigoUe, filtro.CodigoDre, filtro.AnoLetivo, codigosAlunosParaFiltrar);
            var matriculasAgrupadas = dadosMatriculaAlunos.ToLookup(x => $"{x.CodigoAluno}-{x.CodigoTurma}");

            retornoQuery
                .AsParallel()
                .WithDegreeOfParallelism(variaveisAmbiente.ProcessamentoMaximoUes)
                .GroupBy(x => x.UeCodigo)
                .ForAll(agrupamentoUe =>
            {
                foreach (var item in agrupamentoUe)
                {
                    var chave = $"{item.CodigoEol}-{item.TurmaCodigo}";
                    var matriculasDoAluno = matriculasAgrupadas[chave].ToList();
                    
                    if (!matriculasDoAluno.Any())
                        continue;
                    
                        var dadosSituacaoAluno = DeveImprimirNoRelatorio(matriculasDoAluno, item.Mes, filtro.AnoLetivo);
                        if (dadosSituacaoAluno.ImprimirRelatorio)
                        {
                            retornoMapeado.Add(new FrequenciaGlobalDto()
                            {
                                SiglaDre = item.DreSigla,
                                DreCodigo = item.DreCodigo,
                                UeNome = string.Concat(item.DescricaoTipoEscola, " - ", item.UeNome),
                                UeCodigo = item.UeCodigo,
                                Mes = item.Mes,
                                TurmaCodigo = item.TurmaCodigo,
                                Turma = string.Concat(ObterModalidade(item.ModalidadeCodigo).ShortName(), " - ", item.TurmaNome),
                                CodigoEOL = item.CodigoEol,
                                Estudante = dadosSituacaoAluno.NomeFinalAluno,
                                NumeroChamadda = dadosSituacaoAluno.NroChamada,
                                PercentualFrequencia = item.Percentual,
                                QuantidadeCompensacoesAusencias = item.QuantidadeCompensacoes,
                                QuantidadeAusencias = item.QuantidadeAusencias,
                                QuantidadeAulas = item.QuantidadeAulas
                            });
                    }
                }
            });

            return retornoMapeado
                .OrderBy(c => c.SiglaDre)
                .ThenBy(c => c.UeNome)
                .ThenBy(c => c.Mes)
                .ThenBy(c => c.Turma)
                .ThenBy(c => c.Estudante)
                .ToList();
        }

        public class AgrupamentoAlunoTurma
        {
            public string CodigoAluno { get; set; }
            public string CodigoTurma { get; set; }
        }

        private async Task<IEnumerable<DadosMatriculaAlunoDto>> ObterMatriculasAlunos(string codigoUe, string codigoDre, int anoLetivo, string[] codigosAlunos)
        {
            var alunos = await mediator.Send(new ObterDadosAlunosEscolaQuery(codigoUe, codigoDre, anoLetivo, codigosAlunos));

            return alunos;
        }

        private UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto DeveImprimirNoRelatorio(List<DadosMatriculaAlunoDto> dadosMatricula, int mesSelecionado, int anoLetivoSelecionado)
        {
            var dadosSituacaoAluno = new UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto { ImprimirRelatorio = false };
            var itemsMesSelecionado = FiltrarMatriculasConsideradasNoMes(dadosMatricula, mesSelecionado, anoLetivoSelecionado);

            if (itemsMesSelecionado.Any())
            {
                var ultimoRegistroMatricula = itemsMesSelecionado.Last();
                var inativoNoMes = ultimoRegistroMatricula.Inativo && ultimoRegistroMatricula.DataSituacao.Month == mesSelecionado;

                dadosSituacaoAluno.ImprimirRelatorio = true;
                dadosSituacaoAluno.NomeFinalAluno = ultimoRegistroMatricula.ObterNomeFinal();
                dadosSituacaoAluno.NroChamada = ultimoRegistroMatricula.NumeroAlunoChamada;

                if (inativoNoMes)
                    dadosSituacaoAluno.NomeFinalAluno += " - " + ultimoRegistroMatricula.SituacaoMatricula
                        + " " + ultimoRegistroMatricula.DataSituacao.ToString("dd/MM/yyyy");
            }

            return dadosSituacaoAluno;
        }

        private Modalidade ObterModalidade(int modalidadeCodigo)
        {
            return Enum.GetValues(typeof(Modalidade))
                .Cast<Modalidade>().FirstOrDefault(x => (int)x == modalidadeCodigo);
        }

        private IEnumerable<DadosMatriculaAlunoDto> FiltrarMatriculasConsideradasNoMes(List<DadosMatriculaAlunoDto> dadosMatricula, int mesConsiderado, int anoLetivoSelecionado)
        {
            if (dadosMatricula.Any(x => x.AnoLetivo == anoLetivoSelecionado))
            {
                return dadosMatricula.Last().Ativo && dadosMatricula.First().DataMatricula.AntecedeMesAno(mesConsiderado, anoLetivoSelecionado) ||
                       dadosMatricula.Last().Inativo && dadosMatricula.Last().DataSituacao.PosteriorOuEquivalenteMesAno(mesConsiderado, anoLetivoSelecionado)
                       ? dadosMatricula
                       : Enumerable.Empty<DadosMatriculaAlunoDto>();
            }
            else
                return dadosMatricula.Last().Ativo ? dadosMatricula : Enumerable.Empty<DadosMatriculaAlunoDto>();
        }
    }
}
