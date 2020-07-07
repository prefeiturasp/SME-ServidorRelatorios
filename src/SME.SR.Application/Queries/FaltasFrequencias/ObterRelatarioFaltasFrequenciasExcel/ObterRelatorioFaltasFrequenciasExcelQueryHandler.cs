using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasExcelQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciasExcelQuery, IEnumerable<RelatorioFaltasFrequenciasExcelDto>>
    {


        public ObterRelatorioFaltasFrequenciasExcelQueryHandler()
        {

        }

        public async Task<IEnumerable<RelatorioFaltasFrequenciasExcelDto>> Handle(ObterRelatorioFaltasFrequenciasExcelQuery request, CancellationToken cancellationToken)
        {
            var listaMockada = new List<RelatorioFaltasFrequenciasExcelDto>();

            for (int i = 0; i < 100; i++)
            {
                listaMockada.Add(new RelatorioFaltasFrequenciasExcelDto()
                {
                    Ano = 2020,
                    AulasQuantidade = i,
                    AusenciaPercentual = i,
                    Bimestre = i,
                    Disciplina = "Disciplina de teste",
                    DreNome = "Dre nome de teste",
                    EstudanteCodigo = i.ToString(),
                    FaltasQuantidade = i,
                    Turma = "A",
                    UnidadeEscolarNome = "UE Nome"
                });

            }

            return await Task.FromResult(listaMockada);
        }


    }
}
