﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using SME.SR.Data.Models.Conecta;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Word;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands
{
    public class GerarRelatorioLaudaDePublicacaoParceiraDocCommandHandler : AbstractGeradorArquivoDoc, IRequestHandler<GerarRelatorioLaudaDePublicacaoParceiraDocCommand, string>
    {
        private Proposta proposta;
        private Paragraph paragrafo;
        private const string RESPOSTA_OUTROS = "OUTROS";

        public async Task<string> Handle(GerarRelatorioLaudaDePublicacaoParceiraDocCommand request, CancellationToken cancellationToken)
        {
            var nomeArquivo = Guid.NewGuid().ToString();

            this.proposta = request.Proposta;
            this.paragrafo = new Paragraph();

            GerarArquivoDoc(nomeArquivo);

            return nomeArquivo;
        }

        protected override void CriarContexto(Body corpo, MainDocumentPart mainPart)
        {
            CarregarDepacho();
            CarregarPeriodoRealizacao();
            CarregarCargaHorariaVagas();
            CarregarPublicoAlvo();
            CarregarInscricoes(mainPart);
            CarregarCertificacao();
            CarregarAreaPromotora();

            corpo.AdicionarParagrafo(paragrafo);
        }

        private void CarregarDepacho()
        {
            paragrafo.InserirTextoNegrito("DESPACHO DE HOMOLOGAÇÃO Nº ");
            paragrafo.InserirTexto(proposta.NumeroHomologacao.ToString(), true);
            paragrafo.InserirTextoNegrito("CURSO: ");
            paragrafo.InserirTexto(proposta.NomeFormacao, true);
        }

        private void CarregarPeriodoRealizacao()
        {
            paragrafo.InserirTextoNegrito("PERÍODO DE REALIZAÇÃO: ", true);
            paragrafo.InserirTexto(proposta.ObterPeriodoRealizacao(), true);

            var local = proposta.ObterLocalVariasTurmasUmEncontro();

            if (!string.IsNullOrEmpty(local))
                paragrafo.InserirTexto(local, true);
        }

        private void CarregarCargaHorariaVagas()
        {
            paragrafo.InserirTextoNegrito("CARGA HORÁRIA: ");
            paragrafo.InserirTexto(proposta.TotalCargaHoraria.ToString(), true);
            paragrafo.InserirTextoNegrito("TOTAL DE VAGAS: ");
            paragrafo.InserirTexto(proposta.TotalDeVagas.ToString(), true);
            paragrafo.InserirTexto(String.Format("{0} VAGAS POR TURMA", proposta.QuantidadeVagasTurmas), true);
            paragrafo.InserirTexto(String.Format("QUANTIDADE DE TURMAS: {0}", proposta.QuantidadeTurmas), true);
        }

        private void CarregarPublicoAlvo()
        {
            var publicosAlvo = proposta.PublicosAlvo.ToList();
            publicosAlvo.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = proposta.PublicoAlvo_Outros;
            });

            paragrafo.InserirTextoNegrito("PÚBLICO-ALVO: ", true);

            foreach (var cargoFuncao in publicosAlvo)
                paragrafo.InserirTexto($@"{cargoFuncao.Nome.ToUpper()}{(string.IsNullOrEmpty(cargoFuncao.DescricaoAdicional)
                                                                       ? string.Empty : $": {cargoFuncao.DescricaoAdicional.ToUpper()}")}", true);
        }

        private void CarregarInscricoes(MainDocumentPart mainPart)
        {
            var criteriosValidacao = proposta.CriteriosValidacao.ToList();
            criteriosValidacao.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = proposta.CriteriosValidacao_Outros;
            });

            paragrafo.InserirTextoNegrito("INSCRIÇÕES: ");
            var textoPeriodo = "DE {0} ATÉ {1}, PELO LINK: ";
            paragrafo.InserirTexto(String.Format(textoPeriodo, proposta.DataInscricaoInicio.ToString("dd/MM/yyyy"), proposta.DataInscricaoFim.ToString("dd/MM/yyyy")));
            paragrafo.InserirLink(proposta.LinkInscricaoExterna, proposta.LinkInscricaoExterna, mainPart);
            paragrafo.InserirTexto(string.Empty, true);

            foreach (var criterio in criteriosValidacao)
                paragrafo.InserirTexto($@"{criterio.Nome.ToUpper()}{(string.IsNullOrEmpty(criterio.DescricaoAdicional)
                                                          ? string.Empty : $": {criterio.DescricaoAdicional.ToUpper()}")}", true);
        }

        private void CarregarCertificacao()
        {
            var criteriosCertificacao = proposta.CriteriosCertificacao.ToList();
            criteriosCertificacao.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = proposta.Criterios_Outros;
            });

            paragrafo.InserirTextoNegrito("CERTIFICAÇÃO: ", true);

            foreach (var certificacao in criteriosCertificacao)
                paragrafo.InserirTexto($@"{certificacao.Nome.ToUpper()}{(string.IsNullOrEmpty(certificacao.DescricaoAdicional)
                                                            ? string.Empty : $": {certificacao.DescricaoAdicional.ToUpper()}")}", true);
        }

        private void CarregarAreaPromotora()
        {
            paragrafo.InserirTextoNegrito("ÁREA PROMOTORA: ");
            paragrafo.InserirTexto(proposta.NomeAreaPromotora.ToUpper(), true);
            paragrafo.InserirTextoNegrito("DESPACHO: ");
            paragrafo.InserirTexto("À VISTA DO CONTIDO NA INSTRUÇÃO NORMATIVA SME Nº 48, DE 10/12/2020, APÓS ANÁLISE DA PROPOSTA DO CURSO, CONSIDERO ");
            paragrafo.InserirTextoNegrito("HOMOLOGADO.");
        }
    }
}
