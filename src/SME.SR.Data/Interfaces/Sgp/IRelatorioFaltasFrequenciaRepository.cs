using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioFaltasFrequenciaRepository
    {
        Task<IEnumerable<RelatorioFaltaFrequenciaDreDto>> ObterFaltasFrequenciaPorAno(int anoLetivo,
                                                                                      string dreId,
                                                                                      string ueId,
                                                                                      Modalidade modalidade,
                                                                                      IEnumerable<string> anosEscolares,
                                                                                      IEnumerable<string> componentesCurriculares,
                                                                                      IEnumerable<int> bimestres);
    }
}