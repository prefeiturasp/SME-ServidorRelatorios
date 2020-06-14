using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAnotacoesAlunoQueryHandler : IRequestHandler<ObterAnotacoesAlunoQuery, IEnumerable<FechamentoAlunoAnotacaoConselho>>
    {
        private IFechamentoAlunoRepository _fechamentoAlunoRepository;

        public ObterAnotacoesAlunoQueryHandler(IFechamentoAlunoRepository fechamentoAlunoRepository)
        {
            this._fechamentoAlunoRepository = fechamentoAlunoRepository;
        }

        public async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> Handle(ObterAnotacoesAlunoQuery request, CancellationToken cancellationToken)
        {
            var anotacoesConselho = await _fechamentoAlunoRepository.ObterAnotacoesTurmaAlunoBimestreAsync(request.CodigoAluno, request.FechamentoTurmaId);

            RemoverTags(anotacoesConselho);

            return anotacoesConselho;
        }

        private void RemoverTags(IEnumerable<FechamentoAlunoAnotacaoConselho> anotacaoConselhos)
        {
            foreach (var anotacao in anotacaoConselhos)
            {
                foreach (PropertyInfo prop in anotacao.GetType().GetProperties())
                {
                    var tipo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var valor = prop.GetValue(anotacao, null)?.ToString();
                    if (tipo == typeof(string) && !string.IsNullOrEmpty(valor))
                    {
                        DateTime valorData;
                        if (DateTime.TryParse(valor, out valorData))
                            prop.SetValue(anotacao, valorData.ToString("dd/MM/yyyy"));
                        else
                            prop.SetValue(anotacao, Regex.Replace(valor, "<.*?>", String.Empty));
                    }
                }
            }
        }
    }
}
