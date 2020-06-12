using MediatR;
using SME.SR.Data;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // se não vier turmar, pegar turmas pela UE 
            // por modalidade se tiver no filtro
            // por turma
            // por aluno

            if (request.AlunosCodigo.Any())
            {
                foreach (string codigoAluno in request.AlunosCodigo)
                {
                    var dadosAluno = await ObterDadosAluno(request.TurmaCodigo, codigoAluno);
                }
            }
            // se não tiver buscar os alunos da turma

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
