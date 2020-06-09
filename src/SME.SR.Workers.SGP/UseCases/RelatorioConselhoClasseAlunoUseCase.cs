using MediatR;
using Newtonsoft.Json.Linq;
using SME.SR.Workers.SGP.Models;
using SME.SR.Workers.SGP.Queries;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.UseCases
{
    public class RelatorioConselhoClasseAlunoUseCase
    {
        public static async Task<string> Executar(IMediator mediator, JObject jObject)
        {
            try
            {
                jObject["fechamentoTurmaId"] = 54;
                jObject["conselhoClasseId"] = 21;
                jObject["codigoAluno"] = 4820277;

                long fechamentoTurmaId = (long)jObject["fechamentoTurmaId"];
                long conselhoClasseId = (long)jObject["conselhoClasseId"];
                string codigoAluno = jObject["codigoAluno"].ToString();

                var relatorio = await mediator.Send(new ObterRelatorioConselhoClasseAlunoQuery()
                {
                    CodigoAluno = codigoAluno,
                    ConselhoClasseId = conselhoClasseId,
                    FechamentoTurmaId = fechamentoTurmaId
                });

                string jsonString;

                if (relatorio is RelatorioConselhoClasseBimestre)
                    jsonString = JsonSerializer.Serialize((RelatorioConselhoClasseBimestre)relatorio);
                else
                    jsonString = JsonSerializer.Serialize((RelatorioConselhoClasseFinal)relatorio);

                return jsonString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
