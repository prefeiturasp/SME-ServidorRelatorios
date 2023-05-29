using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class VerificarAulasNormaisCriadasProfessorRegenciaQueryHandler : IRequestHandler<VerificarAulasNormaisCriadasProfessorRegenciaQuery, bool>
    {
        private readonly IAulaRepository aulaRepository;
        public VerificarAulasNormaisCriadasProfessorRegenciaQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public Task<bool> Handle(VerificarAulasNormaisCriadasProfessorRegenciaQuery request, CancellationToken cancellationToken)
                => aulaRepository.VerificaExisteAulaCadastradaProfessorRegencia(request.Turmaid, request.ComponenteCurricularId, request.Bimestre, request.TipoCalendarioId);
    }
}
