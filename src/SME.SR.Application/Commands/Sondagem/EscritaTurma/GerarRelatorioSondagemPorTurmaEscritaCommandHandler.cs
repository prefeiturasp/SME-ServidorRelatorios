using ClosedXML.Excel;
using MediatR;
using Sentry;
using SME.SR.Application.Queries.ConsultaSondagemPorTurma;
using SME.SR.Application.Queries.Dre.ObterDreUeNomePorUeCodigo;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.Sondagem.EscritaTurma
{
    public class GerarRelatorioSondagemPorTurmaEscritaCommandHandler : GerarRelatorioSondagemPorTurmaBase, IRequestHandler<GerarRelatorioSondagemPorTurmaEscritaCommand, Unit>
    {
        private readonly IMediator mediator;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioSondagemPorTurmaEscritaCommandHandler(IMediator mediator, IServicoFila servicoFila)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<Unit> Handle(GerarRelatorioSondagemPorTurmaEscritaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dreUeNome = await ObterNomeUeDre(request.UeCodigo);
                var turmaNome = await ObterTurma(request.TurmaId.ToString());
                var proficiencia = (ProficienciaSondagemEnum)request.ProficienciaId;

                var displayNameProficiencia = proficiencia
                    .GetType()
                    .GetMember(proficiencia.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()
                    ?.GetName();

                var exibirBimestre = request.BimestreId is null;
                var usuarioSolicitacao = await mediator.Send(new ObterUsuarioPorCodigoRfQuery(request.UsuarioLogadoRF));
                var dto = (await mediator.Send(new ConsultaSondagemPorTurmaQuery(request.TurmaId, request.ProficienciaId, request.ComponenteCurricularId, (int)request.Modalidade, request.Ano, request.AnoLetivo, request.Semestre, request.UeCodigo, request.BimestreId)))
                            .MapToEscritaEfTurmaSondagemCabecalhoExcelDto(
                                    request.AnoLetivo,
                                    turmaNome,
                                    dreUeNome.UeNome,
                                    dreUeNome.DreNome,
                                    request.Modalidade.Name(),
                                    usuarioSolicitacao.NomeRelatorio,
                                    displayNameProficiencia
                                );

                switch (request.Modalidade)
                {
                    case Modalidade.EJA:
                    case Modalidade.Fundamental:
                        GerarExcelEF(dto, request, request.CodigoCorrelacao, request.Modalidade, displayNameProficiencia);
                        await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(string.Empty, "Relatório da Sondagem"), RotasRabbitSGP.RotaRelatoriosProntosSgp, RotasRabbitSR.RotaRelatoriosSolicitadosSondagemQuestionario, request.CodigoCorrelacao));
                        break;
                }
                return await Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        static XLColor ConverterCor(string cor)
        {
            if (string.IsNullOrEmpty(cor))
                return XLColor.White;

            var hex = cor.TrimStart('#');
            var r = Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return XLColor.FromArgb(r, g, b);
        }

        private void GerarExcelEF(EscritaEfTurmaSondagemCabecalhoExcelDto dto, GerarRelatorioSondagemPorTurmaEscritaCommand request, Guid codigoCorrelacao, Modalidade modalidade, string displayNameProficiencia)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Sondagem");

            
            sheet.Column(1).Width = 6;   // Nº
            sheet.Column(2).Width = 28;  // Nome
            sheet.Column(3).Width = 12;  // Raça
            sheet.Column(4).Width = 12;  // Gênero
            sheet.Column(5).Width = 10;  // LP como 2ª língua?
            sheet.Column(6).Width = 12;  // Sondagem inicial
            sheet.Column(7).Width = 10;  // 1º bim
            sheet.Column(8).Width = 10;  // 2º bim
            if (modalidade == Modalidade.Fundamental) 
            {
                sheet.Column(9).Width = 10;  // 3º bim
                sheet.Column(10).Width = 10; // 4º bim
            }
            int linha = 1;

            
            sheet.Row(1).Height = 20;
            sheet.Row(2).Height = 20;
            sheet.Row(3).Height = 20;

            var logo = sheet.AddPicture(ObterLogo())
                             .MoveTo(sheet.Cell(1, 1))
                             .WithSize(160, 60);


            linha = 4;
            EscreverCelula(sheet, linha, 1, $"Ano letivo: {request.AnoLetivo}   DRE: {dto.Dre}   Semestre: {dto.Semestre}", bold: false);
            sheet.Range(linha, 1, linha, 7).Merge();
            EscreverCelula(sheet, linha, 8, $"Turma: {dto.Turma}", bold: false);
            sheet.Range(linha, 8, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            EscreverCelula(sheet, linha, 1, $"Unidade Educacional: {dto.Ue}", bold: false);
            sheet.Range(linha, 1, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            EscreverCelula(sheet, linha, 1, $"Modalidade: {dto.Modalidade}", bold: false); 
            sheet.Range(linha, 1, linha, 3).Merge();
            EscreverCelula(sheet, linha, 4, $"Proficiência: {dto.Proeficiencia}", bold: false); 
            sheet.Range(linha, 4, linha, 6).Merge();
            EscreverCelula(sheet, linha, 7, $"Data de impressão: {dto.DataImpressao}", bold: false);
            sheet.Range(linha, 7, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            
            EscreverCelula(sheet, linha, 1, $"Usuário: {dto.NomeUsuarioSolicitacao}", bold: false); 
            sheet.Range(linha, 1, linha, 10).Merge();
            AplicarBordaExterna(sheet.Range(linha, 1, linha, 10));
            linha++;

            linha++; 

            var tituloCell = sheet.Cell(linha, 1);
            tituloCell.Value = "Relatório da Sondagem";
            tituloCell.Style.Font.Bold = true;
            tituloCell.Style.Font.FontSize = 14;
            tituloCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Range(linha, 1, linha, 10).Merge();
            linha++;

            var subtituloCell = sheet.Cell(linha, 1);
            subtituloCell.Value = displayNameProficiencia;
            subtituloCell.Style.Font.Bold = true;
            subtituloCell.Style.Font.FontSize = 12;
            subtituloCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Range(linha, 1, linha, 10).Merge();
            linha++;

            linha++;

            if (modalidade == Modalidade.EJA)
            {
                var grupoEscrita = sheet.Range(linha, 6, linha, 7); 
                grupoEscrita.Merge();
                grupoEscrita.Value = displayNameProficiencia;
                grupoEscrita.Style.Font.Bold = true;
                grupoEscrita.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                grupoEscrita.Style.Fill.BackgroundColor = XLColor.LightGray;
                AplicarBordaExterna(grupoEscrita);
            }
            else
            {
                var grupoEscrita = sheet.Range(linha, 6, linha, 10);
                grupoEscrita.Merge();
                grupoEscrita.Value = displayNameProficiencia;
                grupoEscrita.Style.Font.Bold = true;
                grupoEscrita.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                grupoEscrita.Style.Fill.BackgroundColor = XLColor.LightGray;
                AplicarBordaExterna(grupoEscrita);
            }
            linha++;

            
            var headers = modalidade == Modalidade.EJA ? new[]
            {
                (1, "Nº"),
                (2, "Nome"),
                (3, "Raça"),
                (4, "Gênero"),
                (5, "LP como 2ª língua?"),
                (6, "1º bim"),
                (7, "2º bim"),
            } : new[]
            {
                (1, "Nº"),
                (2, "Nome"),
                (3, "Raça"),
                (4, "Gênero"),
                (5, "LP como 2ª língua?"),
                (6, "Sondagem inicial"),
                (7, "1º bim"),
                (8, "2º bim"),
                (9, "3º bim"),
                (10, "4º bim"),
            } ;

            foreach (var (col, texto) in headers)
            {
                var cell = sheet.Cell(linha, col);
                cell.Value = texto;
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            sheet.Row(linha).Height = 40;
            linha++;


            foreach (var item in dto.CorpoRelatorio)
            {
                sheet.Row(linha).Height = 45;
                var corFundo = ConverterCor(item.Cor);

                var cNum = sheet.Cell(linha, 1);
                cNum.Value = item.Numero;
                EstilarCelulaDados(cNum);


                var cNome = sheet.Cell(linha, 2);
                cNome.Value = item.Nome;
                EstilarCelulaDados(cNome);


                var cRaca = sheet.Cell(linha, 3);
                cRaca.Value = item.Raca;
                EstilarCelulaDados(cRaca);


                var cGenero = sheet.Cell(linha, 4);
                cGenero.Value = item.Genero;
                EstilarCelulaDados(cGenero);


                var cLp = sheet.Cell(linha, 5);
                cLp.Value = item.LpComoLinguaPrincipal == "Sim" ? "☑" : "☐";
                cLp.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cLp.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cLp.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cLp.Style.Font.FontSize = 14;


                if (modalidade == Modalidade.EJA)
                {
                    preenchererCelulaSondagem(sheet.Cell(linha, 6), item.PrimeiroBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 7), item.SegundoBimestre, corFundo);
                }

                if (modalidade == Modalidade.Fundamental) {
                    preenchererCelulaSondagem(sheet.Cell(linha, 6), item.SondagemInicial, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 7), item.PrimeiroBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 8), item.SegundoBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 9), item.TerceiroBimestre, corFundo);
                    preenchererCelulaSondagem(sheet.Cell(linha, 10), item.QuartoBimestre, corFundo);
                }

                linha++;
            }

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", codigoCorrelacao.ToString());
            workbook.SaveAs($"{caminhoParaSalvar}.xlsx");

        }

        private async Task<string> ObterTurma(string codigoTurma)
        {
            if (codigoTurma == "-99")
                return "Todos";

            var turma = await mediator.Send(new ObterTurmaQuery(codigoTurma));
            return turma.NomeRelatorio;
        }

        private async Task<DreUeNome> ObterNomeUeDre(string ueCodigo)
        {
            return await mediator.Send(new ObterDreUeNomePorUeCodigoQuery(ueCodigo));
        }
        static void EscreverCelula(IXLWorksheet ws, int row, int col, string valor, bool bold = false)
        {
            var cell = ws.Cell(row, col);
            cell.Value = valor;
            cell.Style.Font.Bold = bold;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }

        static void AplicarBordaExterna(IXLRange range)
        {
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        static void EstilarCelulaDados(IXLCell cell)
        {
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        static void preenchererCelulaSondagem(IXLCell cell, string valor, XLColor corFundo)
        {
            cell.Value = valor;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Font.Bold = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Fill.BackgroundColor = corFundo;


            if (valor == "SSVC" || valor == "Sem preencher" || valor == "Vazio" || valor == "")
                cell.Style.Font.FontColor = XLColor.Black;
            else
                cell.Style.Font.FontColor = XLColor.White;
        }
    }


}
