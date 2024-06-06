
using MediatR;

namespace SME.SR.Application
{
    public class UtilizarNovaOpcaoRespostaSemPreenchimentoQuery : IRequest<bool>
    {
        public UtilizarNovaOpcaoRespostaSemPreenchimentoQuery(int semestre, int bimestre, int anoLetivo)
        {
            Semestre = semestre;
            Bimestre = bimestre;
            AnoLetivo = anoLetivo;
            UtilizarFiltroPorSemestre = semestre > 0;
        }

        public int Semestre { get;}
        public int Bimestre { get;}
        public int AnoLetivo { get;}
        public bool UtilizarFiltroPorSemestre { get; }
    }
}