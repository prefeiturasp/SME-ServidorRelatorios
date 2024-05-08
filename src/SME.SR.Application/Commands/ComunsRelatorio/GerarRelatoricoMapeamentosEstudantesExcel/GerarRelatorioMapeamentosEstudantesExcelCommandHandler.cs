using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Data;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioMapeamentosEstudantesExcelCommandHandler : AsyncRequestHandler<GerarRelatorioMapeamentosEstudantesExcelCommand>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;
        private const int LINHA_NOME_SISTEMA = 2;
        private const int LINHA_NOME_RELATORIO = 3;
        private const int LINHA_CABECALHO_DRE_UE = 6;
        private const int LINHA_CABECALHO_TITULO = 8;
        private const string NOME_COLUNA_SEQUENCIA = "Seq";
        private const string PREFIXO_COLUNA_PROFICIENCIA_PSP = "PSPProficiencia_";
        private const string PREFIXO_COLUNA_NIVEL_PSP = "PSPNivel_";

        private const int LINHA_INICIO_REGISTROS = 10;

        private List<(string nmColuna, string titulo)> ColunasCabecalho = new List<(string nmColuna, string titulo)>()
        {
            (NOME_COLUNA_SEQUENCIA, ""),
            ("Turma", "Turma"),
            ("NomeEstudante", "Nome Estudante"),
            ("CodigoEstudante", "Código EOL"),
            ("ParecerConclusivoAnoAnterior", "Parecer conclusivo do ano anterior"),
            ("TurmaAnoAnterior", "Turma do ano anterior"),
            ("AnotacoesPedagogicasBimestreAnterior_Bim", "Anotações pedagógicas do bimestre anterior"),
            ("AnotacoesPedagogicasBimestreAnterior_DescBim", ""),
            ("DistorcaoIdadeAnoSerie", "Distorção idade/ano/série"),
            ("Nacionalidade", "Nacionalidade"),
            ("AcompanhadoSRMCEFAI", "Acompanhado pelo SRM/CEFAI"),
            ("PossuiPlanoAEE", "Possui plano AEE"),
            ("AcompanhadoNAAPA", "Acompanhado pelo NAAPA"),
            ("AcoesRedeApoio_Bim", "Ações da rede de apoio"),
            ("AcoesRedeApoio_DescBim", ""),
            ("AcoesRecuperacaoContinua_Bim", "Ações de recuperação contínua"),
            ("AcoesRecuperacaoContinua_DescBim", ""),
            ("ParticipaPAP", "Participa do PAP"),
            ("ParticipaProjetosMaisEducacao", "Participa de projetos do Mais Educação"),
            ("ProjetosFortalecimentosAprendizagens", "Projetos Fortalecimentos das Aprendizagens"),
            ("ProgramaSaoPauloIntegral", "Programa São Paulo Integral"),
            ("QualHipoteseEscritaEstudante_Bim", "Qual a hipótese de escrita do estudante"),
            ("QualHipoteseEscritaEstudante_DescBim", ""),
            ("ObsAvaliacaoProcessualEstudante_Bim", "Observações sobre a avaliação processual do estudante"),
            ("ObsAvaliacaoProcessualEstudante_DescBim", ""),
            ("Frequencia_Bim", "Frequência"),
            ("Frequencia_DescBim", ""),
            ("QdadeRegistrosBuscaAtiva_Bim", "Quantidade de registros de busca ativa"),
            ("QdadeRegistrosBuscaAtiva_DescBim", "")
        };

        public GerarRelatorioMapeamentosEstudantesExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioMapeamentosEstudantesExcelCommand request, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                var primeiroRegistro = request.MapeamentosEstudantes.FirstOrDefault();
                var areasConhecimentoProvaSP = request.MapeamentosEstudantes
                                                      .SelectMany(m => m.ObterAvaliacoesExternasProvaSP())
                                                      .Select(avaliacao => avaliacao.AreaConhecimento)
                                                      .Distinct()
                                                      .OrderBy(x => x);
                AdicionarColunasCabecalhoProvaSP(areasConhecimentoProvaSP.ToArray());

                var worksheet = workbook.Worksheets.Add($"{primeiroRegistro.UeCodigo}");
                MontarCabecalhoGeral(worksheet, primeiroRegistro);
                MontarCabecalhoTitulos(worksheet);
                
                var linhaFinal = LINHA_INICIO_REGISTROS;
                foreach (var dto in request.MapeamentosEstudantes
                                            .Select((mapeamento, sequencia) => (mapeamento, sequencia))
                                            .ToList())
                {
                    var qdadeLinhasInseridas = MontarLinhaRegistros(worksheet, linhaFinal, dto.mapeamento, dto.sequencia+1);
                    linhaFinal += qdadeLinhasInseridas;
                }

                AdicionarEstiloGeralTodasColunas(worksheet, LINHA_CABECALHO_TITULO, linhaFinal);
                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }
            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private DataTable ObterDataCabecalhoTitulos()
        {
            var data = new DataTable();
            ColunasCabecalho.ForEach(c =>
                data.Columns.Add(c.nmColuna)
            );

            DataRow titulos = data.NewRow();
            ColunasCabecalho.ForEach(c =>
                titulos[c.nmColuna] = c.titulo
            );
            data.Rows.Add(titulos);

            titulos = data.NewRow();
            ColunasCabecalho.ForEach(c =>
                titulos[c.nmColuna] = ""
            );
            data.Rows.Add(titulos);
            return data;
        }

        private DataTable ObterDataRegistros(MapeamentoEstudanteUltimoBimestreDto mapeamento, int seqRegistro)
        {
            var data = new DataTable();
            ColunasCabecalho.ForEach(c =>
                data.Columns.Add(c.nmColuna)
            );

            foreach (var mapeamentoBimestral in mapeamento.RespostrasBimestrais)
            {
                DataRow valores = data.NewRow();
                valores[NOME_COLUNA_SEQUENCIA] = seqRegistro;
                valores["Turma"] = mapeamento.TurmaCompleta;
                valores["NomeEstudante"] = mapeamento.AlunoNome;
                valores["CodigoEstudante"] = mapeamento.AlunoCodigo;
                valores["ParecerConclusivoAnoAnterior"] = string.Join(" | ", mapeamento.ObterParecerConclusivoAnoAnterior());
                valores["TurmaAnoAnterior"] = mapeamento.TurmaAnoAnterior;
                valores["AnotacoesPedagogicasBimestreAnterior_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["AnotacoesPedagogicasBimestreAnterior_DescBim"] = UtilRegex.RemoverTagsHtml(mapeamentoBimestral.AnotacoesPedagogicasBimestreAnterior_Bimestre);
                valores["DistorcaoIdadeAnoSerie"] = mapeamento.DistorcaoIdadeAnoSerie;
                valores["Nacionalidade"] = mapeamento.Nacionalidade;
                valores["AcompanhadoSRMCEFAI"] = mapeamento.AcompanhadoSRMCEFAI;
                valores["PossuiPlanoAEE"] = mapeamento.PossuiPlanoAEE;
                valores["AcompanhadoNAAPA"] = mapeamento.AcompanhadoNAAPA;
                valores["AcoesRedeApoio_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["AcoesRedeApoio_DescBim"] = UtilRegex.RemoverTagsHtml(mapeamentoBimestral.AcoesRedeApoio_Bimestre);
                valores["AcoesRecuperacaoContinua_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["AcoesRecuperacaoContinua_DescBim"] = UtilRegex.RemoverTagsHtml(mapeamentoBimestral.AcoesRecuperacaoContinua_Bimestre);
                valores["ParticipaPAP"] = string.Join(" | ", mapeamento.ObterProgramasPAP());
                valores["ParticipaProjetosMaisEducacao"] = string.Join(" | ", mapeamento.ObterProgramasMaisEducacao());
                valores["ProjetosFortalecimentosAprendizagens"] = string.Join(" | ", mapeamento.ObterProjetosFortalecimentoAprendizagem());
                valores["ProgramaSaoPauloIntegral"] = mapeamento.ProgramaSPIntegral;
                valores["QualHipoteseEscritaEstudante_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["QualHipoteseEscritaEstudante_DescBim"] = mapeamentoBimestral.HipoteseEscrita_Bimestre;
                valores["ObsAvaliacaoProcessualEstudante_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["ObsAvaliacaoProcessualEstudante_DescBim"] = UtilRegex.RemoverTagsHtml(mapeamentoBimestral.ObsAvaliacaoProcessual_Bimestre);
                valores["Frequencia_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["Frequencia_DescBim"] = mapeamentoBimestral.Frequencia_Bimestre;
                valores["QdadeRegistrosBuscaAtiva_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["QdadeRegistrosBuscaAtiva_DescBim"] = mapeamentoBimestral.QdadeRegistrosBuscasAtivas_Bimestre;
                foreach (var coluna in ColunasCabecalho.Where(col => col.nmColuna.Contains(PREFIXO_COLUNA_PROFICIENCIA_PSP) 
                                                                    || col.nmColuna.Contains(PREFIXO_COLUNA_NIVEL_PSP)))
                {
                    var provasSP = mapeamento.ObterAvaliacoesExternasProvaSP();
                    var avaliacaoAreaConhecimento = provasSP.FirstOrDefault(psp => psp.AreaConhecimento.Equals(coluna.nmColuna
                                                                                                                    .Replace(PREFIXO_COLUNA_PROFICIENCIA_PSP, "")
                                                                                                                    .Replace(PREFIXO_COLUNA_NIVEL_PSP, "")));
                    if (avaliacaoAreaConhecimento != null)
                        valores[coluna.nmColuna] = coluna.nmColuna.Contains(PREFIXO_COLUNA_PROFICIENCIA_PSP) 
                                                   ? avaliacaoAreaConhecimento.Proficiencia.ToString() 
                                                   : avaliacaoAreaConhecimento.Nivel;
                }
                data.Rows.Add(valores);
            }
            return data;
        }


        private void MontarCabecalhoTitulos(IXLWorksheet worksheet)
        {
            worksheet.Cell(LINHA_CABECALHO_TITULO, 1).InsertData(ObterDataCabecalhoTitulos());

            foreach (var coluna in ColunasCabecalho)
            {
                var indice = ColunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) + 1;
                worksheet.Range(LINHA_CABECALHO_TITULO, indice, LINHA_CABECALHO_TITULO + 1, indice).Merge();
            }

            foreach (var coluna in ColunasCabecalho.Where(col => col.nmColuna.Contains("_Bim")))
            {
                var indice = ColunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) +1;
                worksheet.Range(LINHA_CABECALHO_TITULO, indice, LINHA_CABECALHO_TITULO +1, indice+1).Merge();
            }

            AdicionarFundoCinzaClaro(worksheet.Range(LINHA_CABECALHO_TITULO, 2, LINHA_CABECALHO_TITULO, ColunasCabecalho.Count()));
            AdicionarAlinhamentoCentro(worksheet.Range(LINHA_CABECALHO_TITULO, 1, LINHA_CABECALHO_TITULO, ColunasCabecalho.Count()));
        }

        private void AdicionarEstiloRegistros(IXLWorksheet worksheet, int linhaInicial, int linhaFinal)
        {
            foreach (var coluna in ColunasCabecalho.Where(col => !col.nmColuna.Contains("_Bim") && !col.nmColuna.Contains("_DescBim")))
            {
                var indice = ColunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) + 1;
                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Merge();
                AdicionarAlinhamentoCentro(worksheet.Range(linhaInicial, indice, linhaFinal, indice));
            }
        }

        private int MontarLinhaRegistros(IXLWorksheet worksheet, int linhaFinal, MapeamentoEstudanteUltimoBimestreDto dto, int seqRegistro)
        {
            worksheet.Cell(linhaFinal, 1).InsertData(ObterDataRegistros(dto, seqRegistro));
            var qdadeLinhasInseridas = dto.RespostrasBimestrais.Count();
            AdicionarEstiloRegistros(worksheet, linhaFinal, linhaFinal + qdadeLinhasInseridas - 1);
            return qdadeLinhasInseridas;
        }


        private void MontarCabecalhoGeral(IXLWorksheet worksheet, MapeamentoEstudanteUltimoBimestreDto dto)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 1))
                .Scale(0.15);

            worksheet.Cell(LINHA_NOME_SISTEMA, 9).Value = "SGP - Sistema de Gestão Pedagógica";
            var linhaNomeSistema = worksheet.Range(LINHA_NOME_SISTEMA, 9, LINHA_NOME_SISTEMA, ColunasCabecalho.Count());
            linhaNomeSistema.Merge().Style.Font.Bold = true;
            AdicionarFonte(linhaNomeSistema);

            worksheet.Cell(LINHA_NOME_RELATORIO, 9).Value = "Relatório de Mapeamentos de Estudantes";
            AdicionarFonte(worksheet.Range(LINHA_NOME_RELATORIO, 9, LINHA_NOME_RELATORIO, ColunasCabecalho.Count()));

            worksheet.Range(LINHA_CABECALHO_DRE_UE, 1, LINHA_CABECALHO_DRE_UE, ColunasCabecalho.Count()).Merge();
            worksheet.Cell(LINHA_CABECALHO_DRE_UE, 1).Value = $"DRE: {dto.DreAbreviacao}                     Unidade Escolar (UE): {dto.UeCodigo} - {dto.UeCompleta}";
            AdicionarFonte(worksheet.Range(LINHA_CABECALHO_DRE_UE, 1, LINHA_CABECALHO_DRE_UE, ColunasCabecalho.Count()));
            AdicionarBorda(worksheet.Range(LINHA_CABECALHO_DRE_UE, 1, LINHA_CABECALHO_DRE_UE, ColunasCabecalho.Count()));          
        }

        private Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }

        private void AdicionarFonte(IXLRange range)
        {
            range.Style.Font.FontSize = 10;
            range.Style.Font.FontName = "Arial";
        }

        private void AdicionarAlinhamentoCentro(IXLRange range)
        {
           range.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
           range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }
        private void AdicionarQuebraAutoamticaTexto(IXLRange range)
        {
            range.Style.Alignment.WrapText = true;
        }

        private void AdicionarBorda(IXLRange range)
        {
            range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetOutsideBorderColor(XLColor.Black);
        }

        private void AdicionarFundoCinzaClaro(IXLRange range)
        {
            range.Style.Fill.SetBackgroundColor(XLColor.LightGray);
        }

        private void AdicionarEstiloGeralTodasColunas(IXLWorksheet worksheet, int linhaInicial, int linhaFinal)
        {
            worksheet.ShowGridLines = false;
            foreach (var coluna in ColunasCabecalho)
            {
                var indice = ColunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) + 1;
                for (int linha = linhaInicial; linha <= linhaFinal; linha++)
                {
                    var range = worksheet.Range(linha, indice, linha, indice);
                    if (coluna.nmColuna != NOME_COLUNA_SEQUENCIA 
                        || linha != linhaInicial)
                        AdicionarBorda(range);
                    AdicionarFonte(range);
                    AdicionarQuebraAutoamticaTexto(range);
                }
            }
            worksheet.Columns().AdjustToContents();
            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarColunasCabecalhoProvaSP(string[] areasConhecimento)
        {
            foreach (var area in areasConhecimento)
            {
                ColunasCabecalho.Add(($"{PREFIXO_COLUNA_PROFICIENCIA_PSP}{area}", $"PSP Proficiência {area}"));
                ColunasCabecalho.Add(($"{PREFIXO_COLUNA_NIVEL_PSP}{area}", $"PSP Nível {area}"));
            }
        }

    }
}
