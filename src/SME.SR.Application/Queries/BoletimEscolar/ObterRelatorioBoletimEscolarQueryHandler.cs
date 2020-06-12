using MediatR;
using SME.SR.Data;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterRelatorioBoletimEscolarQueryHandler : IRequestHandler<ObterRelatorioBoletimEscolarQuery, BoletimEscolarDto>
    {
        private IMediator _mediator;

        public async Task<BoletimEscolarDto> Handle(ObterRelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            BoletimEscolarDto relatorio = new BoletimEscolarDto();

            //foreach (string codigoAluno in request.CodigosAluno)
            //{

            //    var dadosAluno = await ObterDadosAluno(request.CodigoTurma, codigoAluno);
            //}

            return relatorio;
        }


        private async Task<Aluno> ObterDadosAluno(string codigoTurma, string codigoAluno)
        {
            return await _mediator.Send(new ObterDadosAlunoQuery()
            {
                CodigoTurma = codigoTurma,
                CodigoAluno = codigoAluno
            });
        }
    }
}
