using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using SME.SR.Data.Models.Conecta;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Word;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioLaudaDePublicacaoDiretaDocCommandHandler : AbstractGeradorArquivoDoc, IRequestHandler<GerarRelatorioLaudaDePublicacaoDiretaDocCommand, string>
    {
        private Proposta proposta;
        private Paragraph paragrafo;
        private const string RESPOSTA_OUTROS = "OUTROS";


        public async Task<string> Handle(GerarRelatorioLaudaDePublicacaoDiretaDocCommand request, CancellationToken cancellationToken)
        {
            var nomeArquivo = Guid.NewGuid().ToString();

            this.proposta = request.Proposta;
            this.paragrafo = new Paragraph();

            GerarArquivoDoc(nomeArquivo);

            return nomeArquivo;
        }

        protected override void CriarContexto(Body corpo, MainDocumentPart mainPart)
        {
            CarregarComunicado(mainPart);
            CarregarPublicoAlvo();
            CarregarCronograma();
            CarregarVagas();
            CarregarInscricoes(mainPart);
            CarregarCertificacao();
            CarregarRegentes();
            CarregarAreaPromotora();

            corpo.AdicionarParagrafo(paragrafo);
        }

        private void CarregarComunicado(MainDocumentPart mainPart)
        {
            paragrafo.InserirTexto(proposta.NomeAreaPromotora.ToUpper(), true);
            paragrafo.InserirTexto("COMUNICADO Nº 0, ", true);
            var textoSecretario = @"O SECRETÁRIO MUNICIPAL DE EDUCAÇÃO, NO USO DE SUAS ATRIBUIÇÕES LEGAIS, CONFORME O QUE LHE REPRESENTOU O(A) DIRETOR(A) REGIONAL DE EDUCAÇÃO/COORDENADOR DA DRE/COORDENADORIA {0}, ";
            paragrafo.InserirTexto(String.Format(textoSecretario, proposta.NomeAreaPromotora.ToUpper()));
            paragrafo.InserirTextoNegrito("COMUNICA ");
            var textoRealizacao = "A REALIZAÇÃO DO EVENTO \"{0}\". AS ESPECIFICAÇÕES COMPLETAS DO CURSO ESTÃO NO ACERVO DIGITAL DA SME: ";
            paragrafo.InserirTexto(String.Format(textoRealizacao, proposta.NomeFormacao.ToUpper()));
            paragrafo.InserirLink("https://acervodigital.sme.prefeitura.sp.gov.br/", "https://acervodigital.sme.prefeitura.sp.gov.br/", mainPart);
            paragrafo.InserirTexto(string.Empty, true);
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

            paragrafo.InserirTexto(String.Format("CARGA HORÁRIA TOTAL: {0}", proposta.TotalCargaHoraria));

            if (!string.IsNullOrEmpty(proposta.CargaHorariaPresencial))
                paragrafo.InserirTexto(String.Format(", SENDO {0} HORAS PRESENCIAIS", proposta.CargaHorariaPresencialFormatada));

            if (!string.IsNullOrEmpty(proposta.CargaHorariaSincronaDistancia))
                paragrafo.InserirTexto(String.Format(", {0} HORAS DE ATIVIDADES SÍNCRONAS; E/OU HORAS A DISTÂNCIA", proposta.CargaHorariaSincronaDistancia));

            paragrafo.InserirTexto(".", true);
        }

        private void CarregarCronograma()
        {
            paragrafo.InserirTextoNegrito("CRONOGRAMA: ", true);
            paragrafo.InserirTexto(proposta.ObterPeriodoRealizacao(), true);

            var local = proposta.ObterLocalUmaTurmaUmEncontro();

            if (!string.IsNullOrEmpty(local))
                paragrafo.InserirTexto(local, true);
        }

        private void CarregarVagas()
        {
            paragrafo.InserirTextoNegrito("TOTAL DE VAGAS: ");
            paragrafo.InserirTexto(proposta.TotalDeVagas.ToString(), true);
            paragrafo.InserirTexto(String.Format("{0} VAGAS POR TURMA", proposta.QuantidadeVagasTurmas), true);
            paragrafo.InserirTexto(String.Format("QUANTIDADE DE TURMAS: {0}", proposta.QuantidadeTurmas), true);
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
            var textoPeriodo = "DE {0} ATÉ {1}, PELO LINK:";
            paragrafo.InserirTexto(String.Format(textoPeriodo, proposta.DataInscricaoInicio.ToString("dd/MM/yyyy"), proposta.DataInscricaoFim.ToString("dd/MM/yyyy")));
            var link = "https://conectaformacao.sme.prefeitura.sp.gov.br/area-publica";
            paragrafo.InserirLink(link, link, mainPart);
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

        private void CarregarRegentes()
        {
            paragrafo.InserirTextoNegrito("REGENTES: ", true);

            foreach (var regente in proposta.Regentes)
                paragrafo.InserirTexto(regente.Nome.ToUpper(), true);
        }

        private void CarregarAreaPromotora()
        {
            paragrafo.InserirTextoNegrito("ÁREA PROMOTORA: ", true);
            paragrafo.InserirTexto(proposta.NomeAreaPromotora.ToUpper(), true);
            paragrafo.InserirTexto($"CURSO HOMOLOGADO SOB O NÚMERO ");
            paragrafo.InserirTextoNegrito(proposta.NumeroHomologacao.ToString());
        }
    }
}
