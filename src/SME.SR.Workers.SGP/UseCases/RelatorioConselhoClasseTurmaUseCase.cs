using MediatR;
using Newtonsoft.Json.Linq;
using SME.SR.Workers.SGP.Models;
using SME.SR.Workers.SGP.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.UseCases
{
    public class RelatorioConselhoClasseTurmaUseCase
    {
        public static async Task<string> Executar(IMediator mediator, JObject jObject)
        {
            try
            {
                long fechamentoTurmaId = (long)jObject["fechamentoTurmaId"];
                long conselhoClasseId = (long)jObject["conselhoClasseId"];
                string codigoTurma = jObject["codigoTurma"].ToString();

                var alunos = await ObterAlunosTurma(codigoTurma.ToString(), mediator);

                var lstRelatorioAlunos = new List<RelatorioConselhoClasseBase>();
                string codigoAluno;

                foreach(var aluno in alunos)
                {
                    codigoAluno = aluno.CodigoAluno.ToString();

                    lstRelatorioAlunos.Add(await ObterRelatorioConselhoClasseAluno(conselhoClasseId, fechamentoTurmaId, 
                                                                                   codigoAluno, mediator));
                }

                string jsonString;

                if (lstRelatorioAlunos.FirstOrDefault() is RelatorioConselhoClasseBimestre)
                {
                    List<RelatorioConselhoClasseBimestre> listBimestre = lstRelatorioAlunos.Cast<RelatorioConselhoClasseBimestre>().ToList();
                    jsonString = JsonSerializer.Serialize(listBimestre);
                }
                else
                {
                    List<RelatorioConselhoClasseFinal> listFinal = lstRelatorioAlunos.Cast<RelatorioConselhoClasseFinal>().ToList();
                    jsonString = JsonSerializer.Serialize(listFinal);
                }

                return jsonString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<IEnumerable<Aluno>> ObterAlunosTurma(string codigoTurma, IMediator mediator)
        {
            return await mediator.Send(new ObterAlunosPorTurmaQuery()
            {
                CodigoTurma = codigoTurma
            });
        }

        private static async Task<RelatorioConselhoClasseBase> ObterRelatorioConselhoClasseAluno(long conselhoClasseId, 
                                                                                                long fechamentoTurmaId,
                                                                                                string codigoAluno, 
                                                                                                IMediator mediator)
        {
           return await mediator.Send(new ObterRelatorioConselhoClasseAlunoQuery()
            {
                CodigoAluno = codigoAluno,
                ConselhoClasseId = conselhoClasseId,
                FechamentoTurmaId = fechamentoTurmaId
            });
        }
    }
}
