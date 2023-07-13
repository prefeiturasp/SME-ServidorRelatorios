using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasComplementaresEdFisicaQueryHandler : IRequestHandler<ObterTurmasComplementaresEdFisicaQuery, IEnumerable<Turma>>
    {
        private readonly IMediator mediator;

        public ObterTurmasComplementaresEdFisicaQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<Turma>> Handle(ObterTurmasComplementaresEdFisicaQuery request, CancellationToken cancellationToken)
        {
            var turmasAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigosAlunos.Select(ca => long.Parse(ca)).ToArray(), request.AnoLetivo));
            var turmasComplementaresEdFisica = new List<Turma>();

            turmasAlunos
                .Where(ta => ta.TipoTurma == TipoTurma.EdFisica && request.CodigosTurmas.Contains(ta.RegularCodigo))
                .ToList().ForEach(t =>
                {
                    if (!turmasComplementaresEdFisica.Any(tc => tc.Codigo.Equals(t.TurmaCodigo)))
                        turmasComplementaresEdFisica.Add(mediator.Send(new ObterTurmaPorCodigoQuery(t.TurmaCodigo)).Result);
                });

            return turmasComplementaresEdFisica;
        }
    }
}
