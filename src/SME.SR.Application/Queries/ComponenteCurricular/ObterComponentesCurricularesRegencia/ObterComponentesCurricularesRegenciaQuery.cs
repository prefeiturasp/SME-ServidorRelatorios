using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application.Queries.ComponenteCurricular.ObterComponentesCurricularesRegencia
{
    public class ObterComponentesCurricularesRegenciaQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public Turma Turma { get; set; }

        public long CdComponenteCurricular { get; set; }

        public bool ProfessorCJ { get; set; }

        public string UsuarioRf { get; set; }

        public Guid PerfilAtual { get; set; }
    }
}
