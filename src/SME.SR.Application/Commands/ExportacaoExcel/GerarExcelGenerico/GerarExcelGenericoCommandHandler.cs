using ClosedXML.Excel;
using MediatR;
using Sentry;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarExcelGenericoCommandHandler : IRequestHandler<GerarExcelGenericoCommand, Unit>
    {
        private readonly IServicoFila servicoFila;

        public GerarExcelGenericoCommandHandler(IServicoFila servicoFila)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }
        private PropertyInfo[] ExtractClassPropertyNames(object item)
        {
            var metadataItem = item;
            var propertiesInfo = metadataItem.GetType().GetProperties();
            return propertiesInfo;
        }
        public async Task<Unit> Handle(GerarExcelGenericoCommand request, CancellationToken cancellationToken)
        {
            try
            {

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(request.NomeWorkSheet);

                    if (!request.ObjetoExportacao.Any())
                        throw new NegocioException("Não foi possível localizar o objeto de consulta.");

                    var properties = ExtractClassPropertyNames(request.ObjetoExportacao.FirstOrDefault());

                    MontarCabecalho(worksheet, properties);

                    MontarCorpo(request, worksheet);

                    if (request.PossuiNotaRodape)
                        MontarRodape(request, worksheet);

                    var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                    var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", request.CodigoCorrelacao.ToString());

                    if (request.RelatorioFrequenciaGlobal)
                        AdicionarZeroNaUeFrequênciaGlobal(workbook);

                    var planilha = workbook.Worksheets.First(w => w.Name == "Frequência Global");

                    workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
                }

                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));

                return await Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private static void AdicionarZeroNaUeFrequênciaGlobal(XLWorkbook workbook)
        {
            var planilha = workbook.Worksheets.First(w => w.Name == "Frequência Global");
            var totalLinhas = planilha.Rows().Count();
            for (int linha = 2; linha <= totalLinhas; linha++)
            {
                string novoValor = ("0" + planilha.Cell($"C{linha}").Value.ToString()).ToString();
                planilha.Cell($"C{linha}").SetValue(novoValor);
            }
        }

        private void MontarRodape(GerarExcelGenericoCommand request, IXLWorksheet worksheet)
        {
            var ultimaLinha = worksheet.LastRowUsed().RowNumber();
            worksheet.Row(ultimaLinha + 1).Cell(1).Value = request.NotaRodape;
            worksheet.Columns().AdjustToContents();
        }

        private void MontarCorpo(GerarExcelGenericoCommand request, IXLWorksheet worksheet)
        {
            var bindingFlags = System.Reflection.BindingFlags.Instance |
               System.Reflection.BindingFlags.NonPublic |
               System.Reflection.BindingFlags.Public;


            var iLinha = 2;
            foreach (var item in request.ObjetoExportacao)
            {
                List<object> listValues = item.GetType().GetFields(bindingFlags).Select(field => field.GetValue(item)).Where(value => value != null).ToList();
                var iPropriedade = 0;

                foreach (var item1 in listValues)
                {
                    var celulaNome = $"{Number2String(iPropriedade, true)}{iLinha}";
                    CorpoFormataStylo(worksheet, celulaNome);
                    worksheet.Cells(celulaNome).Value = item1.ToString();
                    iPropriedade++;
                }
                iLinha++;
            }
        }

        private void MontarCabecalho(IXLWorksheet worksheet, PropertyInfo[] properties)
        {
            for (var column = 0; column < properties.Length; column++)
            {
                var propriedadeRaiz = properties.GetValue(column);

                var colunaValor = ((MemberInfo)propriedadeRaiz).Name;

                var nomePropriedade = ((MemberInfo)propriedadeRaiz).CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == "DisplayAttribute");

                if (nomePropriedade != null)
                {
                    var propriedadeAtributoDescricao = nomePropriedade.NamedArguments.FirstOrDefault(a => a.MemberName == "Description");
                    if (nomePropriedade != null)
                        colunaValor = propriedadeAtributoDescricao.TypedValue.Value.ToString();
                }
                var colunaNome = $"{Number2String(column, true)}{1}";

                CabecalhoFormataStylo(worksheet, colunaNome);


                worksheet.Cells(colunaNome).Value = colunaValor;
            }
        }

        private static void CabecalhoFormataStylo(IXLWorksheet worksheet, string colunaNome)
        {
            worksheet.Cells(colunaNome).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(colunaNome).Style.Border.TopBorderColor = XLColor.Black;

            worksheet.Cells(colunaNome).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(colunaNome).Style.Border.RightBorderColor = XLColor.Black;

            worksheet.Cells(colunaNome).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(colunaNome).Style.Border.BottomBorderColor = XLColor.Black;

            worksheet.Cells(colunaNome).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(colunaNome).Style.Border.LeftBorderColor = XLColor.Black;

            worksheet.Cells(colunaNome).Style.Font.Bold = true;

            worksheet.Columns().AdjustToContents();

        }
        private static void CorpoFormataStylo(IXLWorksheet worksheet, string celulaNome)
        {
            worksheet.Cells(celulaNome).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(celulaNome).Style.Border.TopBorderColor = XLColor.Black;

            worksheet.Cells(celulaNome).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(celulaNome).Style.Border.RightBorderColor = XLColor.Black;

            worksheet.Cells(celulaNome).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(celulaNome).Style.Border.BottomBorderColor = XLColor.Black;

            worksheet.Cells(celulaNome).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Cells(celulaNome).Style.Border.LeftBorderColor = XLColor.Black;

        }

        private String Number2String(int number, bool isCaps)
        {

            Char c = (Char)((isCaps ? 65 : 97) + number);

            return c.ToString();

        }
    }
}
