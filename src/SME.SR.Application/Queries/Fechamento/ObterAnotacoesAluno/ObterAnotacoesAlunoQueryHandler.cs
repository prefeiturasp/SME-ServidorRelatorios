using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAnotacoesAlunoQueryHandler : IRequestHandler<ObterAnotacoesAlunoQuery, IEnumerable<FechamentoAlunoAnotacaoConselho>>
    {
        private readonly IFechamentoAlunoRepository fechamentoAlunoRepository;

        public ObterAnotacoesAlunoQueryHandler(IFechamentoAlunoRepository fechamentoAlunoRepository)
        {
            this.fechamentoAlunoRepository = fechamentoAlunoRepository ?? throw new ArgumentNullException(nameof(fechamentoAlunoRepository));
        }

        public async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> Handle(ObterAnotacoesAlunoQuery request, CancellationToken cancellationToken)
        {
            var anotacoesConselho = await fechamentoAlunoRepository.ObterAnotacoesTurmaAlunoBimestreAsync(request.CodigoAluno, request.FechamentoTurmaId);

            RemoverTags(anotacoesConselho);

            return anotacoesConselho;
        }

        private void RemoverTags(IEnumerable<FechamentoAlunoAnotacaoConselho> anotacaoConselhos)
        {
            DateTime data;
            foreach (var anotacao in anotacaoConselhos)
            {
                if (!string.IsNullOrEmpty(anotacao.Data) && DateTime.TryParse(anotacao.Data, out data))
                    anotacao.Data = data.ToString("dd/MM/yyyy");

                if (!string.IsNullOrEmpty(anotacao.Anotacao))
                {
                    anotacao.Anotacao = UtilRegex.RemoverTagsHtmlMidia(anotacao.Anotacao);
                    anotacao.Anotacao = UtilRegex.RemoverTagsHtml(anotacao.Anotacao);
                }
                    
            }
        }
    }
}
