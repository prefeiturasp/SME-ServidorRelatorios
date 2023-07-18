
using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PerguntasAutoralRepository : IPerguntasAutoralRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PerguntasAutoralRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PerguntasAutoralDto>> ObterPerguntasPorComponenteAnoTurma(int anoTurma, int anoLetivo, int bimestre, ComponenteCurricularSondagemEnum? componenteCurricularSondagem)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select \"AnoEscolar\", p.\"Id\" PerguntaId, p.\"Descricao\" Pergunta, r.\"Id\" RespostaId,  r.\"Descricao\" Resposta ");
            query.Append(" from \"Pergunta\" p ");
            query.Append(" inner join \"PerguntaAnoEscolar\" pae on pae.\"PerguntaId\" = p.\"Id\" ");
            query.Append(" inner join \"PerguntaResposta\" pr on pr.\"PerguntaId\" = p.\"Id\" ");
            query.Append(" left join \"PerguntaAnoEscolarBimestre\" paeb ON paeb.\"PerguntaAnoEscolarId\" = pae.\"Id\"");
            query.Append(" inner join \"Resposta\" r on pr.\"RespostaId\" = r.\"Id\"  where 1=1 ");
            
            if (anoTurma > 0)
                query.Append("and \"AnoEscolar\" = @anoTurma ");

            if (anoTurma <= 3)
                query.Append("and pae.\"Grupo\" = @grupoProficiencia ");

            if (componenteCurricularSondagem != null)
                query.Append("and p.\"ComponenteCurricularId\" = @componenteCurricularId ");

            query.Append("and ((EXTRACT(YEAR FROM pae.\"InicioVigencia\") <= @anoLetivo and pae.\"FimVigencia\" is null)");

            query.Append("or @anoLetivo <= (case when pae.\"FimVigencia\" is null then 0 else EXTRACT(YEAR FROM pae.\"FimVigencia\") end)) ");

            query.Append(" and (paeb.\"Id\" is null");
            query.Append(" and not exists(select 1 from \"PerguntaAnoEscolar\" pae");
            query.Append("                inner join \"PerguntaAnoEscolarBimestre\" paeb ON paeb.\"PerguntaAnoEscolarId\" = pae.\"Id\"");
            query.Append("                where pae.\"AnoEscolar\" = @anoTurma");
            query.Append("                  and (pae.\"FimVigencia\" is null and extract(year from pae.\"InicioVigencia\") <= @anoLetivo)");
            query.Append("                  and paeb.\"Bimestre\" = @bimestre)");
            query.Append(" or paeb.\"Bimestre\" = @bimestre)");

            query.Append("order by pae.\"Ordenacao\", pr.\"Ordenacao\"");

            var parametros = new { anoTurma, anoLetivo = anoLetivo, bimestre, componenteCurricularId = componenteCurricularSondagem.Name(), grupoProficiencia = ProficienciaSondagemEnum.Numeros};

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<PerguntasAutoralDto>(query.ToString(), parametros);
        }

        public async Task<IEnumerable<PerguntasOrdemGrupoAutoralDto>> ObterPerguntasPorGrupo(GrupoSondagemEnum grupoSondagem, ComponenteCurricularSondagemEnum componenteCurricular)
        {
            StringBuilder query = new StringBuilder();

            query.Append(" select p.\"Id\" PerguntaId, p.\"Descricao\" Pergunta, o.\"Id\" OrdemId,  o.\"Descricao\" Ordem, ");
            query.Append(" g.\"Id\" GrupoId, g.\"Descricao\" Grupo ");
            query.Append(" from \"Grupo\" g ");
            query.Append(" inner join \"GrupoOrdem\" go on go.\"GrupoId\" = g.\"Id\" ");
            query.Append(" inner join \"Ordem\" o on o.\"Id\" = go.\"OrdemId\" ");
            query.Append(" inner join \"OrdemPergunta\" op on op.\"GrupoId\" = g.\"Id\"  ");
            query.Append(" inner join \"Pergunta\" p on p.\"Id\" = op.\"PerguntaId\" where 1=1 ");

            if (grupoSondagem > 0)
                query.Append("and g.\"Id\" = @grupoId ");

            if (componenteCurricular > 0)
                query.Append("and p.\"ComponenteCurricularId\" = @componenteCurricularId ");

            query.Append("order by go.\"Ordenacao\", op.\"OrdenacaoNaTela\" ");

            var parametros = new { grupoId = grupoSondagem.Name(), componenteCurricularId = componenteCurricular.Name() };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem);
            return await conexao.QueryAsync<PerguntasOrdemGrupoAutoralDto>(query.ToString(), parametros);
        }
    }
}
