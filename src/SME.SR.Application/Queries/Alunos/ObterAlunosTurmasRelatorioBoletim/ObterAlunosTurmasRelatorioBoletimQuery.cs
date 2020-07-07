﻿using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, Aluno>>>
    {
        public string[] CodigosTurma { get; set; }

        public string[] CodigosAlunos { get; set; }
    }
}
