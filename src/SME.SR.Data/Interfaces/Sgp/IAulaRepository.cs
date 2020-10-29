﻿using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre);
        Task<AulaPrevista> ObterAulaPrevistaFiltro(long tipoCalendarioId, string turmaId, string disciplinaId);        
        Task<bool> VerificaExisteAulaCadastradaProfessorRegencia(string componenteCurricularId, int bimestre, long tipoCalendarioId);
        Task<bool> VerificaExisteAulaCadastrada(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId);
        Task<bool> VerificaExisteMaisAulaCadastradaNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre);
        Task<IEnumerable<DateTime>> ObterDiasAulaCriadasPeriodoInicioEFim(long turmaId, long componenteCurricularId, DateTime dataInicio, DateTime dataFim);
        Task<int> ObterQuantidadeAulaGrade(long turmaId, long componenteCurricularCodigo);
        Task<IEnumerable<AulaReduzidaDto>> ObterQuantidadeAulasReduzido(long turmaId, string v, long tipoCalendarioId, int bimestre, bool professorCJ);
        Task<bool> VerificaExsiteAulaTitularECj(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre);
        Task<bool> VerificaExisteAulasMesmoDiaProfessor(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre);

        Task<IEnumerable<AulaDuplicadaControleGradeDto>> DetalharAulasDuplicadasNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre);
        Task<IEnumerable<AulaNormalExcedidoControleGradeDto>> ObterAulasExcedidas(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre);
    }
}
