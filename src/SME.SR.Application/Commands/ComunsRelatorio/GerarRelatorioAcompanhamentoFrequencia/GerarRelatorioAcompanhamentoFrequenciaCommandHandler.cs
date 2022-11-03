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

            var limiteCaracteres = 5200;
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

                relatorio.ehTodosBimestre = ehTodosOsBimestres;

                qtdeCaracteresPagina = qtdeCaracteresPorLinha * 5;

                var lstBimestresAluno = new List<RelatorioFrequenciaIndividualBimestresDto>();
                
                aluno.DescricaoUltimoBimestre = alunoDto.Bimestres.LastOrDefault().NomeBimestre;
                
                foreach (var bimestreDto in alunoDto.Bimestres)
                {
                    var bimestreAluno = MapearBimestre(bimestreDto);

                    var qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + (qtdeCaracteresPorLinha * 3);

                    if (qtdeCaracteresPaginaProposta > limiteCaracteres)
                    {
                        aluno.Bimestres = lstBimestresAluno;
                        AdicionarAluno(relatorio, alunoDto, aluno);
                        paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));
                        paginaAluno++;
                        relatorio.Alunos.FirstOrDefault().NomeAluno = string.Empty;
                        relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                        lstBimestresAluno = new List<RelatorioFrequenciaIndividualBimestresDto>();
                        qtdeCaracteresPagina = qtdeCaracteresPorLinha * 4; //cabeçalho padrão + linha em branco
                    }

                    qtdeCaracteresPagina += qtdeCaracteresPorLinha * 3; //linhas do bimestre
                    
                    var quantidadelinhasCabecalho = request.Relatorio.ImprimirFrequenciaDiaria && bimestreDto.FrequenciaDiaria.Any() ? 2 : 0;
                    qtdeCaracteresPagina += qtdeCaracteresPorLinha * quantidadelinhasCabecalho; //linhas da frequência diária

                    var possuiFrequenciaDiariaParaAdicionar = true;
                    var lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();
                    foreach (var frequenciaDiariaDto in bimestreDto.FrequenciaDiaria)
                    {
                        var frequenciaDiaria = new RelatorioFrequenciaIndividualJustificativasDto();
                        
                        var justificativaAusencia = frequenciaDiariaDto.Justificativa;
                        var tamanhoJustificativaAusencia = justificativaAusencia.Length;

                        if (tamanhoJustificativaAusencia > maximoCaracteresPorJustificativa)
                        {
                            justificativaAusencia = justificativaAusencia.Substring(0, maximoCaracteresPorJustificativa);
                            justificativaAusencia += "...";
                            frequenciaDiariaDto.Justificativa = justificativaAusencia;
                            tamanhoJustificativaAusencia = justificativaAusencia.Length;
                        }

                        if (tamanhoJustificativaAusencia > 0)
                        {
                            tamanhoJustificativaAusencia += 80 * 7;
                            qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + tamanhoJustificativaAusencia;
                        }
                        else
                            qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + qtdeCaracteresPorLinha;

                        if (qtdeCaracteresPaginaProposta > limiteCaracteres) 
                        {
                            var qtdeCaracteresPermitidosPagina = (limiteCaracteres - qtdeCaracteresPagina);
                            var qtdeCaracteresPermitidosJustificativa = justificativaAusencia.Length > qtdeCaracteresPermitidosPagina 
                                ? qtdeCaracteresPermitidosPagina : justificativaAusencia.Length;
                            
                            var ausenciaNaPaginaAtualAux = string.IsNullOrEmpty(justificativaAusencia)
                                ? justificativaAusencia
                                : justificativaAusencia.Substring(0,qtdeCaracteresPermitidosJustificativa);
                            
                            var ausenciaNaPaginaAtual = string.IsNullOrEmpty(ausenciaNaPaginaAtualAux) ? ausenciaNaPaginaAtualAux : ausenciaNaPaginaAtualAux.Substring(0, Math.Min(ausenciaNaPaginaAtualAux.Length, ausenciaNaPaginaAtualAux.LastIndexOf(" ")));
                            var ausenciaRemanescente = string.IsNullOrEmpty(justificativaAusencia) ? justificativaAusencia : justificativaAusencia.Substring(ausenciaNaPaginaAtual.Length);

                            MapearFrequenciaDiaria(frequenciaDiariaDto, frequenciaDiaria, ausenciaNaPaginaAtual);
                            lstJustificativasAusencias.Add(frequenciaDiaria);

                            //Encerrando as listas para gerar página
                            bimestreAluno.FrequenciaDiaria.AddRange(lstJustificativasAusencias);
                            lstBimestresAluno.Add(bimestreAluno);
                            aluno.Bimestres = lstBimestresAluno;

                            AdicionarAluno(relatorio, alunoDto, aluno);

                            paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));
                            paginaAluno++;

                            relatorio.Alunos.FirstOrDefault().Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
                            relatorio.Alunos.FirstOrDefault().NomeAluno = string.Empty;
                            bimestreAluno = new RelatorioFrequenciaIndividualBimestresDto();
                            lstBimestresAluno = new List<RelatorioFrequenciaIndividualBimestresDto>();
                            lstJustificativasAusencias = new List<RelatorioFrequenciaIndividualJustificativasDto>();
                            qtdeCaracteresPagina  = qtdeCaracteresPorLinha * 4; //Cabeçalho padrão + linha em branco
                            
                            if (!string.IsNullOrEmpty(ausenciaRemanescente))
                            {
                                MapearFrequenciaDiaria(frequenciaDiariaDto, frequenciaDiaria, ausenciaRemanescente);
                                lstJustificativasAusencias.Add(frequenciaDiaria);
                                qtdeCaracteresPagina += (int)Math.Round(decimal.Parse(ausenciaRemanescente.Length.ToString()) / qtdeCaracteresPorLinha,MidpointRounding.ToEven);
                            }
                            
                            qtdeCaracteresPagina  += qtdeCaracteresPorLinha * 2; //Cabeçalho da frequência diária                           
                        }
                        else
                        {
                            MapearFrequenciaDiaria(frequenciaDiariaDto, frequenciaDiaria);
                            lstJustificativasAusencias.Add(frequenciaDiaria);
                            possuiFrequenciaDiariaParaAdicionar = true;
                            qtdeCaracteresPagina += qtdeCaracteresPorLinha;
                        }
                    }
                    if (possuiFrequenciaDiariaParaAdicionar)
                    {
                        if (lstJustificativasAusencias.Any())
                            bimestreAluno.FrequenciaDiaria.AddRange(lstJustificativasAusencias);
                        
                            lstBimestresAluno.Add(bimestreAluno);
                    }
                }
                if (lstBimestresAluno.Any())
                {
                    aluno.Bimestres = lstBimestresAluno;

                    if (!relatorio.Alunos.Any(a => a.CodigoAluno.Equals(alunoDto.CodigoAluno)))
                        relatorio.Alunos.Add(aluno);
                    else
                        relatorio.Alunos[0] = aluno;

                    aluno.Bimestres.LastOrDefault().ExibirFinal = ehTodosOsBimestres;
                    paginasAluno.Add(await GerarPagina(paginasAluno, relatorio, qtdeAlunos, paginaAluno));
                }

                var ultimaPagina = paginasAluno.LastOrDefault().Pagina;
                
                paginasAluno.ForEach(f=> f.Total = ultimaPagina);

                paginas.AddRange(paginasAluno);
            }
            
            PdfGenerator pdfGenerator = new PdfGenerator(converter);

            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginas, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório de Registro Individual");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));

        }

        private static void AdicionarAluno(RelatorioFrequenciaIndividualDto relatorio,
            RelatorioFrequenciaIndividualAlunosDto alunoDto, RelatorioFrequenciaIndividualAlunosDto aluno)
        {
            if (!relatorio.Alunos.Any(a => a.CodigoAluno.Equals(alunoDto.CodigoAluno)))
                relatorio.Alunos.Add(aluno);
            else
                relatorio.Alunos[0] = aluno;
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
