using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFechamentosPorCodigosTurmaQueryHandler : IRequestHandler<ObterFechamentosPorCodigosTurmaQuery, IEnumerable<FechamentoTurma>>
    {
        private IFechamentoTurmaRepository fechamentoTurmaRepository;

        public ObterFechamentosPorCodigosTurmaQueryHandler(IFechamentoTurmaRepository fechamentoTurmaRepository)
        {
            this.fechamentoTurmaRepository = fechamentoTurmaRepository;
        }

        public async Task<IEnumerable<FechamentoTurma>> Handle(ObterFechamentosPorCodigosTurmaQuery request, CancellationToken cancellationToken)
        {
            var fechamentos = await fechamentoTurmaRepository.ObterFechamentosPorCodigosTurma(request.CodigosTurma);

            if (fechamentos == null || !fechamentos.Any())
                throw new NegocioException("Não foi possível obter os fechamentos das turmas");

            return fechamentos;
        }
    }
}
