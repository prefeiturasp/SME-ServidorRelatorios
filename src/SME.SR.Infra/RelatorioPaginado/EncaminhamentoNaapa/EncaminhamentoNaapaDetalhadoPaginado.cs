using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class EncaminhamentoNaapaDetalhadoPaginado
    {
        private const int TOTAL_LINHAS = 35;

        private int TotalLinhaPaginaAtual { get; set; }
        private List<EncaminhamentoNaapaDetalhadoPagina> Paginas { get; set; }
        private EncaminhamentoNaapaDetalhadoPagina PaginaAtual { get; set; }
        private RelatorioEncaminhamentoNAAPADetalhadoDto RelatorioDetalhadoDto { get; set; }

        public EncaminhamentoNaapaDetalhadoPaginado()
        {
            Paginas = new List<EncaminhamentoNaapaDetalhadoPagina>();
        }

        public IEnumerable<EncaminhamentoNaapaDetalhadoPagina> ObterPaginas(RelatorioEncaminhamentoNAAPADetalhadoDto dto)
        {
            RelatorioDetalhadoDto = dto;
            PaginaAtual = ObterPagina();
            CarregaPaginas();
            AdicionarPagina();

            return Paginas;
        }

        private void CarregaPaginas()
        {
            CarregueLinhaInformacoes(RelatorioDetalhadoDto.Detalhes?.Informacoes);
            CarregueLinhaQuestoesApresentadas(RelatorioDetalhadoDto.Detalhes?.QuestoesApresentadas);
            CarregueLinhaItinerancia(RelatorioDetalhadoDto.Detalhes?.Itinerancia);
        }

        private void CarregueLinhaInformacoes(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto informacoes)
        {
            if (informacoes == null)
                return;

            AdicionarLinha(new SecaoTituloEncaminhamentoNaapa(informacoes.NomeSecao, string.Empty, true));
            AdicionarLinha(new SecaoInformacoesEncaminhamentoNaapa(informacoes));
            AdicionarSecaoTabela(new SecaoContatoResponsavelEncaminhamentoNaapa(informacoes));
            AdicionarSecaoTabela(new SecaoEnderecoEncaminhamentoNaapa(informacoes));
            AdicionarLinha(new SecaoFiliacaoEncaminhamentoNaapa(informacoes));
            AdicionarSecaoTabela(new SecaoContraturnoEncaminhamentoNaapa(informacoes));
            AdicionarSecaoTabela(new SecaoTurmaProgramaEncaminhamentoNaapa(informacoes));
            AdicionarSecaoInformacoesTipoRespostaTexto(informacoes);
        }

        private void CarregueLinhaQuestoesApresentadas(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto questoesApresentadas)
        {
            if (questoesApresentadas == null)
                return;

            AdicionarLinha(new SecaoTituloEncaminhamentoNaapa(questoesApresentadas.NomeSecao));
            AdicionarLinhaHipoteseDeEscrita(questoesApresentadas);

            var questoes = questoesApresentadas.Questoes.FindAll(q => q.NomeComponente != NomeComponentesEncaminhamentoNaapa.HIPOTESE_ESCRITA);

            foreach (var questao in questoes)
                AdicioneSecaoRespostaTexto(questao);
        }

        private void CarregueLinhaItinerancia(IEnumerable<SecaoQuestoesEncaminhamentoNAAPADetalhadoDto> itinerancias)
        {
            if (itinerancias == null || !itinerancias.Any())
                return;

            AdicionarLinha(new SecaoTituloEncaminhamentoNaapa("ITINERÂNCIA", "HISTÓRICO DOS ATENDIMENTOS"));

            foreach(var itinerancia in itinerancias)
            {
                var nomesComponentes = new string[] { NomeComponentesEncaminhamentoNaapa.TIPO_DO_ATENDIMENTO, NomeComponentesEncaminhamentoNaapa.PROCEDIMENTO_DE_TRABALHO };
                var questoes = itinerancia.Questoes.FindAll(q => !nomesComponentes.Contains(q.NomeComponente));

                foreach (var questao in questoes)
                {
                    if (questao.NomeComponente == NomeComponentesEncaminhamentoNaapa.DATA_DO_ATENDIMENTO)
                        AdicionarLinha(new SecaoItensItineranciaEncaminhamentoNaapa(questao, itinerancia));
                    else
                        AdicioneSecaoRespostaTexto(questao);
                }
            }
        }

        private void AdicionarLinhaHipoteseDeEscrita(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto questoesApresentadas)
        {
            var questao = questoesApresentadas.Questoes.Find(q => q.NomeComponente == NomeComponentesEncaminhamentoNaapa.HIPOTESE_ESCRITA);

            if (!string.IsNullOrEmpty(questao.Resposta))
            {
                questao.Resposta = $"{questao.Questao}: {questao.Resposta}";
                AdicionarLinha(new SecaoRespostaTextoEncaminhamentoNaapa(questao, true));
            }
        }

        private void AdicionarSecaoInformacoesTipoRespostaTexto(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto informacoes)
        {
            var nomesComponentes = new string[] { NomeComponentesEncaminhamentoNaapa.DESCRICAO_ENCAMINHAMENTO, NomeComponentesEncaminhamentoNaapa.FLUXO_ALERTA };
            var questoes = informacoes.Questoes.FindAll(q => nomesComponentes.Contains(q.NomeComponente));

            foreach(var questao in questoes)
                AdicioneSecaoRespostaTexto(questao);

        }

        private void AdicionarSecaoTabela(SecaoTabelaEncaminhamentoNaapa secao)
        {
            var paginasTabela = secao.ObterSecaoTabelaPaginada(TOTAL_LINHAS, TotalLinhaPaginaAtual);

            foreach(var pagina in paginasTabela)
                AdicionarLinha(pagina);
        }

        private void AdicioneSecaoRespostaTexto(QuestaoEncaminhamentoNAAPADetalhadoDto questao)
        {
            if (!string.IsNullOrEmpty(questao.Resposta))
                AdicionarLinha(new SecaoRespostaTextoEncaminhamentoNaapa(questao));
        }

        private void AdicionarLinha(SecaoRelatorioEncaminhamentoNaapa secao)
        {
            var totalLinhaSecao = secao.ObterLinhasDeQuebra();
            TotalLinhaPaginaAtual += totalLinhaSecao;

            if (TotalLinhaPaginaAtual >= TOTAL_LINHAS)
            {
                AdicionarPagina();
                PaginaAtual = ObterPagina(PaginaAtual.Pagina);
                TotalLinhaPaginaAtual = totalLinhaSecao;
                RetiraUltimaSecaoTituloDaPagina();
            }

            AdicionarSecaoPaginaAtual(secao);
        }

        private void AdicionarPagina()
        {
            Paginas.Add(PaginaAtual);
        }

        private void AdicionarSecaoPaginaAtual(SecaoRelatorioEncaminhamentoNaapa secao)
        {
            PaginaAtual.Linhas.Add(secao);
        }

        private void RetiraUltimaSecaoTituloDaPagina()
        {
            var pagina = Paginas.LastOrDefault();
            var ultimaLinha = pagina.Linhas.LastOrDefault(linha => linha.GetType() == typeof(SecaoTituloEncaminhamentoNaapa));

            if (ultimaLinha != null)
            {
                TotalLinhaPaginaAtual += ultimaLinha.ObterLinhasDeQuebra();
                AdicionarSecaoPaginaAtual(ultimaLinha);
                PaginaAtual.Linhas.Remove(ultimaLinha);
            }
        }

        private EncaminhamentoNaapaDetalhadoPagina ObterPagina(int pagina = 0)
        {
            return new EncaminhamentoNaapaDetalhadoPagina()
            {
                Cabecalho = RelatorioDetalhadoDto.Cabecalho,
                Linhas = new List<SecaoRelatorioEncaminhamentoNaapa>(),
                Pagina = pagina + 1
            };
        }
    }
}
