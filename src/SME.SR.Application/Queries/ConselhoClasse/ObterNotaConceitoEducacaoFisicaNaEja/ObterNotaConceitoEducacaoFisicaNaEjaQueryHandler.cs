using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotaConceitoEducacaoFisicaNaEjaQueryHandler : IRequestHandler<ObterNotaConceitoEducacaoFisicaNaEjaQuery, IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto>>
    {
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private readonly IPeriodoFechamentoRepository periodoFechamentoRepository;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;
        private readonly ICicloRepository cicloRepository; 
        private readonly INotaTipoRepository notaTipoRepository;


        public ObterNotaConceitoEducacaoFisicaNaEjaQueryHandler(IPeriodoEscolarRepository periodoEscolarRepository,
            IPeriodoFechamentoRepository periodoFechamentoRepository, ITipoCalendarioRepository tipoCalendarioRepository, ICicloRepository cicloRepository,
            INotaTipoRepository notaTipoRepository)
        {
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.periodoFechamentoRepository = periodoFechamentoRepository ?? throw new ArgumentNullException(nameof(periodoFechamentoRepository));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository));
            this.notaTipoRepository = notaTipoRepository ?? throw new ArgumentNullException(nameof(notaTipoRepository));
        }

        public async Task<IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto>> Handle(ObterNotaConceitoEducacaoFisicaNaEjaQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoNotaTipoNotaDtoEducacaoFisicaDto>();

            await MapearRetorno(retorno, request.Turma,request.AlunosCodigos);

            return retorno;
        }

        private async Task MapearRetorno(List<AlunoNotaTipoNotaDtoEducacaoFisicaDto> retorno, Turma turmaAluno, string[] alunosCodigos)
        {
           
            foreach (var aluno in alunosCodigos)
            {
                var ultimoBimestre = await ObterPeriodoUltimoBimestre(turmaAluno);
                var tipoCalendario = await tipoCalendarioRepository.ObterPorTurma(turmaAluno);
                var periodoFechamentoBimestre = await periodoFechamentoRepository.TurmaEmPeriodoDeFechamentoVigente(turmaAluno, tipoCalendario,DateTimeExtension.HorarioBrasilia().Date, ultimoBimestre.Bimestre);
                var tipoNota = await ObterTipoNota(turmaAluno, periodoFechamentoBimestre);
                var tipoNotaEhConceito = tipoNota.Equals(TipoNota.Conceito.Name());
                retorno.Add(new AlunoNotaTipoNotaDtoEducacaoFisicaDto(tipoNotaEhConceito,aluno));
            }
        }

        private async Task<string> ObterTipoNota(Turma turma, PeriodoFechamentoVigenteDto periodoFechamentoBimestre)
        {
            var dataReferencia = periodoFechamentoBimestre != null ?
                periodoFechamentoBimestre.PeriodoFechamentoFim :
                (await ObterPeriodoUltimoBimestre(turma)).PeriodoFim;

            return await ObterNotaTipo(turma, dataReferencia);

        }

        private async Task<string> ObterNotaTipo(Turma turma, DateTime dataReferencia)
        {
            var cicloId = await cicloRepository.ObterCicloIdPorAnoModalidade(turma.Ano, turma.ModalidadeCodigo);

            if (cicloId == null)
                return string.Empty;

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
