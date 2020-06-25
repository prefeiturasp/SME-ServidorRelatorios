using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ITurmaRepository
    {
        Task<DreUe> ObterDreUe(string codigoTurma);

        Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma);

        Task<IEnumerable<Turma>> ObterPorFiltros(string codigoUe, Modalidade? modalidade, int? anoLetivo, int? semestre);

        Task<Turma> ObterPorCodigo(string codigoTurma);
        Task<IEnumerable<Turma>> ObterPorAbrangenciaFiltros(string codigoUe, Modalidade? modalidade, int? anoLetivo, string login, Guid perfil, bool consideraHistorico, int? semestre);
    }
}
