using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoHistoricoEscolar
    {
        private const int TOTAL_CARACTER_LINHA = 95;
        private const int TOTAL_LINHAS = 38;
        private const int TOTAL_LINHAS_ASSINATURA = 10;
        private readonly IEnumerable<HistoricoEscolarDTO> historicoEscolarDTOs;
        private Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarDTO, int, List<RelatorioPaginadoHistoricoEscolarDto>>> dicionarioSecao;
        private int totalLinhaPaginaAtual = 0;
        private RelatorioPaginadoHistoricoEscolarDto PaginaAtual;

        public RelatorioPaginadoHistoricoEscolar(IEnumerable<HistoricoEscolarDTO> historicoEscolarDTOs)
        {
            this.historicoEscolarDTOs = historicoEscolarDTOs;
        }

        public IEnumerable<RelatorioHistoricoEscolarDto> ObterRelatorioPaginadoFundamental()
        {
            dicionarioSecao = ObterDicionarioPorSecaoFundamental();

            return ObterRelatorioPaginado();
        }

        private IEnumerable<RelatorioHistoricoEscolarDto> ObterRelatorioPaginado()
        {
            var relatorios = new List<RelatorioHistoricoEscolarDto>();

            foreach (var historicoEscolar in historicoEscolarDTOs)
            {
                relatorios.Add(ObterRelatorio(historicoEscolar));
            }

            return relatorios;
        }

        private RelatorioHistoricoEscolarDto ObterRelatorio(HistoricoEscolarDTO historicoEscolar)
        {
            var relatorio = new RelatorioHistoricoEscolarDto()
            {
                RelatorioPaginadados = ObterRelatorioPaginado(historicoEscolar),
            };

            relatorio.TotalPagina = relatorio.RelatorioPaginadados.Count();

            return relatorio;
        }

        private IEnumerable<RelatorioPaginadoHistoricoEscolarDto> ObterRelatorioPaginado(HistoricoEscolarDTO historicoEscolar)
        {
            var paginas = new List<RelatorioPaginadoHistoricoEscolarDto>();

            foreach(var funcao in this.dicionarioSecao.Values)
            {
                paginas.AddRange(funcao(historicoEscolar, paginas.Count()));
                totalLinhaPaginaAtual = 0;
            }

            return paginas;
        }

        private Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarDTO, int,List<RelatorioPaginadoHistoricoEscolarDto>>> ObterDicionarioPorSecaoFundamental()
        {
            return new Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarDTO, int, List<RelatorioPaginadoHistoricoEscolarDto>>>()
            {
                { SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosFundamental, ObterPaginasTabelaHistoricoTodosAnos },
                { SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental, ObterPaginasTabelaDoAnoAtual }
            };
        }

        private List<RelatorioPaginadoHistoricoEscolarDto> ObterPaginasTabelaHistoricoTodosAnos(HistoricoEscolarDTO historicoEscolar, int pagina)
        {
            var paginas = new List<RelatorioPaginadoHistoricoEscolarDto>();

            if (historicoEscolar.DadosHistorico != null)
            {
                PaginaAtual = CriaPagina(historicoEscolar, pagina);
                AdicionarSecaoPagina(ObterQuantidadeLinhaDadosHistorico(historicoEscolar), paginas, historicoEscolar, SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosFundamental);
                AdicionarSecaoPagina(ObterQuantidadeLinhaEstudoRealizado(historicoEscolar), paginas, historicoEscolar, SecaoViewHistoricoEscolar.EstudosRealizados);

                if (historicoEscolar.DadosTransferencia == null)
                    CarregarSecoesObservacoes(historicoEscolar, paginas);

                paginas.Add(PaginaAtual);
            }

            return paginas;
        }

        private List<RelatorioPaginadoHistoricoEscolarDto> ObterPaginasTabelaDoAnoAtual(HistoricoEscolarDTO historicoEscolar, int pagina)
        {
            var paginas = new List<RelatorioPaginadoHistoricoEscolarDto>();

            if (historicoEscolar.DadosTransferencia != null)
            {
                PaginaAtual = CriaPagina(historicoEscolar, pagina);
                AdicionarSecaoPagina(ObterQuantidadeLinhasTransferencia(historicoEscolar), paginas, historicoEscolar, SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental);
                CarregarSecoesObservacoes(historicoEscolar, paginas);

                paginas.Add(PaginaAtual);
            }

            return paginas;
        }

        private void CarregarSecoesObservacoes(HistoricoEscolarDTO historicoEscolar, List<RelatorioPaginadoHistoricoEscolarDto> paginas)
        {
            if (!string.IsNullOrEmpty(historicoEscolar.ObservacaoComplementar))
                AdicionarSecaoPagina(ObterQuantidadeLinhaObsevacaoAssinatura(historicoEscolar), paginas, historicoEscolar, SecaoViewHistoricoEscolar.Observacoes);

            AdicionarSecaoPagina(TOTAL_LINHAS_ASSINATURA, paginas, historicoEscolar, SecaoViewHistoricoEscolar.Assinaturas);
        }

        private int ObterQuantidadeLinhaDadosHistorico(HistoricoEscolarDTO historicoEscolar)
        {
            var linhas = 0;
            var dadosHistoricoDto = historicoEscolar.DadosHistorico;

            if (dadosHistoricoDto.BaseNacionalComum != null)
                foreach (var area in dadosHistoricoDto.BaseNacionalComum.AreasDeConhecimento)
                    linhas += area.ComponentesCurriculares.Count();

            linhas += dadosHistoricoDto.EnriquecimentoCurricular != null ? dadosHistoricoDto.EnriquecimentoCurricular.Count() : 0;

            linhas += dadosHistoricoDto.ProjetosAtividadesComplementares != null ? dadosHistoricoDto.ProjetosAtividadesComplementares.Count() : 0;

            linhas += dadosHistoricoDto.GruposComponentesCurriculares != null ? dadosHistoricoDto.GruposComponentesCurriculares.Count() : 0;

            linhas += dadosHistoricoDto.ParecerConclusivo != null ? 1 : 0;

            return linhas;
        }

        private int ObterQuantidadeLinhaEstudoRealizado(HistoricoEscolarDTO historicoEscolar)
        {
            return historicoEscolar.EstudosRealizados != null ? historicoEscolar.EstudosRealizados.Count() : 0;
        }

        private int ObterQuantidadeLinhaObsevacaoAssinatura(HistoricoEscolarDTO historicoEscolar)
        {
            if (historicoEscolar.ObservacaoComplementar.Length > TOTAL_CARACTER_LINHA)
                return (int)Math.Round((double)historicoEscolar.ObservacaoComplementar.Length / TOTAL_CARACTER_LINHA);

            return 1;
        }

        private int ObterQuantidadeLinhasTransferencia(HistoricoEscolarDTO historicoEscolar)
        {
            var linhas = 0;
            var transferencia = historicoEscolar.DadosTransferencia;

            if (transferencia.BaseNacionalComum != null)
                foreach (var area in transferencia.BaseNacionalComum.AreasDeConhecimento)
                    linhas += area.ComponentesCurriculares.Count();

            linhas += transferencia.EnriquecimentoCurricular != null ? transferencia.EnriquecimentoCurricular.Count() : 0;

            linhas += transferencia.ProjetosAtividadesComplementares != null ? transferencia.ProjetosAtividadesComplementares.Count() : 0;

            linhas += transferencia.GruposComponentesCurriculares != null ? transferencia.GruposComponentesCurriculares.Count() : 0;

            return linhas;
        }

        private RelatorioPaginadoHistoricoEscolarDto CriaPagina(HistoricoEscolarDTO historicoEscolar, int pagina = 0)
        {
            return new RelatorioPaginadoHistoricoEscolarDto()
            {
                HistoricoEscolar = historicoEscolar,
                SecoesPorPagina = new List<SecaoViewHistoricoEscolar>(),
                Pagina = pagina + 1
            };
        }

        private void AdicionarSecaoPagina(int linhasPagina, List<RelatorioPaginadoHistoricoEscolarDto> paginas, HistoricoEscolarDTO historico, SecaoViewHistoricoEscolar secao)
        {
            if ((totalLinhaPaginaAtual + linhasPagina) > TOTAL_LINHAS)
            {
                totalLinhaPaginaAtual = linhasPagina;
                paginas.Add(PaginaAtual);
                PaginaAtual = CriaPagina(historico, PaginaAtual.Pagina);
            }
            else
            {
                totalLinhaPaginaAtual += linhasPagina;
            }

            PaginaAtual.SecoesPorPagina.Add(secao);
        }
    }
}
