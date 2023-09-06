using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterInformacoesEscolaresAlunoQueryHandler : IRequestHandler<ObterInformacoesEscolaresAlunoQuery, InformacaoEscolarAlunoDto>
    {
        private readonly IMediator mediator;
        private readonly IAlunoRepository alunoRepository;

        public ObterInformacoesEscolaresAlunoQueryHandler(IMediator mediator, IAlunoRepository alunoRepository)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new System.ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<InformacaoEscolarAlunoDto> Handle(ObterInformacoesEscolaresAlunoQuery request, CancellationToken cancellationToken)
        {
            var informacoesEscolaresAluno = new InformacaoEscolarAlunoDto() { CodigoAluno = request.CodigoAluno }; 
            var necessidadesEspeciaisAluno = await alunoRepository.ObterNecessidadesEspeciaisPorAluno(long.Parse(request.CodigoAluno));

            if (necessidadesEspeciaisAluno != null)
            {
                informacoesEscolaresAluno.DescricaoNecessidadeEspecial = necessidadesEspeciaisAluno.DescricaoNecessidadeEspecial;
                informacoesEscolaresAluno.DescricaoRecurso = necessidadesEspeciaisAluno.DescricaoRecurso;
                informacoesEscolaresAluno.TipoRecurso = necessidadesEspeciaisAluno.TipoRecurso;
                informacoesEscolaresAluno.TipoNecessidadeEspecial = necessidadesEspeciaisAluno.TipoNecessidadeEspecial;
            }

            informacoesEscolaresAluno.FrequenciaGlobal = await PercentualFrequenciaGlobal(request.CodigoAluno, request.TurmaCodigo);

            return informacoesEscolaresAluno;
        }

        public async Task<string> PercentualFrequenciaGlobal(string codigoAluno, string codigoTurma)
        {
            var turma = await mediator.Send(new ObterTurmaPorCodigoQuery(codigoTurma));
            
            if (turma == null)
                throw new NegocioException($"Não foi possível encontrar a turma de código {codigoTurma}");

            var tipoCalendarioId = turma.ModalidadeCodigo == Modalidade.EJA ? await mediator.Send(new ObterTipoCalendarioIdPorTurmaQuery(turma)) : 0;

            var frequenciasAluno = await mediator.Send(new ObterFrequenciasGeralAlunoPorCodigoAnoSemestreQuery(codigoAluno, turma.AnoLetivo, tipoCalendarioId));

            var frequenciaBimestreAlunoDto = new List<FrequenciaBimestreAlunoDto>();

            foreach (var frequencia in frequenciasAluno)
            {
                var frequenciaBimestreAluno = new FrequenciaBimestreAlunoDto()
                {
                    Bimestre = frequencia.Bimestre,
                    CodigoAluno = frequencia.CodigoAluno,
                    Frequencia = frequencia.PercentualFrequencia,
                    QuantidadeAusencias = frequencia.TotalAusencias,
                    QuantidadeCompensacoes = frequencia.TotalCompensacoes
                };

                frequenciaBimestreAlunoDto.Add(frequenciaBimestreAluno);
            }

            var frequenciaAluno = new FrequenciaAluno()
            {
                TotalAulas = frequenciasAluno.Sum(f => f.TotalAulas),
                TotalAusencias = frequenciasAluno.Sum(f => f.TotalAusencias),
                TotalCompensacoes = frequenciasAluno.Sum(f => f.TotalCompensacoes),
            };

            return frequenciaAluno.PercentualFrequenciaFormatado;
        }
    }
}