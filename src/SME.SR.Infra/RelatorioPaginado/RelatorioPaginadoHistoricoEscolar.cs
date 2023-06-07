using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public abstract class RelatorioPaginadoHistoricoEscolar<HistoricoEscolarGenerico> where HistoricoEscolarGenerico : HistoricoEscolarDto
    {
        protected const int TOTAL_CARACTER_LINHA = 95;
        protected const int TOTAL_LINHAS = 38;
        protected const int TOTAL_LINHAS_ASSINATURA = 10;
        protected readonly IEnumerable<HistoricoEscolarGenerico> historicoEscolarDTOs;
        protected Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarGenerico, int, List<RelatorioPaginadoHistoricoEscolarDto>>> dicionarioSecao;
        protected int totalLinhaPaginaAtual = 0;
        protected RelatorioPaginadoHistoricoEscolarDto PaginaAtual;
        protected SecaoViewHistoricoEscolar TabelaHistoricoTodosAnos { get; set; }
        protected SecaoViewHistoricoEscolar TabelaAnoAtual { get; set; }


        public RelatorioPaginadoHistoricoEscolar(IEnumerable<HistoricoEscolarGenerico> historicoEscolarDTOs)
        {
            this.historicoEscolarDTOs = historicoEscolarDTOs;
        }

        public IEnumerable<RelatorioHistoricoEscolarDto> ObterRelatorioPaginado()
        {
            dicionarioSecao = ObterDicionarioPorSecao();

            var relatorios = new List<RelatorioHistoricoEscolarDto>();

            foreach (var historicoEscolar in historicoEscolarDTOs)
            {
                relatorios.Add(ObterRelatorio(historicoEscolar));
            }

            return relatorios;
        }

        private RelatorioHistoricoEscolarDto ObterRelatorio(HistoricoEscolarGenerico historicoEscolar)
        {
            var relatorio = new RelatorioHistoricoEscolarDto()
            {
                RelatorioPaginadados = ObterRelatorioPaginado(historicoEscolar),
            };

            relatorio.TotalPagina = relatorio.RelatorioPaginadados.Count();

            return relatorio;
        }

        private IEnumerable<RelatorioPaginadoHistoricoEscolarDto> ObterRelatorioPaginado(HistoricoEscolarGenerico historicoEscolar)
        {
            var paginas = new List<RelatorioPaginadoHistoricoEscolarDto>();

            foreach(var funcao in this.dicionarioSecao.Values)
            {
                paginas.AddRange(funcao(historicoEscolar, paginas.Count()));
                totalLinhaPaginaAtual = 0;
            }

            return paginas;
        }

        protected virtual Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarGenerico, int, List<RelatorioPaginadoHistoricoEscolarDto>>> ObterDicionarioPorSecao()
        {
            return new Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarGenerico, int, List<RelatorioPaginadoHistoricoEscolarDto>>>()
            {
                { TabelaHistoricoTodosAnos, ObterPaginasTabelaHistoricoTodosAnos },
                { TabelaAnoAtual, ObterPaginasTabelaDoAnoAtual }
            };
        }

        protected virtual List<RelatorioPaginadoHistoricoEscolarDto> ObterPaginasTabelaHistoricoTodosAnos(HistoricoEscolarGenerico historicoEscolar, int pagina)
        {
            var paginas = new List<RelatorioPaginadoHistoricoEscolarDto>();

            if (historicoEscolar.ContemDadosHistorico())
            {
                PaginaAtual = CriaPagina(historicoEscolar, pagina);
                AdicionarSecaoPagina(ObterQuantidadeLinhaDadosHistorico(historicoEscolar), paginas, historicoEscolar, TabelaHistoricoTodosAnos);
                AdicionarSecaoPagina(ObterQuantidadeLinhaEstudoRealizado(historicoEscolar), paginas, historicoEscolar, SecaoViewHistoricoEscolar.EstudosRealizados);
                if (!historicoEscolar.ContemDadosTransferencia())
                    CarregarSecoesObservacoes(historicoEscolar, paginas);
                paginas.Add(PaginaAtual);
            }

            return paginas;
        }

        protected virtual List<RelatorioPaginadoHistoricoEscolarDto> ObterPaginasTabelaDoAnoAtual(HistoricoEscolarGenerico historicoEscolar, int pagina)
        {
            var paginas = new List<RelatorioPaginadoHistoricoEscolarDto>();

            if (historicoEscolar.ContemDadosTransferencia())
            {
                PaginaAtual = CriaPagina(historicoEscolar, pagina);
                AdicionarSecaoPagina(ObterQuantidadeLinhasTransferencia(historicoEscolar), paginas, historicoEscolar, TabelaAnoAtual);
                CarregarSecoesObservacoes(historicoEscolar, paginas);

                paginas.Add(PaginaAtual);
            }

            return paginas;
        }

        protected void CarregarSecoesObservacoes(HistoricoEscolarGenerico historicoEscolar, List<RelatorioPaginadoHistoricoEscolarDto> paginas)
        {
            if (!string.IsNullOrEmpty(historicoEscolar.ObservacaoComplementar))
                AdicionarSecaoPagina(ObterQuantidadeLinhaObsevacaoAssinatura(historicoEscolar), paginas, historicoEscolar, SecaoViewHistoricoEscolar.Observacoes);

            AdicionarSecaoPagina(TOTAL_LINHAS_ASSINATURA, paginas, historicoEscolar, SecaoViewHistoricoEscolar.Assinaturas);
        }

        protected abstract int ObterQuantidadeLinhaDadosHistorico(HistoricoEscolarGenerico historicoEscolar);

        protected int ObterQuantidadeLinhaEstudoRealizado(HistoricoEscolarGenerico historicoEscolar)
        {
            return historicoEscolar.EstudosRealizados != null ? historicoEscolar.EstudosRealizados.Count() : 0;
        }

        private int ObterQuantidadeLinhaObsevacaoAssinatura(HistoricoEscolarDto historicoEscolar)
        {
            if (historicoEscolar.ObservacaoComplementar.Length > TOTAL_CARACTER_LINHA)
                return 1 + (int)Math.Round((double)historicoEscolar.ObservacaoComplementar.Length / TOTAL_CARACTER_LINHA);

            return 2;
        }

        protected int ObterQuantidadeLinhasTransferencia(HistoricoEscolarDto historicoEscolar)
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

        protected RelatorioPaginadoHistoricoEscolarDto CriaPagina(HistoricoEscolarDto historicoEscolar, int pagina = 0)
        {
            return new RelatorioPaginadoHistoricoEscolarDto()
            {
                HistoricoEscolar = historicoEscolar,
                SecoesPorPagina = new List<SecaoViewHistoricoEscolar>(),
                Pagina = pagina + 1
            };
        }

        protected void AdicionarSecaoPagina(int linhasPagina, List<RelatorioPaginadoHistoricoEscolarDto> paginas, HistoricoEscolarDto historico, SecaoViewHistoricoEscolar secao)
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
