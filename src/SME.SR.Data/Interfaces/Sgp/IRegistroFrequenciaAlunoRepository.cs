using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRegistroFrequenciaAlunoRepository
    {
        Task<IEnumerable<RegistroFrequenciaAlunoDto>> ObterRegistrosFrequenciasAluno(string[] codigosAlunos, string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId, int[] bimestres);
        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunoPorAnoModalidadeSemestre(string alunoCodigo, int anoTurma, long tipoCalendarioId);
    }
}