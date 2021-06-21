using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorAlunosQuery : IRequest<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>>
    {
        public ObterComponentesCurricularesPorAlunosQuery(int[] alunosCodigos, int anoLetivo, int semestre, string codigoUe, Modalidade modalidade, Usuario usuario)
        {
            AlunosCodigos = alunosCodigos;
            AnoLetivo = anoLetivo;
            Semestre = semestre;
            CodigoUe = codigoUe;
            Modalidade = modalidade;
            Usuario = usuario;
        }

        public int[] AlunosCodigos { get; }
        public int AnoLetivo { get; }
        public int Semestre { get; }
        public string CodigoUe { get; }
        public Modalidade Modalidade { get; }
        public Usuario Usuario { get; }
    }
}
