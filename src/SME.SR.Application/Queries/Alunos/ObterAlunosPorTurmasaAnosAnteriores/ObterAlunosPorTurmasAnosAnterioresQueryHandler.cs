using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmasAnosAnterioresQueryHandler : IRequestHandler<ObterAlunosPorTurmasAnosAnterioresQuery, IEnumerable<AlunoDaTurmaDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosPorTurmasAnosAnterioresQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunoDaTurmaDto>> Handle(ObterAlunosPorTurmasAnosAnterioresQuery request, CancellationToken cancellationToken)
        {
            var alunos = await turmaRepository.ObterAlunosPorTurmasAnosAnteriores(request.TurmasId);

            return alunos.GroupBy(a => a.CodigoAluno).SelectMany(x => x.OrderBy(y => y.CodigoAluno).Take(1));
        }
    }

    class ComparadorAlunos : IEqualityComparer<AlunoDaTurmaDto>
    {
        public bool Equals([AllowNull] AlunoDaTurmaDto x, [AllowNull] AlunoDaTurmaDto y)
        {
            return x.CodigoAluno == y.CodigoAluno;
        }

        public int GetHashCode([DisallowNull] AlunoDaTurmaDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
