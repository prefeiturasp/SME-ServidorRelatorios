using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDevolutivasQuery : IRequest<IEnumerable<TurmasDevolutivasDto>>
    {
        private IEnumerable<long> enumerable;
        private string v;

        public ObterDevolutivasQuery(long ueId, IEnumerable<long> turmas, IEnumerable<int> bimestres, int ano, long componenteCurricular, bool utilizarLayoutNovo)
        {
            UeId = ueId;
            Turmas = turmas;
            Bimestres = bimestres;
            Ano = ano;
            ComponenteCurricular = componenteCurricular;
            UtilizarLayoutNovo = utilizarLayoutNovo;
        }

        public ObterDevolutivasQuery(long ueId, IEnumerable<long> enumerable, string v, int ano)
        {
            UeId = ueId;
            this.enumerable = enumerable;
            this.v = v;
            Ano = ano;
        }

        public long UeId { get; }
        public IEnumerable<long> Turmas { get; }
        public IEnumerable<int> Bimestres { get; }
        public int Ano { get; }
        public long ComponenteCurricular { get; }
        public bool UtilizarLayoutNovo { get; set; }
    }
}
