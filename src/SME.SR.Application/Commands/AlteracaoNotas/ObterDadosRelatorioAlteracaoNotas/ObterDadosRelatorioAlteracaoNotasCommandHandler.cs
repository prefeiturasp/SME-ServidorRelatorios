﻿using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioAlteracaoNotasCommandHandler : IRequestHandler<ObterDadosRelatorioAlteracaoNotasCommand, List<TurmaAlteracaoNotasDto>>
    {
        private readonly IMediator mediator;

        public ObterDadosRelatorioAlteracaoNotasCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<List<TurmaAlteracaoNotasDto>> Handle(ObterDadosRelatorioAlteracaoNotasCommand request, CancellationToken cancellationToken)
        {
            var listaTurmaAlteracaoNotasDto = new List<TurmaAlteracaoNotasDto>();

            var modalidadeCalendario = request.FiltroRelatorio.ModalidadeTurma.ObterModalidadeTipoCalendario();

            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(request.FiltroRelatorio.AnoLetivo, modalidadeCalendario, request.FiltroRelatorio.Semestre));

            var turmas = await ObterTurmas(request.FiltroRelatorio.Turma, request.FiltroRelatorio.CodigoUe, request.FiltroRelatorio.AnoLetivo);

            foreach (var turma in turmas)
            {
                
                    if (!turma.Ano.Equals("0"))
                    {
                        var notaTipoValor = await mediator.Send(new ObterTipoNotaPorTurmaQuery(turma, request.FiltroRelatorio.AnoLetivo));

                        var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery()
                        {
                            TurmaCodigo = turma.Codigo
                        });

                        var historicoAlteracaoNotas = await ObterHistoricoAlteracaoNotas(turma.Id, tipoCalendarioId, request.FiltroRelatorio.TipoAlteracaoNota, request.FiltroRelatorio.Bimestres, request.FiltroRelatorio.ComponentesCurriculares);

                        var nomeTurma = turma.NomeRelatorio;

                        foreach (var historicoNota in historicoAlteracaoNotas)
                        {
                            var alunoAtual = alunos.FirstOrDefault(c => c.CodigoAluno == int.Parse(historicoNota.CodigoAluno));

                            if(alunoAtual != null)
                            {
                                historicoNota.NomeAluno = alunoAtual.NomeAluno;
                                historicoNota.NumeroChamada = alunoAtual.NumeroAlunoChamada;
                                historicoNota.NomeTurma = nomeTurma;
                            }                            
                        }

                        await CarregarTipoNotaEdfisica(turma, historicoAlteracaoNotas);

                        if (historicoAlteracaoNotas != null && historicoAlteracaoNotas.Any())
                            listaTurmaAlteracaoNotasDto.Add(await MapearParaTurmaDto(historicoAlteracaoNotas, request.FiltroRelatorio.Bimestres, request.FiltroRelatorio.AnoLetivo, notaTipoValor.TipoNota));
                    }
            }

            if (listaTurmaAlteracaoNotasDto == null || !listaTurmaAlteracaoNotasDto.Any())
            {
                throw new NegocioException("Nenhuma informação para os filtros informados.");
            }
            return listaTurmaAlteracaoNotasDto.OrderBy(x => x.Nome).ToList();
        }

        private async Task<IEnumerable<Turma>> ObterTurmas(IEnumerable<long> FiltroTurmas, string codigoUe, long anoLetivo)
        {
            long[] turmasId = new long[] { };
            IEnumerable<Turma> turmas = new List<Turma>();

            if (FiltroTurmas.Any(c => c == -99))
            {
                turmas = await mediator.Send(new ObterTurmasPorUeEAnoLetivoQuery(codigoUe, anoLetivo));
            }
            else
            {
                turmasId = FiltroTurmas.ToArray();
                turmas = await mediator.Send(new ObterTurmasPorIdsQuery(turmasId));
            }
            return turmas;
        }

        private async Task<List<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotas(long turmaId, long tipoCalendarioId, TipoAlteracaoNota tipoNota, int[] bimestres, long[] componentes)
        {
            var historicoAlteracaoNotas = new List<HistoricoAlteracaoNotasDto>();

            if (tipoNota == TipoAlteracaoNota.Fechamento)
            {
                historicoAlteracaoNotas.AddRange(await ObterHistoricoAlteracaoNotasFechamento(turmaId, tipoCalendarioId, bimestres, componentes));
                return historicoAlteracaoNotas;

            }

            if (tipoNota == TipoAlteracaoNota.ConselhoClasse)
            {
                historicoAlteracaoNotas.AddRange(await ObterHistoricoAlteracaoNotasConselhoClasse(turmaId, tipoCalendarioId, bimestres, componentes));
                return historicoAlteracaoNotas;
            }

            var historicoAlteracaoNotasFechamento = await ObterHistoricoAlteracaoNotasFechamento(turmaId, tipoCalendarioId, bimestres, componentes);
            historicoAlteracaoNotas.AddRange(historicoAlteracaoNotasFechamento);

            var historicoAlteracaoNotasConselhoClasse = await ObterHistoricoAlteracaoNotasConselhoClasse(turmaId, tipoCalendarioId, bimestres, componentes);
            historicoAlteracaoNotas.AddRange(historicoAlteracaoNotasConselhoClasse);

            return historicoAlteracaoNotas;
        }
        private async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(long turmaId, long tipoCalendarioId, int[] bimestres, long[] componentes)
                 => await mediator.Send(new ObterHistoricoNotasFechamentoPorTurmaIdQuery(turmaId, tipoCalendarioId, bimestres, componentes));

        private async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasConselhoClasse(long turmaId, long tipoCalendarioId, int[] bimestres, long[] componentes)
                 => await mediator.Send(new ObterHistoricoNotasConselhoClassePorTurmaIdQuery(turmaId, tipoCalendarioId, bimestres, componentes));


        private async Task<TurmaAlteracaoNotasDto> MapearParaTurmaDto(List<HistoricoAlteracaoNotasDto> historicoAlteracaoNotas, IEnumerable<int> bimestres, int anoLetivo, TipoNota tiponota)
        {
            var turmaDto = new TurmaAlteracaoNotasDto()
            {
                Nome = historicoAlteracaoNotas.FirstOrDefault().NomeTurma,
                AnoAtual = anoLetivo == DateTime.Now.Year ? true : false,
                TipoNotaConceitoDesc = tiponota.Name().ToUpper(),
                TipoNotaConceito = tiponota,
            };

            turmaDto.Bimestres.AddRange(await MapearParaBimestreDto(historicoAlteracaoNotas, tiponota));

            return turmaDto;
        }

        private async Task<List<BimestreAlteracaoNotasDto>> MapearParaBimestreDto(List<HistoricoAlteracaoNotasDto> historicoAlteracaoNotas, TipoNota tiponota)
        {
            var bimestresDto = new List<BimestreAlteracaoNotasDto>();


            foreach (var historicoAlteracaoNotasComponente in historicoAlteracaoNotas.GroupBy(c => c.Bimestre).OrderBy(d => d.Key))
            {

                var bimestreDto = new BimestreAlteracaoNotasDto();

                bimestreDto.Descricao = historicoAlteracaoNotasComponente.FirstOrDefault().Bimestre == 0 ?
                                        $"Bimestre Final"
                                        :
                                        $"{historicoAlteracaoNotasComponente.FirstOrDefault().Bimestre}º Bimestre";

                foreach (var historicoAlteracaoNotasAluno in historicoAlteracaoNotasComponente.GroupBy(c => c.DisciplinaId))
                {
                    bimestreDto.ComponentesCurriculares.Add(await MapearParaComponenteDto(historicoAlteracaoNotasAluno, tiponota));
                }

                bimestresDto.Add(bimestreDto);
            }

            return bimestresDto;
        }

        private async Task<ComponenteCurricularAlteracaoNotasDto> MapearParaComponenteDto(IGrouping<long, HistoricoAlteracaoNotasDto> historicoAlteracaoNotasComponente, TipoNota tiponota)
        {
            var componenteCurricularDto = new ComponenteCurricularAlteracaoNotasDto()
            {
                Nome = historicoAlteracaoNotasComponente.FirstOrDefault().ComponenteCurricularNome
            };
            foreach (var historicoAlteracaoNotasAluno in historicoAlteracaoNotasComponente)
            {
                componenteCurricularDto.AlunosAlteracaoNotasBimestre.Add(await MapearParaAlunoDto(historicoAlteracaoNotasAluno, tiponota));
            }

            return componenteCurricularDto;
        }


        private async Task<AlunosAlteracaoNotasDto> MapearParaAlunoDto(HistoricoAlteracaoNotasDto historicoAlteracaoNotas, TipoNota tipoNotaConceito)
        {
            var AlunosAlteracaoNotasDto = new AlunosAlteracaoNotasDto()
            {
                NumeroChamada = string.IsNullOrEmpty(historicoAlteracaoNotas.NumeroChamada) ? "0" : historicoAlteracaoNotas.NumeroChamada.TrimStart(new Char[] {'0'}),
                Nome = string.IsNullOrEmpty(historicoAlteracaoNotas.NomeAluno) ? "" : ToTitleCase(historicoAlteracaoNotas.NomeAluno),
                TipoAlteracaoNota = historicoAlteracaoNotas.TipoNota.Name(),
                DataAlteracao = historicoAlteracaoNotas.DataAlteracao.ToString("dd/MM/yyy HH:mm"),
                UsuarioAlteracao = string.IsNullOrEmpty(historicoAlteracaoNotas.UsuarioAlteracao) ? "" : ToTitleCase($"{historicoAlteracaoNotas.UsuarioAlteracao} ({historicoAlteracaoNotas.RfAlteracao})"),
                Situacao = historicoAlteracaoNotas.Situacao == 0 ? " - " : historicoAlteracaoNotas.Situacao.Name(),
                UsuarioAprovacao = !string.IsNullOrEmpty(historicoAlteracaoNotas.UsuarioAprovacao) ? $"{ToTitleCase(historicoAlteracaoNotas.UsuarioAprovacao)} ({historicoAlteracaoNotas.RfAprovacao})" : " - ",
                NotaConceitoAnterior = historicoAlteracaoNotas.ObterNotaConceitoAnterior(tipoNotaConceito),
                NotaConceitoAtribuido = historicoAlteracaoNotas.ObterNotaConceitoAtribuido(tipoNotaConceito),
                EmAprovacao = historicoAlteracaoNotas.EmAprovacao
            };

            AlunosAlteracaoNotasDto.NotaConceitoAtribuido = AlunosAlteracaoNotasDto.EmAprovacao ? $"{AlunosAlteracaoNotasDto.NotaConceitoAtribuido}*" : AlunosAlteracaoNotasDto.NotaConceitoAtribuido;

            return AlunosAlteracaoNotasDto;
        }

        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        private async Task CarregarTipoNotaEdfisica(Turma turma, List<HistoricoAlteracaoNotasDto> historicoAlteracao)
        {
            if (turma.EhEja && 
                turma.TipoTurma == TipoTurma.EdFisica &&
                historicoAlteracao != null && 
                historicoAlteracao.Exists(historico => historico.TipoNota == TipoAlteracaoNota.Fechamento))
            {
                await ConverteValorNotaPorTurmaEja(turma, historicoAlteracao.FindAll(historico => historico.TipoNota == TipoAlteracaoNota.Fechamento));
            }
        }

        private async Task ConverteValorNotaPorTurmaEja(Turma turmaEja, List<HistoricoAlteracaoNotasDto> historicoAlteracao)
        {
            var codigosAlunos = historicoAlteracao.Select(nc => nc.CodigoAluno)?.Distinct().ToArray();
            var tipoNotaAluno = await mediator.Send(new ObterTipoTurmaRegularParaEdFisicaQuery(turmaEja, codigosAlunos));

            foreach (var historico in historicoAlteracao)
            {
                if (tipoNotaAluno.ContainsKey(historico.CodigoAluno))
                    historico.CarregaTipoNota(tipoNotaAluno[historico.CodigoAluno]);
            }
        }

    }
}
