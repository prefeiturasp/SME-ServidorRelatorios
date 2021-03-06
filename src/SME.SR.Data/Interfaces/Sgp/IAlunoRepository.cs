﻿using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> ObterDados(string codigoTurma, string codigoAluno);

        Task<IEnumerable<Aluno>> ObterPorCodigosTurma(string[] codigosTurma);

        Task<IEnumerable<Aluno>> ObterPorCodigosAlunoETurma(string[] codigosTurma, string[] codigosAluno);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigos(long[] codigosAlunos);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigosEAnoLetivo(long[] codigosAlunos, long anoLetivo);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosHistoricoAlunosPorCodigos(long[] codigosAlunos);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunoHistoricoEscolar(long[] codigosAlunos);
        Task<IEnumerable<AlunoNomeDto>> ObterNomesAlunosPorCodigos(string[] codigos);
        Task<IEnumerable<Aluno>> ObterAlunosPorTurmaDataSituacaoMaricula(long turmaCodigo, DateTime dataReferencia);
        Task<int> ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(string anoTurma, string ueCodigo, int anoLetivo, long dreCodigo, DateTime dataReferencia);
        Task<IEnumerable<AlunoResponsavelAdesaoAEDto>> ObterAlunosResponsaveisPorTurmasCodigoParaRelatorioAdesao(long[] turmasCodigo, int anoLetivo);
        Task<IEnumerable<AlunoReduzidoDto>> ObterAlunosReduzidosPorTurmaEAluno(long turmaCodigo, long? alunoCodigo);
        Task<IEnumerable<AlunoRetornoDto>> ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long turmaCodigo, long? alunoCodigo, int anoLetivo);
    }
}
