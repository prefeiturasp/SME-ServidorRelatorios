using MediatR;
using Org.BouncyCastle.Ocsp;
using SME.SR.Data.Models.Conecta;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.Conecta;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioPaginadoLaudaCompletaQueryHandler : IRequestHandler<ObterRelatorioPaginadoLaudaCompletaQuery, RelatorioPaginadoLaudaCompletaDto>
    {
        private const int TOTAL_CARACTER_LINHA = 98;
        private const int TOTAL_LINHAS = 60;
        private const int TOTAL_CARACTER_LINHA_PAG = 6600; 

        private int totalLinhaPaginaAtual = 0;
        private PropostaCompleta propostaCompleta;
        private RelatorioPaginadoLaudaCompletaDto relatorioPaginado;
        private RelatorioPaginaLaudaCompletaDto paginaAtual;

        public async Task<RelatorioPaginadoLaudaCompletaDto> Handle(ObterRelatorioPaginadoLaudaCompletaQuery request, CancellationToken cancellationToken)
        {
            this.propostaCompleta = request.PropostaCompleta;

            relatorioPaginado = new RelatorioPaginadoLaudaCompletaDto();

            CarreguePaginas();

            return relatorioPaginado;
        }

        private void CarreguePaginas()
        {
            CriarPagina(1);
            CriarCampos();
        }

        private void AdicionarPagina()
        {
            relatorioPaginado.AdicionarPagina(paginaAtual);
        }

        private void CriarPagina(int pagina)
        {
            paginaAtual = new RelatorioPaginaLaudaCompletaDto();
            paginaAtual.Pagina = pagina;
        }

        private void CriarCampos()
        {
            var funcoes = ObterFuncoesCampos();

            foreach (var funcao in funcoes)
            {
                var campo = funcao();

                if (!(campo is null))
                    CriarCampo(campo);
            }

            AdicionarPagina();
        }

        private void CriarCampo(RelatorioCampoLaudaCompletaDto campo)
        {
            var linhasCampo = ObterQuantidadeLinha(campo);

            if (RealizarQuebraDeTexto(linhasCampo))
                ExecutePaginacaoComQuebraDeTexto(campo, linhasCampo);
            else
                ExecutePaginacao(campo, linhasCampo);
        }

        private void ExecutePaginacao(RelatorioCampoLaudaCompletaDto campo, int linhasCampo)
        {
            if ((totalLinhaPaginaAtual + linhasCampo) > TOTAL_LINHAS)
            {
                totalLinhaPaginaAtual = linhasCampo;
                AdicionarPagina();
                CriarPagina(paginaAtual.Pagina++);
            }
            else
            {
                totalLinhaPaginaAtual += linhasCampo;
            }

            paginaAtual.AdicionarCampo(campo);
        }

        private void ExecutePaginacaoComQuebraDeTexto(RelatorioCampoLaudaCompletaDto campoAtual, int linhasCampo)
        {
            var campos = ObterListaCampoQuebraTexto(campoAtual, linhasCampo);

            foreach (var campo in campos)
                CriarCampo(campo);
        }

        private bool RealizarQuebraDeTexto(int linhasCampo)
        {
            var linhaRestantes = TOTAL_LINHAS - totalLinhaPaginaAtual;

            return linhaRestantes > 4 && linhaRestantes < linhasCampo;
        } 

        private List<Func<RelatorioCampoLaudaCompletaDto>> ObterFuncoesCampos()
        {
            return new List<Func<RelatorioCampoLaudaCompletaDto>>()
            {
                ObterCampoNumeroHomologacao,
                ObterCampoNumeroProposta,
                ObterCampoTipoFormacao,
                ObterCampoAreaPromotora,
                ObterCampoNome,
                ObterCampoModalidade,
                ObterCampoCargaHorariaTotal,
                ObterCampoCargaHorariaPresencial,
                ObterCampoCargaHorariaNaoPresencial,
                ObterCampoCargaHorariaDistancia,
                ObterCampoJustificativa,
                ObterCampoObjetivos,
                ObterCampoConteudoProgramatico,
                ObterCampoProcedimentos,
                ObterCampoAtividade,
                ObterCampoCronograma,
                ObterCampoCriterioCertificacao,
                ObterCampoBiografia,
                ObterCampoQtdeTurmas,
                ObterCampoQtdeVagasPorTurmas,
                ObterCampoTotalVagas,
                ObterCampoPublicoAlvo,
                ObterCampoFuncao,
                ObterCampoVagasRemanecentes,
                ObterCampoCorpoDocente,
                ObterCampoInscricao,
                ObterCampoContatoAreResponsavel
            };
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoNumeroHomologacao()
        {
            return ObterCampo("NÚMERO DESPACHO DE HOMOLOGAÇÃO", propostaCompleta.NumeroHomologacao.ToString());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoNumeroProposta()
        {
            return ObterCampo("NÚMERO DA PROPOSTA DE VALIDAÇÃO", propostaCompleta.Id.ToString());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoTipoFormacao()
        {
            return ObterCampo("TIPO DE FORMAÇÃO", propostaCompleta.TipoFormacao.Name());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoAreaPromotora()
        {
            return ObterCampo("ÁREA PROMOTORA", propostaCompleta.NomeAreaPromotora, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoNome()
        {
            return ObterCampo("NOME", propostaCompleta.NomeFormacao, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoModalidade()
        {
            return ObterCampo("MODALIDADE", propostaCompleta.Modalidade.Name());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCargaHorariaTotal()
        {
            return ObterCampo("CARGA HORÁRIA TOTAL", propostaCompleta.TotalCargaHoraria);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCargaHorariaPresencial()
        {
            if (!string.IsNullOrEmpty(propostaCompleta.CargaHorariaPresencial))
                return ObterCampo("CARGA HORÁRIA PRESENCIAL", propostaCompleta.CargaHorariaPresencialFormatada);

            return null;
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCargaHorariaNaoPresencial()
        {
            if (!string.IsNullOrEmpty(propostaCompleta.CargaHorariaSincrona))
                return ObterCampo("CARGA HORÁRIA NÃO PRESENCIAL", propostaCompleta.CargaHorariaSincronaFormatada);

            return null;
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCargaHorariaDistancia()
        {
            if (!string.IsNullOrEmpty(propostaCompleta.CargaHorariaDistancia))
                return ObterCampo("CARGA HORÁRIA A DISTÂNCIA", propostaCompleta.CargaHorariaDistanciaFormatada);

            return null;
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoJustificativa()
        {
            return ObterCampo("JUSTIFICATIVA", propostaCompleta.Justificativa, true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoObjetivos()
        {
            return ObterCampo("OBJETIVOS", propostaCompleta.Objetivos, true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoConteudoProgramatico()
        {
            return ObterCampo("CONTEÚDO PROGRAMÁTICO", propostaCompleta.ConteudoProgramatico, true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoProcedimentos()
        {
            return ObterCampo("PROCEDIMENTOS", propostaCompleta.Procedimentos, true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoAtividade()
        {
            return ObterCampo("ATIVIDADE OBRIGATÓRIA", propostaCompleta.DescricaoAtividade, true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCronograma()
        {
            var descricao = new StringBuilder();

            descricao.AppendLine($"PERÍODO DE REALIZAÇÃO: {propostaCompleta.ObterPeriodoRealizacao()}");

            foreach (var encontro in propostaCompleta.Encontros)
                descricao.AppendLine($"<br>{encontro.ObterLocalDetalhado()}");

            return ObterCampo("CRONOGRAMA DETALHADO", descricao.ToString(), true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCriterioCertificacao()
        {   
            var criterios = propostaCompleta.CriteriosCertificacao is null ? 
                          string.Empty : 
                          String.Join(", ", propostaCompleta.CriteriosCertificacao.Select(p => p.Nome));
            
            return ObterCampo("CRITÉRIOS DE AVALIAÇÃO E APROVAÇÃO PARA EXPEDIÇÃO DE CERTIFICADO", criterios, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoBiografia()
        {
            return ObterCampo("BIBLIOGRAFIA", propostaCompleta.Referencias, true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoQtdeTurmas()
        {
            return ObterCampo("QUANTIDADE DE TURMAS", propostaCompleta.QuantidadeTurmas.ToString());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoQtdeVagasPorTurmas()
        {
            return ObterCampo("VAGAS POR TURMA", propostaCompleta.QuantidadeVagasTurmas.ToString());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoTotalVagas()
        {
            return ObterCampo("TOTAL DE VAGAS", propostaCompleta.TotalDeVagas.ToString());
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoPublicoAlvo()
        {
            var publicoAlvo = propostaCompleta.PublicosAlvo is null ? 
                                string.Empty : 
                                String.Join(", ", propostaCompleta.PublicosAlvo.Select(p => p.Nome));

            return ObterCampo("PÚBLICO ALVO", publicoAlvo, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoFuncao()
        {
            var funcoes = propostaCompleta.FuncaoEspecifica is null ?
                    string.Empty :
                    String.Join(", ", propostaCompleta.FuncaoEspecifica.Select(p => p.Nome));

            return ObterCampo("FUNÇÃO ESPECÍFICA", funcoes, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoVagasRemanecentes()
        {
            return ObterCampo("HAVENDO VAGAS REMANESCENTES, PODERÃO SER CONTEMPLADOS OS SEGUINTES CARGOS COMO PÚBLICO ALVO)", String.Join(", ", propostaCompleta.VagasRemanecentes).ToUpper(), true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoCorpoDocente()
        {
            var descricao = new StringBuilder();

            foreach (var regente in propostaCompleta.Regentes)
                descricao.AppendLine(regente.ObterDescricaoCompleta());

            return ObterCampo("CORPO DOCENTE", descricao.ToString(), true, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoInscricao()
        {
            var descricao = new StringBuilder();
            var textoPeriodo = "DE {0} ATÉ {1}";
            var textoLink = "<br> PELO LINK: {0}";
            var link = propostaCompleta.EhAreaPromotoraDireta ? "https://conectaformacao.sme.prefeitura.sp.gov.br/area-publica" : propostaCompleta.LinkInscricaoExterna;
            var criterios = propostaCompleta.CriteriosValidacao is null ?
                            string.Empty :
                            String.Join(", ", propostaCompleta.CriteriosValidacao.Select(p => p.Nome));

            descricao.AppendLine(String.Format(textoPeriodo, propostaCompleta.DataInscricaoInicio.ToString("dd/MM/yyyy"), propostaCompleta.DataInscricaoFim.ToString("dd/MM/yyyy")));
            descricao.AppendLine(String.Format(textoLink, link));
            descricao.AppendLine($"<br> {criterios}");

            return ObterCampo("INSCRIÇÕES (PROCEDIMENTOS E PERÍODO)", descricao.ToString(), true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoContatoAreResponsavel()
        {
            var telefones = propostaCompleta.TelefonesAreaPromotora is null ?
                        string.Empty :
                        String.Join(", ", propostaCompleta.TelefonesAreaPromotora.Select(p => p.Telefone));

            return ObterCampo("CONTATO COM A ÁREA RESPONSÁVEL", telefones, true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampo(string campo, string descricao, bool outraLinha = false, bool removerHtml = false, bool mostrarCampo = true)
        {
            descricao = removerHtml ? RemoveHTMLTags(descricao) : descricao;

            return new RelatorioCampoLaudaCompletaDto()
            {
                NomeCampo = campo,
                Descricao = string.IsNullOrEmpty(descricao) ? "-" : descricao.ToUpper(),
                OutraLinha = outraLinha,
                MostrarNome = mostrarCampo
            };
        }

        private int ObterQuantidadeLinha(RelatorioCampoLaudaCompletaDto campo)
        {
            var Qtdelinha = campo.OutraLinha ? 2 : 1;

            if (campo.Descricao.Length > TOTAL_CARACTER_LINHA)
                return Qtdelinha + (int)Math.Round((double)campo.Descricao.Length / TOTAL_CARACTER_LINHA);

            return Qtdelinha;
        }

        private static string RemoveHTMLTags(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            return Regex.Replace(texto, "<.*?>", string.Empty);
        }

        private List<RelatorioCampoLaudaCompletaDto> ObterListaCampoQuebraTexto(RelatorioCampoLaudaCompletaDto campo, int linhasCampo)
        {
            var campos = new List<RelatorioCampoLaudaCompletaDto>();
            var totalLinhasRestante = TOTAL_LINHAS - totalLinhaPaginaAtual - 2;
            var totalPagina = Math.Ceiling((decimal)linhasCampo / (decimal)TOTAL_LINHAS);
            int qtdeCaracteres = totalLinhasRestante > 3 ? (totalLinhasRestante * TOTAL_CARACTER_LINHA) : TOTAL_CARACTER_LINHA_PAG;
            var inicioCaracteres = 0;

            for (int pagina = 1; pagina <= totalPagina; pagina++)
            {
                var texto = ObterTextoComQuebra(campo.Descricao, inicioCaracteres, qtdeCaracteres);
                if (string.IsNullOrEmpty(texto))
                    break;
                var indiceUltimoEspaco = pagina == totalPagina ? -1 : texto.LastIndexOf(" ");
                texto = indiceUltimoEspaco > -1 ? texto.Substring(0, indiceUltimoEspaco) : texto;
                var campoNovo = ObterCampo(campo.NomeCampo, texto, campo.OutraLinha, false, pagina == 1);
                inicioCaracteres = indiceUltimoEspaco > -1 ? inicioCaracteres + indiceUltimoEspaco + 1 : inicioCaracteres + texto.Length;
                qtdeCaracteres = TOTAL_CARACTER_LINHA_PAG;
                campos.Add(campoNovo);
            }

            return campos;
        }

        private string ObterTextoComQuebra(string texto, int inicioCaracteres, int qtdeCaracteres)
        {
            if (texto.Length < inicioCaracteres)
                return string.Empty;

            var ultimoCarateres = texto.Length - inicioCaracteres;

            qtdeCaracteres = Math.Min(qtdeCaracteres, ultimoCarateres);

            if (texto.Length >= qtdeCaracteres)
                return texto.Substring(inicioCaracteres, qtdeCaracteres);

            return string.Empty;
        }
    }
}
