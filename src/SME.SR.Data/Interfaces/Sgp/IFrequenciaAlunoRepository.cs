﻿using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IFrequenciaAlunoRepository
    {
        Task<double> ObterFrequenciaGlobal(string codigoTurma, string codigoAluno);

        Task<FrequenciaAluno> ObterPorAlunoDataDisciplina(string codigoAluno, DateTime dataAtual, TipoFrequenciaAluno tipoFrequencia, string disciplinaId, string codigoTurma);

        Task<FrequenciaAluno> ObterPorAlunoBimestreAsync(string codigoAluno, int bimestre, TipoFrequenciaAluno tipoFrequencia, string disciplinaId, string codigoTurma);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorDisciplinaBimestres(string codigoTurma, string codigoAluno, int? bimestre);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaPorComponentesBimestresTurmas(string[] componentesCurriculares, int[] bimestres, string[] turmasCodigos);
        Task<IEnumerable<string>> ObterAlunosComRegistroFrequenciaPorTurmaBimestre(string turmaCodigo, int bimestre);
        Task<DateTime?> ObterUltimaFrequenciaRegistradaProfessor(string professorRf);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunos(string[] codigosAluno, int anoLetivo, int modalidade, int semestre);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciasPorTurmasAlunosParaHistoricoEscolar(string[] codigosAluno, int anoLetivo, int modalidade, int semestre);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGlobalAlunos(string[] codigosAluno, int anoLetivo, int modalidade);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaDisciplinaGlobalPorTurma(string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunoPorAnoModalidadeSemestre(int anoTurma, long tipoCalendarioId);
        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralPorAnoModalidadeSemestreEAlunos(int anoTurma, long tipoCalendarioId, string[] alunosCodigo);

        Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaGeralAlunosPorTurmaEBimestre(long turmaId, string alunoCodigo, int[] bimestres);
        Task<bool> ExisteFrequenciaRegistradaPorTurmaComponenteCurricular(string codigoTurma, string componenteCurricularId, long periodoEscolarId, int[]bimestres);
        Task<bool> ExisteFrequenciaRegistradaPorTurmaComponenteCurricularEAno(string codigoTurma, string componenteCurricularId, int anoLetivo);
        Task<IEnumerable<FrequenciaAlunoRetornoDto>> ObterFrequenciasAlunosPorTurmas(string[] codigosturma);
        Task<IEnumerable<FrequenciaAlunoRetornoDto>> ObterFrequenciasAlunosPorFiltro(string[] codigosturma, string componenteCurricularId, int bimestre);
    }
}