using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRegistrosFrequenciasAlunoQueryHandler : IRequestHandler<ObterRegistrosFrequenciasAlunoQuery, IEnumerable<RegistroFrequenciaAlunoDto>>
    {
        private readonly IRegistroFrequenciaAlunoRepository repositorio;

        public ObterRegistrosFrequenciasAlunoQueryHandler(IRegistroFrequenciaAlunoRepository repositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public async Task<IEnumerable<RegistroFrequenciaAlunoDto>> Handle(ObterRegistrosFrequenciasAlunoQuery request, CancellationToken cancellationToken)
            => await repositorio.ObterRegistrosFrequenciasAluno(request.CodigosAlunos, request.TurmasCodigo, request.ComponentesCurricularesId, request.TipoCalendarioId, request.Bimestres);

        
    }
}
