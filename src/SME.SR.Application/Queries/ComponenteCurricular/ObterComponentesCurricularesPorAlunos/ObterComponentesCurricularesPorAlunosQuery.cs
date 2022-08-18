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
        public ObterComponentesCurricularesPorAlunosQuery(int[] codigosTurmas, int[] alunosCodigos, int anoLetivo, int semestre, string codigoUe, Modalidade modalidade, Usuario usuario, bool consideraHistorico = false)
        {
            CodigosTurmas = codigosTurmas;
            AlunosCodigos = alunosCodigos;
            AnoLetivo = anoLetivo;
            Semestre = semestre;
            CodigoUe = codigoUe;
            Modalidade = modalidade;
            Usuario = usuario;
            ConsideraHistorico = consideraHistorico;
        }

        public int[] CodigosTurmas { get; set; }
        public int[] AlunosCodigos { get; }
        public int AnoLetivo { get; }
        public int Semestre { get; }
        public string CodigoUe { get; }
        public Modalidade Modalidade { get; }
        public Usuario Usuario { get; }
        public bool ConsideraHistorico { get; }
    }
}
