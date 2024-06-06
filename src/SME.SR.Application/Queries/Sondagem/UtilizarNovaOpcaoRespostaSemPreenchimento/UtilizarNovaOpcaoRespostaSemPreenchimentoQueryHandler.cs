using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SME.SR.Application
{
    public class UtilizarNovaOpcaoRespostaSemPreenchimentoQueryHandler : IRequestHandler<UtilizarNovaOpcaoRespostaSemPreenchimentoQuery,bool>
    {
        private const int ANO_LETIVO_DOIS_MIL_VINTE_QUATRO = 2024;
        private const int ANO_LETIVO_DOIS_MIL_VINTE_CINCO = 2025;
        private const int TERCEIRO_BIMESTRE = 3;
        private const int SEGUNDO_SEMESTRE = 2;
        
        public async Task<bool> Handle(UtilizarNovaOpcaoRespostaSemPreenchimentoQuery request, CancellationToken cancellationToken)
        {
            return request.UtilizarFiltroPorSemestre ? ConsideraNovaOpcaoRespostaSemPreenchimentoPrimeiroAoTerceiroAno(request.AnoLetivo,request.Semestre) 
                : ConsideraNovaOpcaoRespostaSemPreenchimentoQuartoAoNonoAno(request.AnoLetivo,request.Bimestre);
        }
        private static bool ConsideraNovaOpcaoRespostaSemPreenchimentoPrimeiroAoTerceiroAno(int anoLetivo, int periodo)
        {
            return anoLetivo == ANO_LETIVO_DOIS_MIL_VINTE_QUATRO && periodo == SEGUNDO_SEMESTRE || anoLetivo >= ANO_LETIVO_DOIS_MIL_VINTE_CINCO;
        }
        private static bool ConsideraNovaOpcaoRespostaSemPreenchimentoQuartoAoNonoAno(int anoLetivo, int periodo)
        {
            return anoLetivo == ANO_LETIVO_DOIS_MIL_VINTE_QUATRO && periodo >= TERCEIRO_BIMESTRE || anoLetivo >= ANO_LETIVO_DOIS_MIL_VINTE_CINCO;
        }
    }
}