﻿using MediatR;
using SME.SR.Data;
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
        private const string FILTRO_OPCAO_TODOS = "-99";
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
            var alunosEscola = await ObterMatriculasAlunos(filtro.CodigoUe, filtro.CodigoDre, filtro.AnoLetivo, retornoQuery);
            var agrupamento = alunosEscola
                .OrderBy(x => x.CodigoAluno)
                .ThenBy(x => x.CodigoMatricula)
                .ThenBy(x => x.DataMatricula)
                .ThenBy(x => x.DataSituacao)
                .GroupBy(x => new { CodigoAluno = x.CodigoAluno.ToString(), CodigoTurma = x.CodigoTurma })
                .ToDictionary(g => $"{g.Key.CodigoAluno.ToString()}-{g.Key.CodigoTurma}", g => g.ToList());

            retornoQuery
                .AsParallel()
                .WithDegreeOfParallelism(variaveisAmbiente.ProcessamentoMaximoUes)
                .GroupBy(x => x.UeCodigo)
                .ForAll(agrupamentoUe =>
            { 
                    foreach (var item in agrupamentoUe)
                    {
                        if (agrupamento.TryGetValue($"{item.CodigoEol}-{item.TurmaCodigo}", out var alunoAgrupado))
                        {
                            var dadosSituacaoAluno = DeveImprimirNoRelatorio(alunoAgrupado, item.Mes, filtro.AnoLetivo);
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
                                        PercentualFrequencia = item.Percentual
                                    });
                            }
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

        private async Task<IEnumerable<DadosAlunosEscolaDto>> ObterMatriculasAlunos(string codigoUe, string codigoDre, int anoLetivo, IEnumerable<FrequenciaAlunoMensalConsolidadoDto> frequencias)
        {
            List<DadosAlunosEscolaDto> alunos = new List<DadosAlunosEscolaDto>();
            List<Dre> dres;
            if (string.IsNullOrEmpty(codigoDre) || (codigoDre == FILTRO_OPCAO_TODOS))
                dres = frequencias.Select(x => x.DreCodigo).Distinct().Select(x => new Dre() { Codigo = x }).ToList();
            else
                dres = new List<Dre> { await mediator.Send(new ObterDrePorCodigoQuery(codigoDre)) };
            foreach (var dre in dres)
                alunos.AddRange(await mediator
                                .Send(new ObterDadosAlunosEscolaQuery(codigoUe, dre.Codigo, anoLetivo, null)));

            return alunos;
        }

        private UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto DeveImprimirNoRelatorio(List<DadosAlunosEscolaDto> agrupamento, int mesSelecionado, int anoLetivoSelecionado)
        {
            var dadosSituacaoAluno = new UltimaSituacaoAlunoRelatorioFrequenciaGlobalDto { ImprimirRelatorio = false };
            var itemsMesSelecionado = FiltrarMatriculasConsideradasNoMes(agrupamento, mesSelecionado, anoLetivoSelecionado);

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

        private IEnumerable<DadosAlunosEscolaDto> FiltrarMatriculasConsideradasNoMes(List<DadosAlunosEscolaDto> lista, int mesConsiderado, int anoLetivoSelecionado)
        {
            if (lista.Any(x => x.AnoLetivo == anoLetivoSelecionado))
            {
                return lista.Last().Ativo && lista.First().DataMatricula.AntecedeMesAno(mesConsiderado, anoLetivoSelecionado) ||
                       lista.Last().Inativo && lista.Last().DataSituacao.PosteriorOuEquivalenteMesAno(mesConsiderado, anoLetivoSelecionado)
                       ? lista
                       : Enumerable.Empty<DadosAlunosEscolaDto>();
            }
            else
                return lista.Last().Ativo ? lista : Enumerable.Empty<DadosAlunosEscolaDto>();
        }
    }
}
