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
            var dto = new RelatorioAlteracaoNotasDto();
            var listaTurmaAlteracaoNotasDto = new List<TurmaAlteracaoNotasDto>();

            bool isAnoAtual = request.FiltroRelatorio.AnoLetivo == DateTime.Now.Year ? true : false;

            var modalidadeCalendario = request.FiltroRelatorio.ModalidadeTurma == Modalidade.EJA ?
                                                ModalidadeTipoCalendario.EJA : request.FiltroRelatorio.ModalidadeTurma == Modalidade.Infantil ?
                                                    ModalidadeTipoCalendario.Infantil : ModalidadeTipoCalendario.FundamentalMedio;

            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(request.FiltroRelatorio.AnoLetivo, modalidadeCalendario, request.FiltroRelatorio.Semestre));

            var turmas = await ObterTurmas(request.FiltroRelatorio.Turma, request.FiltroRelatorio.CodigoUe, request.FiltroRelatorio.AnoLetivo);

            foreach (var turma in turmas)
            {
                try
                {
                    if (!turma.Ano.Equals("0"))
                    {
                        var notaTipoValor = await mediator.Send(new ObterTipoNotaPorTurmaQuery(turma, request.FiltroRelatorio.AnoLetivo));

                        var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery()
                        {
                            TurmaCodigo = turma.turma_id
                        });

                        var historicoAlteracaoNotas = await ObterHistoricoAlteracaoNotas(turma.Codigo, tipoCalendarioId, request.FiltroRelatorio.TipoAlteracaoNota);

                        var nomeTurma = turma.NomeRelatorio;

                        foreach (var historicoNota in historicoAlteracaoNotas)
                        {
                            var alunoAtual = alunos.FirstOrDefault(c => c.CodigoAluno == int.Parse(historicoNota.CodigoAluno));

                            historicoNota.NomeAluno = alunoAtual.NomeAluno;
                            historicoNota.NumeroChamada = alunoAtual.NumeroAlunoChamada;
                            historicoNota.NomeTurma = nomeTurma;
                        }                                             

                        listaTurmaAlteracaoNotasDto.Add(await MapearParaTurmaDto(historicoAlteracaoNotas, request.FiltroRelatorio.Bimestres, request.FiltroRelatorio.AnoLetivo, notaTipoValor.TipoNota));
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }

            if (listaTurmaAlteracaoNotasDto == null || !listaTurmaAlteracaoNotasDto.Any())
            {
                throw new NegocioException("Nenhuma informação para os filtros informados.");
            }
            return listaTurmaAlteracaoNotasDto;
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

        private async Task<List<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotas(string turmaCodigo, long tipoCalendarioId, TipoAlteracaoNota tipoNota)
        {
            var historicoAlteracaoNotas = new List<HistoricoAlteracaoNotasDto>();

            if (tipoNota == TipoAlteracaoNota.Fechamento)
            {
                historicoAlteracaoNotas.AddRange(await ObterHistoricoAlteracaoNotasFechamento(turmaCodigo, tipoCalendarioId));
                return historicoAlteracaoNotas;

            }

            if (tipoNota == TipoAlteracaoNota.ConselhoClasse)
            {
                historicoAlteracaoNotas.AddRange(await ObterHistoricoAlteracaoNotasConselhoClasse(turmaCodigo, tipoCalendarioId));
                return historicoAlteracaoNotas;
            }

            var historicoAlteracaoNotasFechamento = await ObterHistoricoAlteracaoNotasFechamento(turmaCodigo, tipoCalendarioId);
            historicoAlteracaoNotas.AddRange(historicoAlteracaoNotasFechamento);

            var historicoAlteracaoNotasConselhoClasse = await ObterHistoricoAlteracaoNotasConselhoClasse(turmaCodigo, tipoCalendarioId);
            historicoAlteracaoNotas.AddRange(historicoAlteracaoNotasConselhoClasse);

            return historicoAlteracaoNotas;
        }
        private async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(string turmaCodigo, long tipoCalendarioId)
                 => await mediator.Send(new ObterHistoricoNotasFechamentoPorTurmaIdQuery(long.Parse(turmaCodigo), tipoCalendarioId));

        private async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasConselhoClasse(string turmaCodigo, long tipoCalendarioId)
                 => await mediator.Send(new ObterHistoricoNotasConselhoClassePorTurmaIdQuery(long.Parse(turmaCodigo), tipoCalendarioId));


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
                NumeroChamada = historicoAlteracaoNotas.NumeroChamada,
                Nome = ToTitleCase(historicoAlteracaoNotas.NomeAluno),
                TipoAlteracaoNota = historicoAlteracaoNotas.TipoNota.Name(),
                DataAlteracao = historicoAlteracaoNotas.DataAlteracao.ToString("dd/MM/yyy HH:mm"),
                UsuarioAlteracao = ToTitleCase($"{historicoAlteracaoNotas.UsuarioAlteracao} ({historicoAlteracaoNotas.RfAlteracao})"),
                Situacao = historicoAlteracaoNotas.Situacao.Name(),
                UsuarioAprovacao = !string.IsNullOrEmpty(historicoAlteracaoNotas.UsuarioAprovacao) ? $"{ToTitleCase(historicoAlteracaoNotas.UsuarioAprovacao)} ({historicoAlteracaoNotas.RfAprovacao})" : "",
                NotaConceitoAnterior = tipoNotaConceito == TipoNota.Nota ? historicoAlteracaoNotas.NotaAnterior.ToString() : historicoAlteracaoNotas.ConceitoAnteriorId.Name(),
                NotaConceitoAtribuido = tipoNotaConceito == TipoNota.Nota ? historicoAlteracaoNotas.NotaAtribuida.ToString() : historicoAlteracaoNotas.ConceitoAtribuidoId.Name(),
            };

            return AlunosAlteracaoNotasDto;
        }

        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
