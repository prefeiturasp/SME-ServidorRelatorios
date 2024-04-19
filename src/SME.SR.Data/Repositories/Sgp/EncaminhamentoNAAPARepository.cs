using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class EncaminhamentoNAAPARepository : IEncaminhamentoNAAPARepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private const int SECAO_ETAPA_1 = 1;
        private const int SECAO_INFORMACOES_ALUNO_ORDEM = 1;
        private const int SECAO_ITINERANCIA_ORDEM = 3;
        private const string NOME_COMPONENTE_DATA_ENTRADA_QUEIXA = "DATA_ENTRADA_QUEIXA";
        private const string NOME_COMPONENTE_PORTA_ENTRADA = "PORTA_ENTRADA";
        private const string NOME_COMPONENTE_FLUXO_ALERTA = "FLUXO_ALERTA";
        private const string NOME_COMPONENTE_DATA_DO_ATENDIMENTO = "DATA_DO_ATENDIMENTO";

        public EncaminhamentoNAAPARepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }       

        private string ObterCondicaoDre(FiltroRelatorioEncaminhamentoNAAPADto filtro) =>
                    !filtro.DreCodigo.EstaFiltrandoTodas() ? " and d.dre_id = @dreCodigo " : string.Empty;

        private string ObterCondicaoUe(FiltroRelatorioEncaminhamentoNAAPADto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;

        private string ObterCondicaoIds(long[] ids) =>
            ids.Any() ? " and en.id = ANY(@ids) " : string.Empty;

        private string ObterCondicaoSituacao(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var condicao = string.Empty;

            if (!filtro.ExibirEncerrados)
                condicao += $" and en.situacao <> {(int)SituacaoNAAPA.Encerrado}";

            if (filtro.SituacaoIds != null && !filtro.SituacaoIds.EstaFiltrandoTodas())
                condicao += " and en.situacao = ANY(@situacaoIds) ";

            return condicao;
        }

        private string ObterCondicaoPortaEntrada(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var condicao = string.Empty;

            if (filtro.PortaEntradaIds != null && filtro.PortaEntradaIds.Any())
                condicao += $" and qportaentrada.PortaEntradaId = ANY(@portaEntradaIds)";

            return condicao;
        }

        private string ObterCondicaoFluxoAlerta(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var condicao = string.Empty;

            if (filtro.FluxoAlertaIds != null && filtro.FluxoAlertaIds.Any())
                condicao += $" and qfluxoalerta.FluxoAlertaId = ANY(@fluxoAlertaIds)";

            return condicao;
        }

        private string ObterCondicaoModalidades(FiltroRelatorioEncaminhamentoNAAPADto filtro) =>
            filtro.Modalidades != null && filtro.Modalidades.Any() ? " and t.modalidade_codigo = ANY(@modalidades)" : string.Empty;

        private string ObterCondicaoAnosEscolaresCodigos(FiltroRelatorioEncaminhamentoNAAPADto filtro) =>
             filtro.AnosEscolaresCodigos != null && filtro.AnosEscolaresCodigos.Any() && !filtro.AnosEscolaresCodigos.EstaFiltrandoTodas() ? " and t.ano = ANY(@anosEscolaresCodigos)" : string.Empty;

        private string ObterCondicao(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioEncaminhamentoNAAPADto, string>>
            {
                ObterCondicaoModalidades,
                ObterCondicaoAnosEscolaresCodigos,
                ObterCondicaoDre,
                ObterCondicaoUe,
                ObterCondicaoSituacao,
                ObterCondicaoPortaEntrada,
                ObterCondicaoFluxoAlerta
            };

            foreach (var funcao in funcoes)
                query.Append(funcao(filtro));

            return query.ToString();
        }

        public async Task<IEnumerable<EncaminhamentoNAAPASimplesDto>> ObterResumoEncaminhamentosNAAPA(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var query = new StringBuilder();

            query.AppendLine($@"     
                    with vw_resposta_data as (
                        select ens.encaminhamento_naapa_id, 
                               case when length(enr.texto) > 0 then to_date(enr.texto,'yyyy-mm-dd') else null end DataAberturaQueixa	
                        from encaminhamento_naapa_secao ens   
                        join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                        join questao q on enq.questao_id = q.id 
                        join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                        join secao_encaminhamento_naapa secao on secao.id = ens.secao_encaminhamento_id                        
                        left join opcao_resposta opr on opr.id = enr.resposta_id
                        where q.nome_componente = @nomeComponenteDataEntradaQueixa and secao.etapa = @secaoEtapa1 and secao.ordem = @secaoInformacoesAlunoOrdem
                        ),
                        vw_resposta_porta_entrada as (
                        select ens.encaminhamento_naapa_id, 
                                opr.nome as PortaEntrada,
                                enr.resposta_id  as PortaEntradaId
                        from encaminhamento_naapa_secao ens   
                        join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                        join questao q on enq.questao_id = q.id 
                        join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                        join secao_encaminhamento_naapa secao on secao.id = ens.secao_encaminhamento_id                        
                        left join opcao_resposta opr on opr.id = enr.resposta_id
                         where q.nome_componente = @nomeComponentePortaEntrada and secao.etapa = @secaoEtapa1 and secao.ordem = @secaoInformacoesAlunoOrdem
                        ),
                        vw_resposta_fluxo_alerta as (
                        select ens.encaminhamento_naapa_id, 
                                opr.nome as FluxoAlerta,
                                enr.resposta_id  as FluxoAlertaId
                        from encaminhamento_naapa_secao ens   
                        join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                        join questao q on enq.questao_id = q.id 
                        join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                        join secao_encaminhamento_naapa secao on secao.id = ens.secao_encaminhamento_id                        
                        left join opcao_resposta opr on opr.id = enr.resposta_id
                         where q.nome_componente = @nomeComponenteFluxoAlerta and secao.etapa = @secaoEtapa1 and secao.ordem = @secaoInformacoesAlunoOrdem
                        ),
                        vw_resposta_data_ultimo_atendimento as (
                        select ens.encaminhamento_naapa_id, 
                               max(to_date(enr.texto,'yyyy-mm-dd')) DataUltimoAtendimento	
                        from encaminhamento_naapa_secao ens   
                        join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                        join questao q on enq.questao_id = q.id 
                        join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                        join secao_encaminhamento_naapa secao on secao.id = ens.secao_encaminhamento_id                        
                        left join opcao_resposta opr on opr.id = enr.resposta_id
                        where q.nome_componente = @nomeComponenteDataAtendimento and secao.etapa = @secaoEtapa1 and secao.ordem = @secaoItineranciaOrdem
                        group by ens.encaminhamento_naapa_id
                        )
		                select en.id, d.dre_id dreId, 
	                        d.abreviacao as dreAbreviacao,
	                        u.ue_id as ueCodigo,
	                        u.nome as ueNome,
	                        u.tipo_escola as tipoEscola,
	                        en.aluno_codigo as alunoCodigo,
	                        en.aluno_nome as alunoNome,
	                        t.turma_id as turmaCodigo,
	                        t.nome as turmaNome,
	                        t.ano_letivo as anoLetivo,
	                        t.modalidade_codigo as modalidade,
                            t.tipo_turno as turmaTipoTurno,
	                        en.situacao as situacao
	                        ,qdata.DataAberturaQueixa as DataEntradaQueixa
                            ,qatendimetno.DataUltimoAtendimento
	                        ,qportaentrada.PortaEntradaId as Id
	                        ,qportaentrada.PortaEntrada as Nome
	                        ,qfluxoalerta.FluxoAlertaId as Id
	                        ,qfluxoalerta.FluxoAlerta as Nome
	                    from encaminhamento_naapa en 
                        inner join turma t on t.id = en.turma_id
			                inner join ue u on u.id = t.ue_id 
			                inner join dre d on d.id = u.dre_id
			                left join vw_resposta_data qdata on qdata.encaminhamento_naapa_id = en.id
			                left join vw_resposta_porta_entrada qportaentrada on qportaentrada.encaminhamento_naapa_id = en.id
			                left join vw_resposta_fluxo_alerta qfluxoalerta on qfluxoalerta.encaminhamento_naapa_id = en.id
                            left join vw_resposta_data_ultimo_atendimento qatendimetno on qatendimetno.encaminhamento_naapa_id = en.id
                            where not en.excluido and
                                  t.historica = @consideraHistorico and
                                  t.ano_letivo = @anoLetivo
                            ");

            query.AppendLine(ObterCondicao(filtro));
            query.AppendLine(" order by d.abreviacao, u.nome, DataAberturaQueixa;");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            var lookup = new Dictionary<long, EncaminhamentoNAAPASimplesDto>();

            await conexao.QueryAsync<EncaminhamentoNAAPASimplesDto, OpcaoRespostaSimplesDTO, OpcaoRespostaSimplesDTO, EncaminhamentoNAAPASimplesDto>(query.ToString(),
                (encaminhamento, opcaoRespostaPortaEntrada, opcaoRespostaFluxoAlerta) =>
                {
                    EncaminhamentoNAAPASimplesDto encaminhamentoNAAPA;
                    if (!lookup.TryGetValue(encaminhamento.Id, out encaminhamentoNAAPA))
                    {
                        encaminhamentoNAAPA = encaminhamento;
                        lookup.Add(encaminhamento.Id, encaminhamento);
                    }
                    if (opcaoRespostaPortaEntrada != null && opcaoRespostaPortaEntrada.Id != 0)
                        encaminhamentoNAAPA.PortaEntrada = opcaoRespostaPortaEntrada.Nome;
                    if (opcaoRespostaFluxoAlerta != null && opcaoRespostaFluxoAlerta.Id != 0)
                        encaminhamentoNAAPA.AdicionarFluxoAlerta(opcaoRespostaFluxoAlerta.Nome);
                    return encaminhamentoNAAPA;
                },
                new
                {
                    consideraHistorico = filtro.ConsideraHistorico,
                    anoLetivo = filtro.AnoLetivo,
                    dreCodigo = filtro.DreCodigo,
                    ueCodigo = filtro.UeCodigo,
                    modalidades = filtro.Modalidades.Select(m => (int)m).ToArray(),
                    anosEscolaresCodigos = filtro.AnosEscolaresCodigos,
                    situacaoIds = filtro.SituacaoIds,                    
                    portaEntradaIds = filtro.PortaEntradaIds,
                    fluxoAlertaIds = filtro.FluxoAlertaIds,
                    nomeComponenteDataEntradaQueixa = NOME_COMPONENTE_DATA_ENTRADA_QUEIXA,
                    nomeComponentePortaEntrada = NOME_COMPONENTE_PORTA_ENTRADA,
                    nomeComponenteFluxoAlerta = NOME_COMPONENTE_FLUXO_ALERTA,
                    nomeComponenteDataAtendimento = NOME_COMPONENTE_DATA_DO_ATENDIMENTO,
                    secaoEtapa1 = SECAO_ETAPA_1,
                    secaoInformacoesAlunoOrdem = SECAO_INFORMACOES_ALUNO_ORDEM,
                    secaoItineranciaOrdem = SECAO_ITINERANCIA_ORDEM
                }, splitOn: "id,id,id");
            return lookup.Values;
        }

        public async Task<IEnumerable<EncaminhamentoNAAPADto>> ObterEncaminhamentosNAAPAPorIds(long[] encaminhamentoNaapaIds)
        {
            var query = new StringBuilder();

            query.Append(" select en.id,");
            query.Append(" d.dre_id dreId,");
            query.Append(" d.abreviacao as dreAbreviacao,");
            query.Append(" u.ue_id as ueCodigo,");
            query.Append(" u.nome as ueNome,");
            query.Append(" u.tipo_escola as tipoEscola,");
            query.Append(" en.aluno_codigo as alunoCodigo,");
            query.Append(" en.aluno_nome as alunoNome,");
            query.Append(" t.turma_id as turmaCodigo,");
            query.Append(" t.nome as turmaNome,");
            query.Append(" t.ano_letivo as anoLetivo,");
            query.Append(" t.modalidade_codigo as modalidade,");
            query.Append(" en.situacao as situacao,");
            query.Append(" en.criado_em as CriadoEm,");
            query.Append(" t.tipo_turno as Turno  ");
            query.Append(" from encaminhamento_naapa en");
            query.Append(" inner join turma t on t.id = en.turma_id");
            query.Append(" inner join ue u on u.id = t.ue_id");
            query.Append(" inner join dre d on d.id = u.dre_id");
            query.Append(" where not en.excluido and en.id = any(@encaminhamentoNaapaIds)");
            query.Append(" order by d.abreviacao, u.nome, en.aluno_codigo");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<EncaminhamentoNAAPADto>(query.ToString(), new { encaminhamentoNaapaIds });
        }
    }
}
