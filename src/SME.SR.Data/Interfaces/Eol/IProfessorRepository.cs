using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IProfessorRepository
    {
        Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> BuscarProfessorTitularComponenteCurricularPorTurma(string[] codigosTurma);

        Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> BuscarProfessorTitularComponenteCurricularPorCodigosRf(string[] codigosRf);

        Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> BuscarProfessorTitularExternoComponenteCurricularPorTurma(string[] codigosTurma);
    }
}
