using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDevolutivasQuery : IRequest<IEnumerable<TurmasDevolutivasDto>>
    {
        public ObterDevolutivasQuery(long ueId, IEnumerable<long> turmas, IEnumerable<int> bimestres, int ano)
        {
            UeId = ueId;
            Turmas = turmas;
            Bimestres = bimestres;
            Ano = ano;
        }

        public long UeId { get; }
        public IEnumerable<long> Turmas { get; }
        public IEnumerable<int> Bimestres { get; }
        public int Ano { get; }
    }
}
