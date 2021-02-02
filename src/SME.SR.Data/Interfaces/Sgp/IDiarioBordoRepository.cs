using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IDiarioBordoRepository
    {
        Task<DateTime?> ObterUltimoDiarioBordoProfessor(string professorRf);
        Task<IEnumerable<AulaDiarioBordoDto>> ObterAulasDiarioBordo(long anoLetivo, int bimestre, string codigoUe, long componenteCurricular, bool listarDataFutura, string codigoTurma, Modalidade modalidadeTurma, ModalidadeTipoCalendario modalidadeCalendario, int semestre);
    }
}
