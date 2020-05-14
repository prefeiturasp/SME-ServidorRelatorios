using SME.SR.Infra.Dtos;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.GetRelatoriosTarefasEmAndamento;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExecucaoRelatorioAsync;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExportacaoRelatorioAsync;
using SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostModificarParametrosRelatorio;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetDetalhesExecucaoRelatorio;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.GetRelatoriosTarefasEmAndamento;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExecucaoRelatorioAsync;
using SME.SR.Infra.Dtos.Resposta.ExecucaoRelatorio.PostExportacaoRelatorioAsync;
using System;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface IExecucaoRelatorioService
    {
        Task<ExecucaoRelatorioRespostaDto> PostAsync(ExecucaoRelatorioRequisicaoDto requisicao);

        Task<string> ObterPool(Guid requisicaoId);

        Task<DetalhesExecucaoRelatorioRespostaDto> ObterDetalhes(Guid requisicaoId);

        Task<string> ObterSaida(Guid requisicaoId, Guid exportacaoId);

        Task<string> ObterAnexos(Guid requisicaoId, Guid exportacaoId, string nomeArquivo);

        Task<ExportacaoRelatorioRespostaDto> PostExportacao(Guid requisicaoId, ExportacaoRelatorioRequisicaoDto requisicao);

        Task<string> PostModificarParametros(Guid requisicaoId, ModificarParametrosRelatorioRequisicaoDto requisicao);

        Task<string> ObterPoolExportacao(Guid requisicaoId, Guid exportacaoId);

        Task<RelatoriosTarefasEmAndamentoRespostaDto> ObterRelatoriosTarefasEmAndamento(RelatoriosTarefasEmAndamentoRequisicaoDto requisicao);

        Task<string> InterromperRelatoriosTarefas(Guid requisicaoId);
    }
}
