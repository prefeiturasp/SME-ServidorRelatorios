using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoHistoricoEscolar
    {
        private readonly IEnumerable<HistoricoEscolarDTO> historicoEscolarDTOs;
        private Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarDTO, IEnumerable<SecaoViewHistoricoEscolar>>> dicionarioSecao;

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
            var relatorios = new List<RelatorioPaginadoHistoricoEscolarDto>();
            var pagina = 1;

            foreach(var funcao in this.dicionarioSecao.Values)
            {
                var secoes = funcao(historicoEscolar);

                if (secoes.Any())
                {
                    relatorios.Add(new RelatorioPaginadoHistoricoEscolarDto()
                    {
                        HistoricoEscolar = historicoEscolar,
                        SecoesPorPagina = secoes,
                        PaginaAtual = pagina
                    });

                    pagina++;
                }
            }

            return relatorios;
        }

        private Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarDTO, IEnumerable<SecaoViewHistoricoEscolar>>> ObterDicionarioPorSecaoFundamental()
        {
            return new Dictionary<SecaoViewHistoricoEscolar, Func<HistoricoEscolarDTO, IEnumerable<SecaoViewHistoricoEscolar>>>()
            {
                { SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosFundamental, ObterSecoesTabelaHistoricoTodosAnos },
                { SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental, ObterSecoesTabelaDoAnoAtual }
            };
        }

        private IEnumerable<SecaoViewHistoricoEscolar> ObterSecoesTabelaHistoricoTodosAnos(HistoricoEscolarDTO historicoEscolar)
        {
            var secoes = new List<SecaoViewHistoricoEscolar>();

            if (historicoEscolar.DadosHistorico != null)
            {
                secoes.Add(SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosFundamental);
                secoes.Add(SecaoViewHistoricoEscolar.EstudosRealizados);

                if (historicoEscolar.DadosTransferencia == null)
                    secoes.AddRange(ObterSecoesObservacoes(historicoEscolar));
            }

            return secoes;
        }

        private IEnumerable<SecaoViewHistoricoEscolar> ObterSecoesTabelaDoAnoAtual(HistoricoEscolarDTO historicoEscolar)
        {
            var secoes = new List<SecaoViewHistoricoEscolar>();

            if (historicoEscolar.DadosTransferencia != null)
            {
                secoes.Add(SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental);

                secoes.AddRange(ObterSecoesObservacoes(historicoEscolar));
            }

            return secoes;
        }

        private IEnumerable<SecaoViewHistoricoEscolar> ObterSecoesObservacoes(HistoricoEscolarDTO historicoEscolar)
        {
            var secoes = new List<SecaoViewHistoricoEscolar>();

            if (!string.IsNullOrEmpty(historicoEscolar.ObservacaoComplementar))
                secoes.Add(SecaoViewHistoricoEscolar.Observacoes);

            secoes.Add(SecaoViewHistoricoEscolar.Assinaturas);

            return secoes;
        }
    }
}
