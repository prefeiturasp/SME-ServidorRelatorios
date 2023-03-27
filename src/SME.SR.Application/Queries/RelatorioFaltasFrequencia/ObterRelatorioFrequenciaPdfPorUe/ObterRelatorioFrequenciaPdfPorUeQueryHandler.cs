using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorUeQueryHandler : IRequestHandler<ObterRelatorioFrequenciaPdfPorUeQuery, RelatorioFrequenciaUeDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFrequenciaPdfPorUeQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioFrequenciaUeDto> Handle(ObterRelatorioFrequenciaPdfPorUeQuery request, CancellationToken cancellationToken)
        {
            var relatorioFrequenciaUeDto = new RelatorioFrequenciaUeDto();

            foreach (var turma in request.RelatorioFrequenciaTurmaAnoDto)
            {
                turma.EhExibirTurma = request.Filtro.TipoRelatorio == TipoRelatorioFaltasFrequencia.Ano;
                turma.Nome = turma.Nome.Equals("0º ano") ? "TURMA DE PROGRAMA" : turma.NomeTurmaAno.ToUpper();
                
                relatorioFrequenciaUeDto.TurmasAnos.Add(await mediator.Send(new ObterRelatorioFrequenciaPdfPorTurmaQuery(request.Filtro, turma.Bimestres,
                        request.PeriodosEscolares, request.Componentes, request.Alunos, request.DeveAdicionarFinal,
                        request.MostrarSomenteFinal)));
            }

            return relatorioFrequenciaUeDto;
        }
    }
}
