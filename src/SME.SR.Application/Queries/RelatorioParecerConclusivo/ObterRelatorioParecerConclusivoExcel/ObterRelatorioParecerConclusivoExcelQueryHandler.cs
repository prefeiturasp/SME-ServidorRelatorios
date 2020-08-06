using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoExcelQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoExcelQuery, IEnumerable<RelatorioParecerConclusivoExcelDto>>
    {
        public async Task<IEnumerable<RelatorioParecerConclusivoExcelDto>> Handle(ObterRelatorioParecerConclusivoExcelQuery request, CancellationToken cancellationToken)
        {
            var listaPareceresConclusivos = new List<RelatorioParecerConclusivoExcelDto>();

            foreach (var dre in request.RelatorioParecerConclusivo.Dres)
            {
                foreach (var ue in dre.Ues)
                {
                    foreach (var ciclo in ue.Ciclos)
                    {
                        foreach (var ano in ciclo.Anos)
                        {
                            foreach (var parecer in ano.PareceresConclusivos)
                            {
                                listaPareceresConclusivos.Add(new RelatorioParecerConclusivoExcelDto()
                                {
                                    NomeDre = dre.Nome,
                                    NomeUe = ue.Nome,
                                    Ciclo = ciclo.Nome,
                                    Ano = ano.Nome,
                                    Turma = parecer.TurmaNome,
                                    CodigoAluno = parecer.AlunoCodigo,
                                    NomeAluno = parecer.AlunoNomeCompleto,
                                    ParecerConclusivo = parecer.ParecerConclusivoDescricao
                                });
                            }
                        }
                    }
                }
            }
            return await Task.FromResult(listaPareceresConclusivos.OrderBy( a => a.NomeDre).ThenBy(a => a.NomeUe).ToList());
        }
    }
}
