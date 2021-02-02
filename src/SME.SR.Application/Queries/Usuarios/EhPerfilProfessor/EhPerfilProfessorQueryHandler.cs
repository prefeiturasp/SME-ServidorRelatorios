using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class EhPerfilProfessorQueryHandler : IRequestHandler<EhPerfilProfessorQuery, bool>
    {
        public Task<bool> Handle(EhPerfilProfessorQuery request, CancellationToken cancellationToken)
            => Task.FromResult(new[] {
                Perfis.PERFIL_PROFESSOR,
                Perfis.PERFIL_CJ,
                Perfis.PERFIL_PROFESSOR_INFANTIL,
                Perfis.PERFIL_CJ_INFANTIL,
                Perfis.PERFIL_POA,
                Perfis.PERFIL_PAEE,
                Perfis.PERFIL_PAP,
                Perfis.PERFIL_POEI,
                Perfis.PERFIL_POED,
                Perfis.PERFIL_POSL
            }.Contains(request.Perfil));
    }
}
