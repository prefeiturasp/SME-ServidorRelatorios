using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoQuery, RelatorioParecerConclusivoDto>
    {
        private readonly IParecerConclusivoRepository parecerConclusivoRepository;
        private readonly ITurmaRepository turmaRepository;

        public ObterRelatorioParecerConclusivoQueryHandler(IParecerConclusivoRepository parecerConclusivoRepository, ITurmaRepository turmaRepository)
        {
            this.parecerConclusivoRepository = parecerConclusivoRepository ?? throw new System.ArgumentNullException(nameof(parecerConclusivoRepository));
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }
        public async Task<RelatorioParecerConclusivoDto> Handle(ObterRelatorioParecerConclusivoQuery request, CancellationToken cancellationToken)
        {
            var retorno = new RelatorioParecerConclusivoDto();

            var parecesParaTratar = await parecerConclusivoRepository.ObterPareceresFinais(request.filtroRelatorioParecerConclusivoDto.AnoLetivo,
                request.filtroRelatorioParecerConclusivoDto.DreCodigo, request.filtroRelatorioParecerConclusivoDto.UeCodigo, request.filtroRelatorioParecerConclusivoDto.Modalidade,
                request.filtroRelatorioParecerConclusivoDto.Semestre, request.filtroRelatorioParecerConclusivoDto.CicloId, request.filtroRelatorioParecerConclusivoDto.Anos, 
                request.filtroRelatorioParecerConclusivoDto.ParecerConclusivoId);


            if (parecesParaTratar == null || !parecesParaTratar.Any())
                throw new NegocioException("Não foi possível localizar informações com o(s) filtro(s) informado(s).");

            var turmasCodigos = parecesParaTratar.Select(a => a.TurmaId).Distinct();
            
            var alunosDasTurmas = await turmaRepository.ObterAlunosPorTurmas(turmasCodigos);

            if (alunosDasTurmas == null || !alunosDasTurmas.Any())
                throw new NegocioException("Não foi possível localizar informações dos alunos.");




            return await Task.FromResult(retorno);
        }
    }
}
