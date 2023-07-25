using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTipoTurmaRegularParaEdFisicaQuery : IRequest<Dictionary<string, TipoNota?>>
    {
        public ObterTipoTurmaRegularParaEdFisicaQuery(Turma turma, string[] codigosAlunos)
        {
            Turma = turma;
            CodigosAlunos = codigosAlunos;
        }

        public Turma Turma { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
