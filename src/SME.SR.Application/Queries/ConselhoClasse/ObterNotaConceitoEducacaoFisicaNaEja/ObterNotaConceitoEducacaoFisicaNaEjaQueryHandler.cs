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

namespace SME.SR.Application
{
    public class ObterNotaConceitoEducacaoFisicaNaEjaQueryHandler : IRequestHandler<ObterNotaConceitoEducacaoFisicaNaEjaQuery, IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto>>
    {
        private readonly IMediator mediator;
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private readonly IPeriodoFechamentoRepository periodoFechamentoRepository;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;
        private readonly ICicloRepository cicloRepository; 
        private readonly INotaTipoRepository notaTipoRepository;
        private const double NOTA_CONCEITO_CINCO = 5.0;
        private const double NOTA_CONCEITO_SETE = 7.0;
        private readonly long? COMPONENTECURRICULARCODIGOEDFISICA = 6;

        public ObterNotaConceitoEducacaoFisicaNaEjaQueryHandler(IMediator mediator, IPeriodoEscolarRepository periodoEscolarRepository,
            IPeriodoFechamentoRepository periodoFechamentoRepository, ITipoCalendarioRepository tipoCalendarioRepository, ICicloRepository cicloRepository,
            INotaTipoRepository notaTipoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.periodoFechamentoRepository = periodoFechamentoRepository ?? throw new ArgumentNullException(nameof(periodoFechamentoRepository));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository));
            this.notaTipoRepository = notaTipoRepository ?? throw new ArgumentNullException(nameof(notaTipoRepository));
        }

        public async Task<IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto>> Handle(ObterNotaConceitoEducacaoFisicaNaEjaQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoNotaTipoNotaDtoEducacaoFisicaDto>();
            var turmaAlunos = request.Turma;
            IEnumerable<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto> codigosTurmasRelacionadas = null;
            var turmasitinerarioEnsinoMedio = (await mediator.Send(new ObterTurmaItinerarioEnsinoMedioQuery())).ToList();
            if (turmaAlunos.EhTurmaEdFisicaOuItinerario() || turmasitinerarioEnsinoMedio.Any(a => a.Id == (int)turmaAlunos.TipoTurma))
            {
                var turmasCodigosParaConsulta = new List<int>();
                turmasCodigosParaConsulta.AddRange(turmaAlunos.ObterTiposRegularesDiferentes());
                codigosTurmasRelacionadas = await mediator.Send(new ObterTurmaCodigosEAlunosPorAnoLetivoAlunoTipoTurmaQuery(turmaAlunos.AnoLetivo,request.AlunosCodigos, turmasCodigosParaConsulta,false));
            }

            await MapearRetorno(retorno, turmaAlunos, codigosTurmasRelacionadas, request.AlunosCodigos, turmasitinerarioEnsinoMedio);

            return retorno;
        }

        private async Task MapearRetorno(List<AlunoNotaTipoNotaDtoEducacaoFisicaDto> retorno, Turma turma, IEnumerable<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto> codigosTurmasRelacionadas, string[] alunosCodigos, List<TurmaItinerarioEnsinoMedioDto> turmasitinerarioEnsinoMedio)
        {
           
            foreach (var aluno in alunosCodigos)
            {
                var turmaAluno = turma;
                if (turma.EhTurmaEdFisicaOuItinerario() || turmasitinerarioEnsinoMedio.Any(a => a.Id == (int)turma.TipoTurma))
                {
                    var codigoTurma = codigosTurmasRelacionadas.Where(x => x.CodigoAluno.ToString() == aluno).Select(s => s.CodigoTurma).FirstOrDefault().ToString();
                    turmaAluno = await mediator.Send(new ObterTurmaPorCodigoQuery(codigoTurma));
                }
                var ultimoBimestre = await ObterPeriodoUltimoBimestre(turmaAluno);
                var tipoCalendario = await tipoCalendarioRepository.ObterPorTurma(turma);
                var periodoFechamentoBimestre = await periodoFechamentoRepository.TurmaEmPeriodoDeFechamentoVigente(turmaAluno, tipoCalendario,DateTimeExtension.HorarioBrasilia().Date, ultimoBimestre.Bimestre);
                var tipoNota = await ObterTipoNota(turmaAluno, periodoFechamentoBimestre);
                var tipoNotaEhConceito = tipoNota.Equals(TipoNota.Conceito.Name());
                retorno.Add(new AlunoNotaTipoNotaDtoEducacaoFisicaDto("", tipoNotaEhConceito));
            }
        }

        private double? MontarNota(double? notaComponenteNotaConceito, bool turmaTipoNotaConceito, long? componenteCurricularCodigo)
        {
            if (turmaTipoNotaConceito && COMPONENTECURRICULARCODIGOEDFISICA.Equals(componenteCurricularCodigo))
            {
                if (notaComponenteNotaConceito < NOTA_CONCEITO_CINCO)
                    return (double)ConceitoValores.NS;
                else if (notaComponenteNotaConceito >= NOTA_CONCEITO_CINCO && notaComponenteNotaConceito  < NOTA_CONCEITO_SETE)
                    return (double)ConceitoValores.S;
                else if (notaComponenteNotaConceito  >= NOTA_CONCEITO_SETE)
                    return (double)ConceitoValores.P;
                else return notaComponenteNotaConceito;
            }
            else
                return notaComponenteNotaConceito;
        }

        private async Task<string> ObterTipoNota(Turma turma, PeriodoFechamentoVigenteDto periodoFechamentoBimestre)
        {
            var dataReferencia = periodoFechamentoBimestre != null ?
                periodoFechamentoBimestre.PeriodoFechamentoFim :
                (await ObterPeriodoUltimoBimestre(turma)).PeriodoFim;

            var tipoNota = await ObterNotaTipo(turma, dataReferencia);
            if (tipoNota == null)
                throw new NegocioException("Não foi possível identificar o tipo de nota da turma");

            return tipoNota;
        }

        private async Task<string> ObterNotaTipo(Turma turma, DateTime dataReferencia)
        {
            var cicloId = await cicloRepository.ObterCicloIdPorAnoModalidade(turma.Ano, turma.ModalidadeCodigo);

            if (cicloId == null)
                throw new NegocioException("Não foi encontrado o ciclo da turma informada");

            return await notaTipoRepository.ObterPorCicloIdDataAvalicacao(cicloId, dataReferencia);
        }

        private async Task<PeriodoEscolar> ObterPeriodoUltimoBimestre(Turma turma)
        {
            var periodoEscolarUltimoBimestre = await periodoEscolarRepository.ObterUltimoPeriodoAsync(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre);
            if (periodoEscolarUltimoBimestre == null)
                throw new NegocioException("Não foi possível localizar o período escolar do ultimo bimestre da turma");

            return periodoEscolarUltimoBimestre;
        }
    }
}
