using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseNotaRepository
    {
        Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAluno(long conselhoClasseId, string codigoAluno);
        Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisAlunoBimestre(string codigoTurma, string codigoAluno);
        Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisPorTurma(string turmaCodigo);
        Task<IEnumerable<RetornoNotaConceitoBimestreComponenteDto>> ObterNotasFinaisPorTurmasBimestresComponentes(string[] dresCodigos, string[] uesCodigos, long[] turmasId, int? semestre, int modalidade, string[] anos, int anoLetivo, int[] bimestres, long[] componentesCurricularesCodigos);
    }
}
