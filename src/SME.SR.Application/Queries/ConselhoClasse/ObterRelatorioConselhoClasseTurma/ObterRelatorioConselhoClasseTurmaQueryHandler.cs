using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.ConselhoClasse.ObterRelatorioConselhoClasseTurma
{
    public class ObterRelatorioConselhoClasseTurmaQueryHandler : IRequestHandler<ObterRelatorioConselhoClasseTurmaQuery, IEnumerable<RelatorioConselhoClasseBase>>
    {
        private readonly IMediator mediator;

        public ObterRelatorioConselhoClasseTurmaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<RelatorioConselhoClasseBase>> Handle(ObterRelatorioConselhoClasseTurmaQuery request, CancellationToken cancellationToken)
        {
            var turma = await mediator.Send(new ObterFechamentoTurmaPorIdQuery() { FechamentoTurmaId = request.FechamentoTurmaId });


            var alunos = await ObterAlunosTurma(turma.TurmaId);

            var lstRelatorioAlunos = new List<RelatorioConselhoClasseBase>();
            string codigoAluno;

            foreach (var aluno in alunos)
            {
                codigoAluno = aluno.CodigoAluno.ToString();

                lstRelatorioAlunos.Add(await ObterRelatorioConselhoClasseAluno(request.ConselhoClasseId, request.FechamentoTurmaId,
                                                                               codigoAluno));
            }

            return lstRelatorioAlunos;
        }

        private async Task<IEnumerable<Aluno>> ObterAlunosTurma(string codigoTurma)
        {
            return await mediator.Send(new ObterAlunosPorTurmaQuery()
            {
                TurmaCodigo = codigoTurma
            });
        }

        private async Task<RelatorioConselhoClasseBase> ObterRelatorioConselhoClasseAluno(long conselhoClasseId,
                                                                                                long fechamentoTurmaId,
                                                                                                string codigoAluno)
        {
         var retorno = await mediator.Send(new ObterRelatorioConselhoClasseAlunoQuery()
            {
                CodigoAluno = codigoAluno,
                ConselhoClasseId = conselhoClasseId,
                FechamentoTurmaId = fechamentoTurmaId
            });

            return retorno.Relatorio.FirstOrDefault();
        }
    }
}
