using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IFrequenciaAlunoRepository
    {
        Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno);

        Task<FrequenciaAluno> ObterPorAlunoDataDisciplina(string codigoAluno, DateTime dataAtual, TipoFrequenciaAluno tipoFrequencia, string disciplinaId);

        Task<FrequenciaAluno> ObterPorAlunoBimestreAsync(string codigoAluno, int bimestre, TipoFrequenciaAluno tipoFrequencia, string disciplinaId);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorDisciplinaBimestres(string codigoTurma, string codigoAluno, int? bimestre);
    }
}
