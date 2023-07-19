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

            var alunos = await ObterAlunosTurma(turma.Turma.Codigo);

            var lstRelatorioAlunos = new List<RelatorioConselhoClasseBase>();
            string codigoAluno;

            foreach (var aluno in alunos)
            {
                codigoAluno = aluno.CodigoAluno.ToString();

                var alunoPossuiConselho = await AlunoPossuiConselhoClasseCadastrado(request.ConselhoClasseId, codigoAluno);

                if (alunoPossuiConselho)
                {
                   var dadosAlunoConselho = await ObterRelatorioConselhoClasseAluno(request.ConselhoClasseId, request.FechamentoTurmaId,
                                                                                   codigoAluno, request.Usuario);
                   if(dadosAlunoConselho != null)
                        lstRelatorioAlunos.Add(dadosAlunoConselho);
                }
                    
            }

            return lstRelatorioAlunos;
        }

        private async Task<bool> AlunoPossuiConselhoClasseCadastrado(long conselhoClasseId,
                                                                                   string codigoAluno)
        {
            return await mediator.Send(new AlunoPossuiConselhoClasseCadastradoQuery()
            {
                ConselhoClasseId = conselhoClasseId,
                CodigoAluno = codigoAluno
            });
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
                                                                                                string codigoAluno,
                                                                                                Usuario usuario)
        {
            var retorno = await mediator.Send(new ObterRelatorioConselhoClasseAlunoQuery()
            {
                CodigoAluno = codigoAluno,
                ConselhoClasseId = conselhoClasseId,
                FechamentoTurmaId = fechamentoTurmaId,
                Usuario = usuario
            });

            return retorno.Relatorio?.FirstOrDefault() ?? null;
        }
    }
}
