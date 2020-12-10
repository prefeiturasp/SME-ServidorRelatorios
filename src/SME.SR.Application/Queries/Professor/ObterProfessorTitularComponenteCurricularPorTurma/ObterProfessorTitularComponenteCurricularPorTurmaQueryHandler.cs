﻿using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterProfessorTitularComponenteCurricularPorTurmaQueryHandler : IRequestHandler<ObterProfessorTitularComponenteCurricularPorTurmaQuery, IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        private readonly IProfessorRepository professorRepository;

        public ObterProfessorTitularComponenteCurricularPorTurmaQueryHandler(IProfessorRepository professorRepository)
        {
            this.professorRepository = professorRepository ?? throw new ArgumentNullException(nameof(professorRepository));
        }

        public async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> Handle(ObterProfessorTitularComponenteCurricularPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var lstProfessores = await professorRepository.BuscarProfessorTitularComponenteCurricularPorTurma(request.CodigosTurma);

            return lstProfessores;
        }
    }
}
