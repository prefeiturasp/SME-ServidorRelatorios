﻿using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasRelatorioBoletimQuery : IRequest<IEnumerable<Turma>>
    {
        public string CodigoTurma { get; set; }

        public string CodigoUe { get; set; }

        public Modalidade Modalidade { get; set; }

        public int AnoLetivo { get; set; }

        public int Semestre { get; set; }

        public Usuario Usuario { get; set; }
        public bool ConsideraHistorico { get; set; }
    }
}
