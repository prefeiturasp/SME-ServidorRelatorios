using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAcompanhamentoFrequenciaCommandHandler : AsyncRequestHandler<GerarRelatorioAcompanhamentoFrequenciaCommand>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioAcompanhamentoFrequenciaCommandHandler(IConverter converter,
                                                       IHtmlHelper htmlHelper,
                                                       IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }


        protected override async Task Handle(GerarRelatorioAcompanhamentoFrequenciaCommand request, CancellationToken cancellationToken)
        {
            var paginas = new List<PaginaParaRelatorioPaginacaoSoloDto>();

            var limiteCaracteres = 7300;
            var qtdeCaracteresPorLinha = 140;
            var qtdeCaracteresPagina = 0;
            var qtdeAlunos = request.Relatorio.Alunos.Count();
            var paginaAluno = 0;
            var ehTodosOsBimestres = false;

            foreach (var alunoDto in request.Relatorio.Alunos)
            {
                paginaAluno++;

                ehTodosOsBimestres = request.Relatorio.ehTodosBimestre;

                var relatorio = MapearRelatorio(request);
                var aluno = MapearAluno(alunoDto);

                qtdeCaracteresPagina = qtdeCaracteresPorLinha * 4;

                var lstBimestresAluno = new List<RelatorioFrequenciaIndividualBimestresDto>();
                foreach (var bimestreDto in alunoDto.Bimestres)
                {
                    var bimestreAluno = MapearBimestre(bimestreDto);

                    var qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + (qtdeCaracteresPorLinha * 3);

                    if (qtdeCaracteresPaginaProposta > limiteCaracteres)
                    {
                        paginas.Add(await GerarPagina(paginas, relatorio, qtdeAlunos, paginaAluno));
                        relatorio.Alunos.FirstOrDefault().NomeAluno = string.Empty;
                        relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                    }

                    qtdeCaracteresPagina += qtdeCaracteresPorLinha * 3;

                    var possuiJustificativaParaAdicionar = true;
                    var lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();
                    foreach (var justificativaDto in bimestreDto.Justificativas)
                    {
                        var justificativaAusencia = new RelatorioFrequenciaIndividualJustificativasDto();

                        var motivoAusencia = justificativaDto.MotivoAusencia;
                        var tamanhoMotivoAusencia = motivoAusencia.Length;
                        qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + (tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia);

                        if (qtdeCaracteresPaginaProposta > limiteCaracteres) 
                        {
                            var qtdeCaracteresPermitidos = (limiteCaracteres - qtdeCaracteresPagina);
                            var ausenciaNaPaginaAtualAux = motivoAusencia.Substring(0, qtdeCaracteresPermitidos);
                            var ausenciaNaPaginaAtual = ausenciaNaPaginaAtualAux.Substring(0, Math.Min(ausenciaNaPaginaAtualAux.Length, ausenciaNaPaginaAtualAux.LastIndexOf(" ")));
                            qtdeCaracteresPermitidos = ausenciaNaPaginaAtual.Length;
                            var ausenciaRemanescente = motivoAusencia.Substring(qtdeCaracteresPermitidos);
                            var numeroPaginasAusencia = (ausenciaRemanescente.Length / limiteCaracteres) + 1;

                            justificativaAusencia.DataAusencia = justificativaDto.DataAusencia;
                            justificativaAusencia.MotivoAusencia = ausenciaNaPaginaAtual;
                            lstJustificativasAusencias.Add(justificativaAusencia);
                            tamanhoMotivoAusencia = ausenciaNaPaginaAtual.Length;
                            qtdeCaracteresPagina += tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia;

                            //Encerrando as listas para gerar página
                            bimestreAluno.Justificativas.AddRange(lstJustificativasAusencias);
                            lstBimestresAluno.Add(bimestreAluno);
                            aluno.Bimestres = lstBimestresAluno;

                            if (!relatorio.Alunos.Any(a => a.CodigoAluno.Equals(alunoDto.CodigoAluno)))
                                relatorio.Alunos.Add(aluno);
                            else
                                relatorio.Alunos[0] = aluno;

                            paginas.Add(await GerarPagina(paginas, relatorio, qtdeAlunos, paginaAluno));
                            qtdeCaracteresPagina = qtdeCaracteresPorLinha;

                            relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                            relatorio.Alunos.FirstOrDefault().NomeAluno = string.Empty;
                            bimestreAluno = new RelatorioFrequenciaIndividualBimestresDto();
                            lstBimestresAluno = new List<RelatorioFrequenciaIndividualBimestresDto>();
                            lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();

                            var gerarPagina = false;
                            for (int noPagina = 1; noPagina <= numeroPaginasAusencia; noPagina++)
                            {
                                qtdeCaracteresPermitidos = Math.Min(ausenciaRemanescente.Length, limiteCaracteres);

                                ausenciaNaPaginaAtual = ausenciaRemanescente;

                                if (ausenciaRemanescente.Length >= limiteCaracteres)
                                {
                                    var ausenciaAux = ausenciaRemanescente.Substring(0, (qtdeCaracteresPermitidos));
                                    ausenciaNaPaginaAtual = ausenciaAux.Substring(0, Math.Min(ausenciaAux.Length, ausenciaAux.LastIndexOf(" ")));
                                    qtdeCaracteresPermitidos = ausenciaNaPaginaAtual.Length;
                                    ausenciaRemanescente = ausenciaRemanescente.Substring(qtdeCaracteresPermitidos);
                                    gerarPagina = true;
                                }      

                                lstJustificativasAusencias.Add(new RelatorioFrequenciaIndividualJustificativasDto
                                {
                                    DataAusencia = justificativaDto.DataAusencia,
                                    MotivoAusencia = ausenciaNaPaginaAtual
                                });

                                relatorio.Alunos.FirstOrDefault().Bimestres.Add(new RelatorioFrequenciaIndividualBimestresDto() { Justificativas = lstJustificativasAusencias });

                                if (gerarPagina)
                                {
                                    paginas.Add(await GerarPagina(paginas, relatorio, qtdeAlunos, paginaAluno));
                                    relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                                    lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();
                                    possuiJustificativaParaAdicionar = false;
                                    gerarPagina = false;
                                    qtdeCaracteresPagina = qtdeCaracteresPorLinha;
                                }
                                else
                                {
                                    possuiJustificativaParaAdicionar = true;
                                    tamanhoMotivoAusencia = lstJustificativasAusencias.LastOrDefault().MotivoAusencia.Length;
                                    qtdeCaracteresPagina += tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia;
                                }
                                    
                            }
                        }
                        else
                        {
                            justificativaAusencia.DataAusencia = justificativaDto.DataAusencia;
                            justificativaAusencia.MotivoAusencia = justificativaDto.MotivoAusencia;
                            lstJustificativasAusencias.Add(justificativaAusencia);
                            possuiJustificativaParaAdicionar = true;
                            tamanhoMotivoAusencia = justificativaDto.MotivoAusencia.Length;
                            qtdeCaracteresPagina += tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia;
                        }
                    }
                    if (possuiJustificativaParaAdicionar)
                    {
                        if (lstJustificativasAusencias.Any())
                            bimestreAluno.Justificativas.AddRange(lstJustificativasAusencias);

                        lstBimestresAluno.Add(bimestreAluno);
                    }
                }
                aluno.Bimestres = lstBimestresAluno;

                if (!relatorio.Alunos.Any(a => a.CodigoAluno.Equals(alunoDto.CodigoAluno)))
                    relatorio.Alunos.Add(aluno);
                else
                    relatorio.Alunos[0] = aluno;

                relatorio.ehTodosBimestre = ehTodosOsBimestres;

                paginas.Add(await GerarPagina(paginas, relatorio, qtdeAlunos, paginaAluno));
            }

            PdfGenerator pdfGenerator = new PdfGenerator(converter);

            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginas, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório de Registro Individual");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private RelatorioFrequenciaIndividualDto MapearRelatorio(GerarRelatorioAcompanhamentoFrequenciaCommand request)
        {
            return new RelatorioFrequenciaIndividualDto
            {
                DreNome = request.Relatorio.DreNome,
                UeNome = request.Relatorio.UeNome,
                Usuario = request.Relatorio.Usuario,
                RF = request.Relatorio.RF,
                ehInfantil = request.Relatorio.ehInfantil,
            };
        }

        private RelatorioFrequenciaIndividualAlunosDto MapearAluno(RelatorioFrequenciaIndividualAlunosDto alunoDto)
        {
            return new RelatorioFrequenciaIndividualAlunosDto
            {
                NomeAluno = alunoDto.NomeAluno,
                CodigoAluno = alunoDto.CodigoAluno,
                TituloFinal = alunoDto.TituloFinal,
                TotalAulasDadasFinal = alunoDto.TotalAulasDadasFinal,
                TotalPresencasFinal = alunoDto.TotalPresencasFinal,
                TotalRemotoFinal = alunoDto.TotalRemotoFinal,
                TotalAusenciasFinal = alunoDto.TotalAusenciasFinal,
                TotalCompensacoesFinal = alunoDto.TotalCompensacoesFinal,
                PercentualFrequenciaFinal = alunoDto.PercentualFrequenciaFinal,
            };
        }

        private RelatorioFrequenciaIndividualBimestresDto MapearBimestre(RelatorioFrequenciaIndividualBimestresDto bimestreDto)
        {
            return  new RelatorioFrequenciaIndividualBimestresDto
            {
                NomeBimestre = bimestreDto.NomeBimestre,
                DadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto
                {
                    TotalAulasDadas = bimestreDto.DadosFrequencia.TotalAulasDadas,
                    TotalPresencas = bimestreDto.DadosFrequencia.TotalPresencas,
                    TotalRemoto = bimestreDto.DadosFrequencia.TotalRemoto,
                    TotalAusencias = bimestreDto.DadosFrequencia.TotalAusencias,
                    TotalCompensacoes = bimestreDto.DadosFrequencia.TotalCompensacoes,
                    TotalPercentualFrequencia = bimestreDto.DadosFrequencia.TotalPercentualFrequencia,
                    TotalPercentualFrequenciaFormatado = bimestreDto.DadosFrequencia.TotalPercentualFrequenciaFormatado
                }
            };
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, RelatorioFrequenciaIndividualDto relatorio, int qtdeAlunos, int numeroPaginaAluno)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioFrequenciaIndividualNovo", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, numeroPaginaAluno, qtdeAlunos);
        }          
    }
}
