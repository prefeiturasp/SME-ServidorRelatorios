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

                    var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                    var caminhoParaSalvar = Path.Combine(caminhoBase, $"relatorios", request.CodigoCorrelacao.ToString());

                    workbook.SaveAs($"{caminhoParaSalvar}.xlsx");
                }
                
                servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbit.FilaSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, request.CodigoCorrelacao));

                return await Task.FromResult(Unit.Value);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
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
                    worksheet.Cells($"{Number2String(iPropriedade, true)}{iLinha}").Value = item1.ToString();
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

                var colunaNome = ((MemberInfo)propriedadeRaiz).Name;

                var nomePropriedade = ((MemberInfo)propriedadeRaiz).CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == "DisplayAttribute");

                if (nomePropriedade != null)
                {
                    var propriedadeAtributoDescricao = nomePropriedade.NamedArguments.FirstOrDefault(a => a.MemberName == "Description");
                    if (nomePropriedade != null)
                        colunaNome = propriedadeAtributoDescricao.TypedValue.Value.ToString();
                }

                worksheet.Cells($"{Number2String(column, true)}{1}").Value = colunaNome;
            }
        }

        private String Number2String(int number, bool isCaps)
        {

            Char c = (Char)((isCaps ? 65 : 97) + number);

            return c.ToString();

        }
    }
}
