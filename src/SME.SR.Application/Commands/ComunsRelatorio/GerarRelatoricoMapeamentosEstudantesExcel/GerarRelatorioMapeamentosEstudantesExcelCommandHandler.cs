using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SME.SR.Data;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.FrequenciaMensal;
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

        private List<(string nmColuna, string titulo)> ColunasCabecalho = new List<(string nmColuna, string titulo)>()
        {
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
            ("QdadeRegistrosBuscaAtiva_DescBim", ""),
            ("PSPProficienciaCN", "PSP Proficiência CN"),
            ("PSPNivelCN", "PSP Nível CN"),
            ("PSPProficienciaCH", "PSP Proficiência CH"),
            ("PSPNivelCH", "PSP Nível CH"),
            ("PSPProficienciaLP", "PSP Proficiência LP"),
            ("PSPNivelLP", "PSP Nível LP"),
            ("PSPProficienciaMAT", "PSP Proficiência MAT"),
            ("PSPNivelMAT", "PSP Nível MAT"),
            ("PSPProficienciaRED", "PSP Proficiência RED"),
            ("PSPNivelRED", "PSP Nível RED"),
        };

        public GerarRelatorioMapeamentosEstudantesExcelCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioMapeamentosEstudantesExcelCommand request, CancellationToken cancellationToken)
        {
            /*var controlesFrequencias = await mediator.Send(new ObterControleFrequenciaMensalParaExcelQuery(request.ControlesFrequenciasMensais.ToList()));

            if (!controlesFrequencias.Any())
                throw new NegocioException("Não possui informações.");
            */
            using (var workbook = new XLWorkbook())
            {
                var primeiroRegistro = request.MapeamentosEstudantes.FirstOrDefault();
                var worksheet = workbook.Worksheets.Add($"{primeiroRegistro.UeCodigo}");
                MontarCabecalho(worksheet, primeiroRegistro, ColunasCabecalho.Count());
                AdicionarEstiloCabecalho(worksheet, ColunasCabecalho.Count(), LINHA_NOME_SISTEMA);
                AdicionarEstiloCabecalho(worksheet, ColunasCabecalho.Count(), LINHA_CABECALHO_DRE_UE);
                AdicionarEstiloCabecalho(worksheet, ColunasCabecalho.Count(), LINHA_NOME_RELATORIO);

                worksheet.Cell(LINHA_CABECALHO_TITULO, 1).InsertData(ObterDataCabecalhoTitulos());
                AdicionarEstiloCabecalhoTitulos(worksheet);
                var linhaFinal = LINHA_CABECALHO_TITULO + 1;

                foreach (var dto in request.MapeamentosEstudantes)
                {
                    worksheet.Cell(linhaFinal, 1).InsertData(ObterDataRegistros(dto));
                    var qdadeLinhasInseridas = dto.RespostrasBimestrais.Count();
                    AdicionarEstiloRegistros(worksheet, linhaFinal, linhaFinal + qdadeLinhasInseridas - 1);
                    linhaFinal += qdadeLinhasInseridas;
                    /*AdicionarColunaMes(worksheet, dtoExcelMes, linhaFinal, dtoExcelMes.TabelaDeDado.Columns.Count);
                    linhaFinal += 2;
                    worksheet.Cell(linhaFinal, 1).InsertData(dtoExcelMes.TabelaDeDado);
                    AdicionarEstilo(worksheet, dtoExcelMes, linhaFinal);
                    linhaFinal += dtoExcelMes.TabelaDeDado.Rows.Count;*/

                }


                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }

            /*
            using (var workbook = new XLWorkbook())
            {
                foreach (var dtoExcel in controlesFrequencias)
                {
                    var worksheet = workbook.Worksheets.Add(dtoExcel.CodigoCriancaEstudante);
                    var linhaFinal = LINHA_MES;

                    MontarCabecalho(worksheet, dtoExcel, 20);
                    AdicionarEstiloCabecalho(worksheet, dtoExcel.FrequenciasMeses.Max(data => data.TabelaDeDado.Columns.Count));

                    foreach (var dtoExcelMes in dtoExcel.FrequenciasMeses)
                    {
                        linhaFinal += 1;
                        AdicionarColunaMes(worksheet, dtoExcelMes, linhaFinal, dtoExcelMes.TabelaDeDado.Columns.Count);
                        linhaFinal += 2;
                        worksheet.Cell(linhaFinal, 1).InsertData(dtoExcelMes.TabelaDeDado);
                        AdicionarEstilo(worksheet, dtoExcelMes, linhaFinal);
                        linhaFinal += dtoExcelMes.TabelaDeDado.Rows.Count;
                    }
                }

                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", $"{request.CodigoCorrelacao}");

                workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
            }

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));*/
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
            return data;
        }

        private DataTable ObterDataRegistros(MapeamentoEstudanteUltimoBimestreDto mapeamento)
        {
            var data = new DataTable();
            ColunasCabecalho.ForEach(c =>
                data.Columns.Add(c.nmColuna)
            );

            foreach (var mapeamentoBimestral in mapeamento.RespostrasBimestrais)
            {
                DataRow valores = data.NewRow();
                valores["Turma"] = mapeamento.TurmaCompleta;
                valores["NomeEstudante"] = mapeamento.AlunoNome;
                valores["CodigoEstudante"] = mapeamento.AlunoCodigo;
                valores["ParecerConclusivoAnoAnterior"] = string.Join(" | ", mapeamento.ObterParecerConclusivoAnoAnterior());
                valores["TurmaAnoAnterior"] = mapeamento.TurmaAnoAnterior;
                valores["AnotacoesPedagogicasBimestreAnterior_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["AnotacoesPedagogicasBimestreAnterior_DescBim"] = mapeamentoBimestral.AnotacoesPedagogicasBimestreAnterior_Bimestre;
                valores["DistorcaoIdadeAnoSerie"] = mapeamento.DistorcaoIdadeAnoSerie;
                valores["Nacionalidade"] = mapeamento.Nacionalidade;
                valores["AcompanhadoSRMCEFAI"] = mapeamento.AcompanhadoSRMCEFAI;
                valores["PossuiPlanoAEE"] = mapeamento.PossuiPlanoAEE;
                valores["AcompanhadoNAAPA"] = mapeamento.AcompanhadoNAAPA;
                valores["AcoesRedeApoio_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["AcoesRedeApoio_DescBim"] = mapeamentoBimestral.AcoesRedeApoio_Bimestre;
                valores["AcoesRecuperacaoContinua_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["AcoesRecuperacaoContinua_DescBim"] = mapeamentoBimestral.AcoesRecuperacaoContinua_Bimestre;
                valores["ParticipaPAP"] = string.Join(" | ", mapeamento.ObterProgramasPAP());
                valores["ParticipaProjetosMaisEducacao"] = string.Join(" | ", mapeamento.ObterProgramasMaisEducacao());
                valores["ProjetosFortalecimentosAprendizagens"] = string.Join(" | ", mapeamento.ObterProjetosFortalecimentoAprendizagem());
                valores["ProgramaSaoPauloIntegral"] = mapeamento.ProgramaSPIntegral;
                valores["QualHipoteseEscritaEstudante_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["QualHipoteseEscritaEstudante_DescBim"] = mapeamentoBimestral.HipoteseEscrita_Bimestre;
                valores["ObsAvaliacaoProcessualEstudante_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["ObsAvaliacaoProcessualEstudante_DescBim"] = mapeamentoBimestral.ObsAvaliacaoProcessual_Bimestre;
                valores["Frequencia_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["Frequencia_DescBim"] = mapeamentoBimestral.Frequencia_Bimestre;
                valores["QdadeRegistrosBuscaAtiva_Bim"] = $"{mapeamentoBimestral.Bimestre}º Bim.";
                valores["QdadeRegistrosBuscaAtiva_DescBim"] = mapeamentoBimestral.QdadeRegistrosBuscasAtivas_Bimestre;
                valores["PSPProficienciaCN"] = "";
                valores["PSPNivelCN"] = "";
                valores["PSPProficienciaCH"] = "";
                valores["PSPNivelCH"] = "";
                valores["PSPProficienciaLP"] = "";
                valores["PSPNivelLP"] = "";
                valores["PSPProficienciaMAT"] = "";
                valores["PSPNivelMAT"] = "";
                valores["PSPProficienciaRED"] = "";
                valores["PSPNivelRED"] = "";
                data.Rows.Add(valores);
            }
            return data;
        }

        private void AdicionarLinhas(IEnumerable<ControleFrequenciaPorComponenteDto> frequenciasPorComponente, DataTable data, int mes)
        {
            var linha = data.NewRow();

            foreach (var frequenciaComponente in frequenciasPorComponente)
            {
                linha[$"{mes}_Componentes"] = frequenciaComponente.NomeComponente;
                linha[$"{mes}_FrequenciaDoPeriodo"] = frequenciaComponente.FrequenciaDoPeriodo;

                foreach (var FrequenciaTipo in frequenciaComponente.FrequenciaPorTipo)
                {
                    linha[$"{mes}_TipoFrequencia"] = FrequenciaTipo.TipoFrequencia;
                    linha[$"{mes}_TotalDoPeriodo"] = FrequenciaTipo.TotalDoPeriodo;

                    foreach (var FrequenciaAula in FrequenciaTipo.FrequenciaPorAula)
                    {
                        linha[$"{mes}_{FrequenciaAula.DiaSemanaNumero}"] = FrequenciaAula.Valor;
                    }

                    data.Rows.Add(linha);
                    linha = data.NewRow();
                }
            }
        }

        private void AdicionarEstiloCabecalhoTitulos(IXLWorksheet worksheet)
        {
            foreach(var coluna in ColunasCabecalho.Where(col => col.nmColuna.Contains("_Bim") || col.nmColuna.Contains("_DescBim")))
            {
                var indice = ColunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) +1;
                worksheet.Range(LINHA_CABECALHO_TITULO, indice, LINHA_CABECALHO_TITULO, indice+1).Merge();
            }
            worksheet.Columns().AdjustToContents();
            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloRegistros(IXLWorksheet worksheet, int linhaInicial, int linhaFinal)
        {
            foreach (var coluna in ColunasCabecalho.Where(col => !col.nmColuna.Contains("_Bim") && !col.nmColuna.Contains("_DescBim")))
            {
                var indice = ColunasCabecalho.IndexOf((coluna.nmColuna, coluna.titulo)) + 1;
                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Merge();

               /* worksheet.Range(linhaInicial, indice, linhaFinal, indice).Style.Font.SetFontName("Arial");
                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Style.Border.SetOutsideBorderColor(XLColor.Black);
                worksheet.Range(linhaInicial, indice, linhaFinal, indice).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);*/
            }

            
        }

        private void AdicionarEstiloCabecalho(IXLWorksheet worksheet, int ultimaColunaUsada, int linha)
        {
            var range = worksheet.Range(linha, 1, linha, ultimaColunaUsada);
            range.Style.Font.SetFontSize(10);
            range.Style.Font.SetFontName("Arial");
        }


        private void AdicionarTituloColunas(Dictionary<int, string> diasSemanas, DataTable data, int mes)
        {
            DataRow linhaNumeroDia = data.NewRow();
            DataRow linhaSiglaDia = data.NewRow();

            foreach (var dias in diasSemanas.OrderBy(dia => dia.Key))
            {
                if (string.IsNullOrEmpty(dias.Value))
                    continue;
                linhaNumeroDia[$"{mes}_{dias.Key}"] = dias.Key;
                linhaSiglaDia[$"{mes}_{dias.Key}"] = dias.Value;
            }

            linhaSiglaDia[$"{mes}_TotalDoPeriodo"] = "Total";
            linhaSiglaDia[$"{mes}_FrequenciaDoPeriodo"] = "Frequência do Período";

            data.Rows.Add(linhaSiglaDia);
            data.Rows.Add(linhaNumeroDia);
        }

        private void AdicionarColunaMes(IXLWorksheet worksheet, FrequenciaPorMesExcelDto dto, int linha, int totalColuna)
        {
            worksheet.Cell(linha, 1).Value = $"Mês: {dto.Mes}";
            var rangeMes = worksheet.Range(linha, 1, linha, 1 + totalColuna);
            rangeMes.Merge().Style.Font.Bold = true;
            AdicinarFonte(rangeMes);
            var linhaFrequencia = linha + 1;
            worksheet.Cell(linhaFrequencia, 1).Value = $"Frequência global do mês: {dto.FrequenciaGlobal}";
            var rangeFreq = worksheet.Range(linhaFrequencia, 1, linhaFrequencia, 1 + totalColuna);
            rangeFreq.Merge().Style.Font.Bold = true;
            AdicinarFonte(rangeFreq);
        }

        private void MontarCabecalho(IXLWorksheet worksheet, MapeamentoEstudanteUltimoBimestreDto dto, int totalColunas)
        {
            worksheet.AddPicture(ObterLogo())
                .MoveTo(worksheet.Cell(2, 1))
                .Scale(0.15);

            worksheet.Cell(LINHA_NOME_SISTEMA, 9).Value = "SGP - Sistema de Gestão Pedagógica";
            var linhaNomeSistema = worksheet.Range(LINHA_NOME_SISTEMA, 9, LINHA_NOME_SISTEMA, totalColunas);
            linhaNomeSistema.Merge().Style.Font.Bold = true;
            AdicinarFonte(linhaNomeSistema);

            worksheet.Cell(LINHA_NOME_RELATORIO, 9).Value = "Relatório de Mapeamentos de Estudantes";
            AdicinarFonte(worksheet.Range(LINHA_NOME_RELATORIO, 9, LINHA_NOME_RELATORIO, totalColunas));

            worksheet.Cell(LINHA_CABECALHO_DRE_UE, 1).Value = $"DRE: {dto.DreAbreviacao}    Unidade Escolar (UE): {dto.UeCompleta}";
            AdicinarFonte(worksheet.Range(LINHA_CABECALHO_DRE_UE, 1, LINHA_CABECALHO_DRE_UE, totalColunas));
        }

        private Stream ObterLogo()
        {
            string base64Logo = SmeConstants.LogoSmeMono.Substring(SmeConstants.LogoSmeMono.IndexOf(',') + 1);
            return new MemoryStream(Convert.FromBase64String(base64Logo));
        }

        private void AdicinarFonte(IXLRange range)
        {
            range.Merge().Style.Font.FontSize = 10;
            range.Style.Font.FontName = "Arial";
        }

        private void AdicionarEstilo(IXLWorksheet worksheet, FrequenciaPorMesExcelDto dtoExcelMes, int linha)
        {
            int ultimaColunaUsada = worksheet.LastColumnUsed().ColumnNumber();
            int ultimaLinhaUsada = worksheet.LastRowUsed().RowNumber();

            AdicionarEstiloCabecalhoLinha(worksheet, ultimaColunaUsada, linha);
            AdicionarEstiloCorpo(worksheet, ultimaLinhaUsada, dtoExcelMes.TabelaDeDado.Rows.Count, linha, ultimaColunaUsada);
            AdicionarEstiloColunaDia(worksheet, dtoExcelMes.ColunasDiasNaoLetivosFinaisSemana, linha, ultimaLinhaUsada);

            worksheet.ShowGridLines = false;
            worksheet.Columns().AdjustToContents();
            worksheet.ColumnsUsed().AdjustToContents();
            worksheet.RowsUsed().AdjustToContents();
        }

        private void AdicionarEstiloColunaDia(IXLWorksheet worksheet, IEnumerable<int> colunasDiasSemAula, int linha, int ultimaLinha)
        {
            foreach(var indiceColuna in colunasDiasSemAula)
            {
                worksheet.Range(linha, indiceColuna, ultimaLinha, indiceColuna).Style.Fill.BackgroundColor = XLColor.LightGray;
            }
        }

        private void AdicionarEstiloCorpo(IXLWorksheet worksheet, int ultimaLinhaUsada, int totalRegistro, int linha, int ultimaColunaMes)
        {
            var linhaTabela = linha;
            var linhaSubCabecalho = linhaTabela + 1;

            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Font.SetFontName("Arial");
            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetOutsideBorderColor(XLColor.Black);

            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(linhaTabela, 1, ultimaLinhaUsada, ultimaColunaMes).Style.Border.SetInsideBorderColor(XLColor.Black);

            worksheet.Range(linhaTabela, 1, linhaTabela, ultimaColunaMes).Style.Font.SetFontSize(10);
            worksheet.Range(linhaTabela, 1, linhaTabela, ultimaColunaMes).Style.Font.Bold = true;
            worksheet.Range(linhaTabela, 1, linhaTabela, ultimaColunaMes).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheet.Range(linhaSubCabecalho, 1, linhaSubCabecalho, ultimaColunaMes).Style.Font.SetFontSize(10);
            worksheet.Range(linhaSubCabecalho, 1, linhaSubCabecalho, ultimaColunaMes).Style.Font.Bold = true;
            worksheet.Range(linhaSubCabecalho, 1, linhaSubCabecalho, ultimaColunaMes).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheet.Range(linhaTabela, 2, ultimaLinhaUsada, ultimaColunaMes).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Range(linhaTabela, 1, linhaTabela + 1, 1).Merge();
            worksheet.Range(linhaTabela, ultimaColunaMes - 1, linhaTabela + 1, ultimaColunaMes - 1).Merge();
            worksheet.Range(linhaTabela, ultimaColunaMes, linhaTabela + 1, ultimaColunaMes).Merge();

            Merge(worksheet, linhaTabela, 1, totalRegistro);
            Merge(worksheet, linhaTabela, ultimaColunaMes, totalRegistro);
        }

        private void Merge(IXLWorksheet worksheet, int linha, int coluna, int totalRegistro)
        {
            var linhaInicio = linha + 2;
            var linhaFinal = 0;

            totalRegistro = linhaInicio + (totalRegistro - 3);

            while (linhaInicio <= totalRegistro)
            {
                linhaFinal = linhaInicio + 2;
                worksheet.Range(linhaInicio, coluna, linhaFinal, coluna).Merge();
                linhaInicio = linhaFinal + 1;
            }
        }

        private void AdicionarEstiloCabecalhoLinha(IXLWorksheet worksheet, int ultimaColunaUsada, int linha)
        {
            var range = worksheet.Range(linha, 1, linha, ultimaColunaUsada);
            range.Style.Font.SetFontSize(10);
            range.Style.Font.SetFontName("Arial");
        }
    }
}
