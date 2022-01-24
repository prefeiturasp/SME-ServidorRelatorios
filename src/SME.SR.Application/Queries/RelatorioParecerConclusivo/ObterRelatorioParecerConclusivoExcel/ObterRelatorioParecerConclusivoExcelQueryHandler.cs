using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoExcelQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoExcelQuery, IEnumerable<RelatorioParecerConclusivoExcelDto>>
    {
        private readonly IMediator mediator;

        public ObterRelatorioParecerConclusivoExcelQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
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
                                var parecerConclusivo = new RelatorioParecerConclusivoExcelDto()
                                {
                                    NomeDre = dre.Nome,
                                    NomeUe = ue.Nome,
                                    Ciclo = ciclo.Nome,
                                    Ano = ano.Nome,
                                    Turma = parecer.TurmaNome,
                                    CodigoAluno = parecer.AlunoCodigo,
                                    NomeAluno = parecer.AlunoNomeCompleto,
                                    ParecerConclusivo = parecer.ParecerConclusivoDescricao
                                };
                                await VerificarParecerEmAprovacao(parecer, parecerConclusivo, request.AnoLetivo);
                                listaPareceresConclusivos.Add(parecerConclusivo);
                            }
                        }
                    }
                }
            }
            return await Task.FromResult(listaPareceresConclusivos.OrderBy( a => a.NomeDre).ThenBy(a => a.NomeUe).ToList());
        }

        private async Task VerificarParecerEmAprovacao(RelatorioParecerConclusivoAlunoDto relatorioParecerAluno, RelatorioParecerConclusivoExcelDto parecerConclusivoExcel, int ano)
        {
            string parecerConclusivoDescricao = await mediator.Send(new ObterDescricaoParecerEmAprovacaoQuery(relatorioParecerAluno.AlunoCodigo, ano));
            if (parecerConclusivoDescricao != null)
            {
                parecerConclusivoExcel.ParecerConclusivo = parecerConclusivoDescricao;
                parecerConclusivoExcel.EmAprovacao = true;
            }
            else
            {
                parecerConclusivoExcel.EmAprovacao = false;
            }
        }
    }
}
