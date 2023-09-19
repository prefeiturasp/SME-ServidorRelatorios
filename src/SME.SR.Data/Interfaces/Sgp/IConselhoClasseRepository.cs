using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseRepository
    {
        Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId);
        Task<IEnumerable<long>> ObterPareceresConclusivosPorTipoAprovacao(bool aprovado);
        Task<IEnumerable<int>> ObterBimestresPorAlunoCodigo(string codigoAluno, int anoLetivo, Modalidade modalidade, int semestre, string codigoTurma);
        Task<IEnumerable<TotalAulasTurmaDisciplinaDto>> ObterTotalAulasSemFrequenciaPorTurmaBismetre(string[] discplinaId, string[] codigoTurma, int[] bimestre);
    }
}
