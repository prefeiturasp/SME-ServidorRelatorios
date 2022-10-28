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

            var limiteCaracteres = 4500;
            var qtdeCaracteresPorLinha = 110;
            var qtdeCaracteresPagina = 0;
            var qtdeAlunos = request.Relatorio.Alunos.Count();
            var paginaAluno = 0;
            var ehTodosOsBimestres = false;
            var maximoCaracteresPorJustificativa = 500;

            foreach (var alunoDto in request.Relatorio.Alunos)
            {
                var paginasAluno = new List<PaginaParaRelatorioPaginacaoSoloDto>();

                paginaAluno = 1;

                ehTodosOsBimestres = request.Relatorio.ehTodosBimestre;

                var relatorio = MapearRelatorio(request);
                var aluno = MapearAluno(alunoDto);

                qtdeCaracteresPagina = qtdeCaracteresPorLinha * (request.Relatorio.ImprimirFrequenciaDiaria ? 10 : 5);

                var lstBimestresAluno = new List<RelatorioFrequenciaIndividualBimestresDto>();
                foreach (var bimestreDto in alunoDto.Bimestres)
                {
                    var bimestreAluno = MapearBimestre(bimestreDto);

                    var qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + (qtdeCaracteresPorLinha * 3);

                    if (qtdeCaracteresPaginaProposta > limiteCaracteres)
                    {
                        paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));
                        paginaAluno++;
                        relatorio.Alunos.FirstOrDefault().NomeAluno = string.Empty;
                        relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                    }

                    var quantidadelinhasCabecalho = request.Relatorio.ImprimirFrequenciaDiaria && bimestreDto.FrequenciaDiaria.Any() ? 5 : 3;
                    qtdeCaracteresPagina += qtdeCaracteresPorLinha * quantidadelinhasCabecalho;

                    var possuiJustificativaParaAdicionar = true;
                    var lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();
                    foreach (var frequenciaDiariaDto in bimestreDto.FrequenciaDiaria)
                    {
                        var frequenciaDiaria = new RelatorioFrequenciaIndividualJustificativasDto();

                        var motivoAusencia = frequenciaDiariaDto.Justificativa;
                        var tamanhoMotivoAusencia = motivoAusencia.Length;

                        if (request.Relatorio.ImprimirFrequenciaDiaria && tamanhoMotivoAusencia > maximoCaracteresPorJustificativa)
                        {
                            motivoAusencia = motivoAusencia.Substring(0, maximoCaracteresPorJustificativa);
                            motivoAusencia += "...";
                            frequenciaDiariaDto.Justificativa = motivoAusencia;
                            tamanhoMotivoAusencia = motivoAusencia.Length;
                        }

                        if (tamanhoMotivoAusencia > 0)
                            tamanhoMotivoAusencia += qtdeCaracteresPorLinha * 2;

                        qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + (tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia);

                        if (qtdeCaracteresPaginaProposta > limiteCaracteres) 
                        {
                            var qtdeCaracteresPermitidos = (limiteCaracteres - qtdeCaracteresPagina);
                            var ausenciaNaPaginaAtualAux = string.IsNullOrEmpty(motivoAusencia) ? motivoAusencia : motivoAusencia.Substring(0, qtdeCaracteresPermitidos);
                            var ausenciaNaPaginaAtual = string.IsNullOrEmpty(ausenciaNaPaginaAtualAux) ? ausenciaNaPaginaAtualAux : ausenciaNaPaginaAtualAux.Substring(0, Math.Min(ausenciaNaPaginaAtualAux.Length, ausenciaNaPaginaAtualAux.LastIndexOf(" ")));
                            qtdeCaracteresPermitidos = ausenciaNaPaginaAtual.Length;
                            var ausenciaRemanescente = string.IsNullOrEmpty(motivoAusencia) ? motivoAusencia : motivoAusencia.Substring(qtdeCaracteresPermitidos);
                            var numeroPaginasAusencia = (ausenciaRemanescente.Length / limiteCaracteres) + 1;

                            MapearFrequenciaDiaria(frequenciaDiariaDto, frequenciaDiaria, ausenciaNaPaginaAtual);
                            lstJustificativasAusencias.Add(frequenciaDiaria);
                            tamanhoMotivoAusencia = ausenciaNaPaginaAtual.Length;
                            qtdeCaracteresPagina += tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia;

                            //Encerrando as listas para gerar página
                            bimestreAluno.FrequenciaDiaria.AddRange(lstJustificativasAusencias);
                            lstBimestresAluno.Add(bimestreAluno);
                            aluno.Bimestres = lstBimestresAluno;

                            if (!relatorio.Alunos.Any(a => a.CodigoAluno.Equals(alunoDto.CodigoAluno)))
                                relatorio.Alunos.Add(aluno);
                            else
                                relatorio.Alunos[0] = aluno;

                            paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));
                            paginaAluno++;
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
                                    var ausenciaAux = string.IsNullOrEmpty(ausenciaRemanescente) ? ausenciaRemanescente : ausenciaRemanescente.Substring(0, (qtdeCaracteresPermitidos));
                                    ausenciaNaPaginaAtual = string.IsNullOrEmpty(ausenciaAux) ? ausenciaAux : ausenciaAux.Substring(0, Math.Min(ausenciaAux.Length, ausenciaAux.LastIndexOf(" ")));
                                    qtdeCaracteresPermitidos = ausenciaNaPaginaAtual.Length;
                                    ausenciaRemanescente = string.IsNullOrEmpty(ausenciaRemanescente) ? ausenciaRemanescente : ausenciaRemanescente.Substring(qtdeCaracteresPermitidos);
                                    gerarPagina = true;
                                }

                                var justificativaPagina = new RelatorioFrequenciaIndividualJustificativasDto();
                                MapearFrequenciaDiaria(frequenciaDiariaDto, justificativaPagina, ausenciaNaPaginaAtual);

                                lstJustificativasAusencias.Add(justificativaPagina);

                                relatorio.Alunos.FirstOrDefault().Bimestres.Add(new RelatorioFrequenciaIndividualBimestresDto() { FrequenciaDiaria = lstJustificativasAusencias });

                                if (gerarPagina)
                                {
                                    paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));
                                    paginaAluno++;
                                    relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                                    lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();
                                    possuiJustificativaParaAdicionar = false;
                                    gerarPagina = false;
                                    qtdeCaracteresPagina = qtdeCaracteresPorLinha;
                                }
                                else
                                {
                                    possuiJustificativaParaAdicionar = true;
                                    tamanhoMotivoAusencia = lstJustificativasAusencias.LastOrDefault().Justificativa.Length;
                                    qtdeCaracteresPagina += tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia;
                                }
                                    
                            }
                        }
                        else
                        {
                            MapearFrequenciaDiaria(frequenciaDiariaDto, frequenciaDiaria);
                            lstJustificativasAusencias.Add(frequenciaDiaria);
                            possuiJustificativaParaAdicionar = true;
                            tamanhoMotivoAusencia = frequenciaDiariaDto.Justificativa.Length;
                            qtdeCaracteresPagina += tamanhoMotivoAusencia == 0 ? qtdeCaracteresPorLinha : tamanhoMotivoAusencia;
                        }
                    }
                    if (possuiJustificativaParaAdicionar)
                    {
                        if (lstJustificativasAusencias.Any())
                            bimestreAluno.FrequenciaDiaria.AddRange(lstJustificativasAusencias);

                        lstBimestresAluno.Add(bimestreAluno);
                    }
                }
                aluno.Bimestres = lstBimestresAluno;

                if (!relatorio.Alunos.Any(a => a.CodigoAluno.Equals(alunoDto.CodigoAluno)))
                    relatorio.Alunos.Add(aluno);
                else
                    relatorio.Alunos[0] = aluno;

                relatorio.ehTodosBimestre = ehTodosOsBimestres;

                paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));

                var ultimaPagina = paginasAluno.LastOrDefault().Pagina;
                
                paginasAluno.ForEach(f=> f.Total = ultimaPagina);

                paginas.AddRange(paginasAluno);
            }
            
            PdfGenerator pdfGenerator = new PdfGenerator(converter);

            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginas, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório de Registro Individual");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private void MapearFrequenciaDiaria(                        
                        RelatorioFrequenciaIndividualJustificativasDto frequenciaDiariaOrigem,
                        RelatorioFrequenciaIndividualJustificativasDto frequenciaDiariaDestino,
                        string justificativa = "")
        {
            var descJustificativa = frequenciaDiariaOrigem.Justificativa ?? "";

            frequenciaDiariaDestino.DataAula = frequenciaDiariaOrigem.DataAula;
            frequenciaDiariaDestino.Justificativa = string.IsNullOrEmpty(justificativa) ? descJustificativa : justificativa;
            frequenciaDiariaDestino.QuantidadePresenca = frequenciaDiariaOrigem.QuantidadePresenca;
            frequenciaDiariaDestino.QuantidadeAusencia = frequenciaDiariaOrigem.QuantidadeAusencia;
            frequenciaDiariaDestino.QuantidadeAulas = frequenciaDiariaOrigem.QuantidadeAulas;
            frequenciaDiariaDestino.QuantidadeRemoto = frequenciaDiariaOrigem.QuantidadeRemoto;
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
                TurmaNome = request.Relatorio.TurmaNome,
                ComponenteNome = request.Relatorio.ComponenteNome,
                ImprimirFrequenciaDiaria = request.Relatorio.ImprimirFrequenciaDiaria
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
