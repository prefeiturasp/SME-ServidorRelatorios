using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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

            var frequenciaGlobalAluno = await mediator.Send(new ObterFrequenciaGlobalPorAlunoQuery() { CodigoAluno = request.CodigoAluno, CodigoTurma = request.TurmaCodigo });
            informacoesEscolaresAluno.FrequenciaGlobal = frequenciaGlobalAluno;

            return informacoesEscolaresAluno;
        }
    }
}