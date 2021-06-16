﻿using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseAlunoRepository : IConselhoClasseAlunoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseAlunoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ParecerConclusivo;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<string>(query, parametros);
            }
        }


        public async Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterParecerConclusivoPorTurma(string turmaCodigo)
        {
            var query = ConselhoClasseAlunoConsultas.ParecerConclusivoPorTurma;

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<ConselhoClasseParecerConclusivo>(query, new { turmaCodigo });
            }
        }

        public async Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.Recomendacoes;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<RecomendacaoConselhoClasseAluno>(query, parametros);
            }
        }

        public async Task<IEnumerable<RecomendacaoConselhoClasseAluno>> ObterRecomendacoesPorAlunosTurmas(string[] codigosAluno, string[] codigosTurma, int anoLetivo, Modalidade? modalidade, int semestre)
        {
            var query = new StringBuilder(@"select distinct t.turma_id TurmaCodigo, cca.aluno_codigo AlunoCodigo,
                                 cca.recomendacoes_aluno RecomendacoesAluno, 
                                 cca.recomendacoes_familia RecomendacoesFamilia,
                                 cca.anotacoes_pedagogicas AnotacoesPedagogicas
                                from conselho_classe cc
                                inner join fechamento_turma ft on cc.fechamento_turma_id = ft.id 
                                inner join turma t on ft.turma_id = t.id 
                                inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id
                                inner join tipo_calendario tc on t.ano_letivo = tc.ano_letivo and tc.modalidade = @modalidadeTipoCalendario
                                inner join periodo_escolar p on p.tipo_calendario_id = tc.id
                                where 1 = 1");

            if (codigosAluno != null && codigosAluno.Length > 0)
                query.AppendLine(" and cca.aluno_codigo = ANY(@codigosAluno) ");

            if (codigosTurma != null && codigosTurma.Length > 0)
                query.AppendLine(" and t.turma_id = ANY(@codigosTurma) ");

            if (anoLetivo > 0)
                query.AppendLine(" and t.ano_letivo = @anoLetivo ");

            if (modalidade.HasValue)
                query.AppendLine(" and t.modalidade_codigo = @modalidade ");

            DateTime dataReferencia = DateTime.MinValue;
            if (modalidade == Modalidade.EJA)
            {
                var periodoReferencia = semestre == 1 ? "periodo_inicio < @dataReferencia" : "periodo_fim > @dataReferencia";
                query.AppendLine($"and exists(select 0 from periodo_escolar p where tipo_calendario_id = tc.id and {periodoReferencia})");

                // 1/6/ano ou 1/7/ano dependendo do semestre
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 7, 1);
            }

            var parametros = new
            {
                codigosAluno,
                codigosTurma,
                anoLetivo,
                modalidade = (int)modalidade,
                modalidadeTipoCalendario = modalidade.HasValue ? (int)modalidade.Value.ObterModalidadeTipoCalendario() : (int)default,
                dataReferencia
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<RecomendacaoConselhoClasseAluno>(query.ToString(), parametros);
            }
        }

        public async Task<bool> PossuiConselhoClasseCadastrado(long conselhoClasseId, string codigoAluno)
        {
            var query = ConselhoClasseAlunoConsultas.ObterPorConselhoClasseId;
            var parametros = new { ConselhoClasseId = conselhoClasseId, CodigoAluno = codigoAluno };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<bool>(query, parametros);
            }
        }
    }
}
