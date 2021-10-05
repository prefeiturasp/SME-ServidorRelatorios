using MediatR;
using SME.SR.Infra.Dtos.Relatorios.NotasEConceitosFinais;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterNotaConceitoEmAprovacaoQuery : IRequest<NotaConceitoEmAprovacaoDto>
    {
        public ObterNotaConceitoEmAprovacaoQuery(string codigoAluno, long? conselhoClasseAlunoId)
        {
            CodigoAluno = codigoAluno;
            ConselhoClasseAlunoId = conselhoClasseAlunoId;
        }
        public string CodigoAluno { get; set; }
        public long? ConselhoClasseAlunoId { get; set; }
    }
}
