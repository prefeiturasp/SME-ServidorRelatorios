﻿using MediatR;
using SME.SR.Data.Models.Conecta;
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
        private const int TOTAL_CARACTER_LINHA = 92;
        private const int TOTAL_LINHAS = 52;
        private const int TOTAL_CARACTER_LINHA_PAG = 5300;
        private const string RESPOSTA_OUTROS = "OUTROS";

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
                CriarPagina(paginaAtual.Pagina + 1);
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
            var criteriosCertificacao = propostaCompleta.CriteriosCertificacao.ToList();
            criteriosCertificacao.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = propostaCompleta.Criterios_Outros;
            });

            var criterios = criteriosCertificacao is null ? 
                          string.Empty : 
                          String.Join(", ", criteriosCertificacao.Select(p => $@"{p.Nome}{(string.IsNullOrEmpty(p.DescricaoAdicional)
                                                                                                                ? string.Empty : $": {p.DescricaoAdicional}")}"));
            
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
            var publicosAlvo = propostaCompleta.PublicosAlvo.ToList();
            publicosAlvo.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = propostaCompleta.PublicoAlvo_Outros;
            });
            return ObterCampo("PÚBLICO ALVO", ObterDescricao(publicosAlvo), true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoFuncao()
        {
            var funcoes = propostaCompleta.FuncaoEspecifica.ToList();
            funcoes.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = propostaCompleta.FuncaoEspecifica_Outros;
            });
            return ObterCampo("FUNÇÃO ESPECÍFICA", ObterDescricao(funcoes), true);
        }

        private RelatorioCampoLaudaCompletaDto ObterCampoVagasRemanecentes()
        {
            return ObterCampo("HAVENDO VAGAS REMANESCENTES, PODERÃO SER CONTEMPLADOS OS SEGUINTES CARGOS COMO PÚBLICO ALVO)", ObterDescricao(propostaCompleta.VagasRemanecentes), true);
        }

        private string ObterDescricao(IEnumerable<PropostaPublicoAlvo> publico)
        {
            return publico is null ? string.Empty : String.Join(", ", publico.Select(p => $@"{p.Nome.ToUpper()}{(string.IsNullOrEmpty(p.DescricaoAdicional)
                                                                                                                ? string.Empty : $": {p.DescricaoAdicional.ToUpper()}")}"));
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
            var criteriosValidacao = propostaCompleta.CriteriosValidacao.ToList();
            criteriosValidacao.ForEach(item =>
            {
                if (item.Nome.ToUpper().Equals(RESPOSTA_OUTROS))
                    item.DescricaoAdicional = propostaCompleta.CriteriosValidacao_Outros;
            });

            var descricao = new StringBuilder();
            var textoPeriodo = "DE {0} ATÉ {1}";
            var textoLink = "<br> PELO LINK: {0}";
            var link = propostaCompleta.EhAreaPromotoraDireta ? "https://conectaformacao.sme.prefeitura.sp.gov.br/area-publica" : propostaCompleta.LinkInscricaoExterna;
            var criterios = criteriosValidacao is null ?
                            string.Empty :
                            String.Join(", ", criteriosValidacao.Select(p => $@"{p.Nome}{(string.IsNullOrEmpty(p.DescricaoAdicional)
                                                                                                                ? string.Empty : $": {p.DescricaoAdicional}")}"));

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
            descricao = removerHtml ? UtilRegex.RemoverTagsHtml(descricao) : descricao;

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
            if (campo.Descricao.Length > TOTAL_CARACTER_LINHA)
                return (campo.MostrarNome ? 1 : 0) + (int)Math.Ceiling((double)campo.Descricao.Length / TOTAL_CARACTER_LINHA);

            return campo.OutraLinha ? 2 : 1;
        }

        private int ObterTotalPaginaQuebraTexto(int linhasCampo, int linhaRestante)
        {
            if (linhasCampo > TOTAL_LINHAS)
                return (int)Math.Ceiling((decimal)linhasCampo / (decimal)TOTAL_LINHAS);

            return linhasCampo > linhaRestante ? 2 : 1;
        }

        private List<RelatorioCampoLaudaCompletaDto> ObterListaCampoQuebraTexto(RelatorioCampoLaudaCompletaDto campo, int linhasCampo)
        {
            var campos = new List<RelatorioCampoLaudaCompletaDto>();
            var totalLinhasRestante = TOTAL_LINHAS - totalLinhaPaginaAtual - 1;
            var totalPagina = ObterTotalPaginaQuebraTexto(linhasCampo, totalLinhasRestante);
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
