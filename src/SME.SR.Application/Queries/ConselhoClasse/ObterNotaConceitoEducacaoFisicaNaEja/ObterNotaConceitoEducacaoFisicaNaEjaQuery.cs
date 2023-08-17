using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotaConceitoEducacaoFisicaNaEjaQuery : IRequest<IEnumerable<AlunoNotaTipoNotaDtoEducacaoFisicaDto>>
    {
        public ObterNotaConceitoEducacaoFisicaNaEjaQuery(string[] alunosCodigos, Turma turma)
        {
            AlunosCodigos = alunosCodigos;
            Turma = turma;
        }

        public string[] AlunosCodigos { get; set; }
        
        public Turma Turma { get; set; }
    }
}
