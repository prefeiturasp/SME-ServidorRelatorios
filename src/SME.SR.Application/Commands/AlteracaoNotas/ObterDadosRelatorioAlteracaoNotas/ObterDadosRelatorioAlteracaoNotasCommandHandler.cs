using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
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

                if (historicoAlteracaoNotas.Any())
                    listaTurmaAlteracaoNotasDto.Add(await MapearParaTurmaDto(historicoAlteracaoNotas, request.FiltroRelatorio.Bimestres, long.Parse(turma.turma_id), tipoCalendarioId, request.FiltroRelatorio.ModalidadeTurma));
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


        private async Task<TurmaAlteracaoNotasDto> MapearParaTurmaDto(List<HistoricoAlteracaoNotasDto> historicoAlteracaoNotas, IEnumerable<int> bimestres, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var turmaDto = new TurmaAlteracaoNotasDto()
            {
                Nome = historicoAlteracaoNotas.FirstOrDefault().NomeTurma
            };

            foreach (var bimestre in bimestres.OrderBy(o => o))
            {
                turmaDto.Bimestres.Add(await MapearParaBimestreDto(bimestre, historicoAlteracaoNotas, turmaId, tipoCalendarioId, modalidadeTurma));
            }

            return turmaDto;
        }

        private async Task<BimestreAlteracaoNotasDto> MapearParaBimestreDto(int bimestre, List<HistoricoAlteracaoNotasDto> historicoAlteracaoNotas, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var historicoAlteracaoNotasBimestre = historicoAlteracaoNotas.Where(c => c.Bimestre == bimestre);

            var bimestreDto = new BimestreAlteracaoNotasDto()
            {
                Descricao = $"{bimestre}º Bimestre"
            };

            foreach (var historicoAlteracaoNotasComponente in historicoAlteracaoNotasBimestre)
            {
                bimestreDto.ComponentesCurriculares.Add(await MapearParaComponenteDto(historicoAlteracaoNotas, turmaId, tipoCalendarioId, modalidadeTurma));
            }

            return bimestreDto;
        }

        private async Task<ComponenteCurricularAlteracaoNotasDto> MapearParaComponenteDto(List<HistoricoAlteracaoNotasDto> historicoAlteracaoNotas, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {

            var componenteCurricularDto = new ComponenteCurricularAlteracaoNotasDto()
            {
                Nome = historicoAlteracaoNotas.FirstOrDefault().ComponenteCurricularNome
            };
            foreach (var historicoAlteracaoNotasAluno in historicoAlteracaoNotas.OrderBy(c => c.NumeroChamada))
            {
                componenteCurricularDto.AlunosAlteracaoNotasBimestre.Add(await MapearParaAlunoDto(historicoAlteracaoNotasAluno, turmaId, tipoCalendarioId, modalidadeTurma));
            }

            return componenteCurricularDto;
        }


        private async Task<AlunosAlteracaoNotasDto> MapearParaAlunoDto(HistoricoAlteracaoNotasDto historicoAlteracaoNotas, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var AlunoDto = new AlunosAlteracaoNotasDto();

            AlunoDto.NumeroChamada = historicoAlteracaoNotas.NumeroChamada;
            AlunoDto.Nome = historicoAlteracaoNotas.NomeAluno;
            AlunoDto.TipoNota = historicoAlteracaoNotas.TipoNota;
            AlunoDto.NotaAnterior = historicoAlteracaoNotas.NotaAnterior.ToString();
            AlunoDto.NotaAtribuida = historicoAlteracaoNotas.NotaAtribuida.ToString();
            AlunoDto.DataAlteracao = historicoAlteracaoNotas.DataAlteracao.ToString("dd/MM/yyy HH:mm");
            AlunoDto.UsuarioAlteracao = $"{historicoAlteracaoNotas.UsuarioAlteracao} ({historicoAlteracaoNotas.RfAlteracao})";
            AlunoDto.Situacao = historicoAlteracaoNotas.Situacao;
            AlunoDto.UsuarioAprovacao = $"{historicoAlteracaoNotas.UsuarioAprovacao} ({historicoAlteracaoNotas.RfAprovacao})";
            AlunoDto.TipoNotaConceito = historicoAlteracaoNotas.TipoNotaConceito;
            AlunoDto.ConceitoAnterior = historicoAlteracaoNotas.ConceitoAnteriorId;
            AlunoDto.ConceitoAtribuido = historicoAlteracaoNotas.ConceitoAtribuidoId;

            return AlunoDto;
        }
    }
}
