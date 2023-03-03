using Dapper;
using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class EncaminhamentoNAAPARepository : IEncaminhamentoNAAPARepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private const int SECAO_ETAPA_1 = 1;
        private const int SECAO_INFORMACOES_ALUNO_ORDEM = 1;
        private const string NOME_COMPONENTE_DATA_ENTRADA_QUEIXA = "DATA_ENTRADA_QUEIXA";
        private const string NOME_COMPONENTE_PORTA_ENTRADA = "PORTA_ENTRADA";
        private const string NOME_COMPONENTE_FLUXO_ALERTA = "FLUXO_ALERTA";

        public EncaminhamentoNAAPARepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        private string ObterCondicaoDre(FiltroRelatorioEncaminhamentoNAAPADto filtro) =>
                    !filtro.DreCodigo.EstaFiltrandoTodas() ? " and d.dre_id = @dreCodigo " : string.Empty;

        private string ObterCondicaoUe(FiltroRelatorioEncaminhamentoNAAPADto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;

        
        private string ObterCondicaoSituacao(FiltroRelatorioEncaminhamentoNAAPADto filtro)  
        {
            var condicao = string.Empty;

            if (!filtro.ExibirEncerrados)
                condicao += $" and ea.situacao <> {(int)SituacaoNAAPA.Encerrado}";
            if (!filtro.SituacaoIds.EstaFiltrandoTodas())
                condicao += " and ea.situacao = ANY(@situacaoIds) ";

            return condicao;
        }

        private string ObterCondicaoPortaEntrada(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var condicao = string.Empty;

            if (filtro.PortaEntradaIds.Any())
                condicao += $" and qportaentrada.PortaEntradaId = ANY(@portaEntradaIds)";

            return condicao;
        }

        private string ObterCondicaoFluxoAlerta(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var condicao = string.Empty;

            if (filtro.FluxoAlertaIds.Any())
                condicao += $" and qfluxoalerta.FluxoAlertaId = ANY(@fluxoAlertaIds)";

            return condicao;
        }


        private string ObterCondicao(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioEncaminhamentoNAAPADto, string>>
            {
                ObterCondicaoDre,
                ObterCondicaoUe,
                ObterCondicaoSituacao,
                ObterCondicaoPortaEntrada,
                ObterCondicaoFluxoAlerta
            };

            foreach(var funcao in funcoes)
            {
                query.Append(funcao(filtro));
            }

            return query.ToString();
        }

        public async Task<IEnumerable<EncaminhamentoNAAPADto>> ObterEncaminhamentosNAAPA(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            var query = new StringBuilder();

            query.AppendLine($@"     
                    with vw_resposta_data as (
                        select ens.encaminhamento_naapa_id, 
                               enr.texto DataAberturaQueixaInicio	
                        from encaminhamento_naapa_secao ens   
                        join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                        join questao q on enq.questao_id = q.id 
                        join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                        join secao_encaminhamento_naapa secao on secao.id = ens.secao_encaminhamento_id
                        left join opcao_resposta opr on opr.id = enr.resposta_id
                        where q.nome_componente = {NOME_COMPONENTE_DATA_ENTRADA_QUEIXA} and secao.etapa = {SECAO_ETAPA_1} and secao.ordem = {SECAO_INFORMACOES_ALUNO_ORDEM}
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
                         where q.nome_componente = {NOME_COMPONENTE_PORTA_ENTRADA} and secao.etapa = {SECAO_ETAPA_1} and secao.ordem = {SECAO_INFORMACOES_ALUNO_ORDEM}
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
                         where q.nome_componente = {NOME_COMPONENTE_FLUXO_ALERTA} and secao.etapa = {SECAO_ETAPA_1} and secao.ordem = {SECAO_INFORMACOES_ALUNO_ORDEM}
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
	                        en.situacao as situacao
	                        ,case when length(qdata.DataAberturaQueixaInicio) > 0 then to_date(qdata.DataAberturaQueixaInicio,'yyyy-mm-dd') else null end DataEntradaQueixa
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
                            where not en.excluido    
                            ");

            query.AppendLine(ObterCondicao(filtro));

            query.AppendLine(" order by d.abreviacao, u.nome, case when length(qdata.DataAberturaQueixaInicio) > 0 then to_date(qdata.DataAberturaQueixaInicio,'yyyy-mm-dd') else null end ;   ");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            var lookup = new Dictionary<long, EncaminhamentoNAAPADto>();

            return await conexao.QueryAsync<EncaminhamentoNAAPADto, OpcaoRespostaSimplesDTO, OpcaoRespostaSimplesDTO, EncaminhamentoNAAPADto>(query.ToString(), 
                (encaminhamento, opcaoRespostaPortaEntrada, opcaoRespostaFluxoAlerta) =>
                {
                    EncaminhamentoNAAPADto encaminhamentoNAAPA;
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
                dreCodigo = filtro.DreCodigo,
                ueCodigo = filtro.UeCodigo,
                situacaoIds = filtro.SituacaoIds,
                portaEntradaIds = filtro.PortaEntradaIds,
                fluxoAlertaIds = filtro.FluxoAlertaIds               
            }, splitOn: "id,id,id");
        }
    }
}
