using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioAtaBimestralQueryHandler : IRequestHandler<ObterNotasRelatorioAtaBimestralQuery, IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        private INotaConceitoRepository notasConceitoRepository;
        private IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;

        public ObterNotasRelatorioAtaBimestralQueryHandler(INotaConceitoRepository notasConceitoRepository, IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentException(nameof(conselhoClasseAlunoRepository));
        }

        public async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> Handle(ObterNotasRelatorioAtaBimestralQuery request, CancellationToken cancellationToken)
        {
            var notas = await notasConceitoRepository.ObterNotasTurmasAlunosParaAtaBimestralAsync(request.CodigosAlunos, request.AnoLetivo, request.Modalidade, request.Semestre, request.TiposTurma, request.Bimestre);

            if (notas == null || !notas.Any())
                throw new NegocioException("Não foi possível obter as notas dos alunos");

            await CarregarConselhoDeClasseAlunoId(notas);

            return notas.GroupBy(nf => nf.CodigoTurma);
        }

        private async Task CarregarConselhoDeClasseAlunoId(IEnumerable<NotasAlunoBimestre> notasAluno)
        {
            var turmaIds = notasAluno.Select(nota => nota.IdTurma).Distinct().ToArray();
            var alunosCodigos = notasAluno.Select(nota => nota.CodigoAluno).Distinct().ToArray();
            var conselhos = await conselhoClasseAlunoRepository.ObterConselhoDeClasseAlunoId(turmaIds, alunosCodigos);

            if (conselhos.Any())
            {
                foreach (var nota in notasAluno)
                {
                    nota.ConselhoClasseAlunoId = conselhos.FirstOrDefault(conselho => conselho.TurmaId == nota.IdTurma &&
                                                                                      conselho.AlunoCodigo == nota.CodigoAluno)?.ConselhoClasseAlunoId ?? 0;
                }
            }
        }
    }
}
