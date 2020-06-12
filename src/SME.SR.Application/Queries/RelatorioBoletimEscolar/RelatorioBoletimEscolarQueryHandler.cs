using MediatR;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioBoletimEscolar
{
    public class RelatorioBoletimEscolarQueryHandler : IRequestHandler<RelatorioBoletimEscolarQuery, BoletimEscolarDto>
    {
        public Task<BoletimEscolarDto> Handle(RelatorioBoletimEscolarQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new BoletimEscolarDto
            {
                Alunos = new List<BoletimEscolarAlunoDto>
                {
                    new BoletimEscolarAlunoDto
                    {
                        Cabecalho = new BoletimEscolarCabecalhoDto
                        {
                            NomeDre = "dre 123",
                            NomeUe = "ue 123",
                            Data = DateTime.Now
                        }
                    }
                }
            });
        }
    }
}
