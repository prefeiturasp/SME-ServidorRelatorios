using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IConselhoClasseConsolidadoRepository
    {
        Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasAsync(string[] turmasCodigo);
        Task<IEnumerable<ConselhoClasseConsolidadoTurmaDto>> ObterConselhosClasseConsolidadoPorTurmasTodasUesAsync(string dreCodigo, int modalidade, int[] bimestres, SituacaoConselhoClasse? situacao, int anoLetivo, int semestre, bool exibirHistorico);
        Task<IEnumerable<NotasAlunoBimestreBoletimSimplesDto>> ObterNotasBoletimPorAlunoTurma(string[] alunosCodigos, string[] turmasCodigos, int semestre, int anoAtual);
    }
}
