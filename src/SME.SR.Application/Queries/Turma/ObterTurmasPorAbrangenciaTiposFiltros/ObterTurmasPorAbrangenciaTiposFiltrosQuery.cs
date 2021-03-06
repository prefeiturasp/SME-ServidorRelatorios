﻿using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorAbrangenciaTiposFiltrosQuery : IRequest<IEnumerable<Turma>>
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }

        public Modalidade Modalidade { get; set; }

        public int AnoLetivo { get; set; }

        public int Semestre { get; set; }

        public string Login { get; set; }

        public Guid Perfil { get; set; }

        public bool ConsideraHistorico { get; set; }

        public bool? PossuiFechamento { get; set; }

        public bool? SomenteEscolarizadas { get; set; }
        public int[] Tipos { get; set; }

        public SituacaoFechamento? SituacaoFechamento { get; set; }

        public SituacaoConselhoClasse? SituacaoConselhoClasse  { get; set; }

        public int[] Bimestres { get; set; }
    }
}
