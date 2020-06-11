using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Data
{
    public class ComponenteCurricular
    {
        public ComponenteCurricular()
        {
            ComponentePlanejamentoRegencia = false;
        }
        public string AnoTurma { get; set; }
        public long Codigo { get; set; }
        public string Descricao { get; set; }
        public string DescricaoFormatada => string.IsNullOrWhiteSpace(Descricao) ? string.Empty : Descricao.Trim();
        public bool TerritorioSaber { get; set; }
        public string TipoEscola { get; set; }
        public int TurnoTurma { get; set; }
        public bool ComponentePlanejamentoRegencia { get; set; }

        public long? CodigoComponentePai(IEnumerable<ComponenteCurricularApiEol> componentesApiEol) => componentesApiEol?
                                            .FirstOrDefault(w => w.IdComponenteCurricular == Codigo)?.IdComponenteCurricularPai;

        public bool EhCompartilhada(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.EhCompartilhada);
        }

        public bool PodeLancarNota(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo) ||
                       componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.PermiteLancamentoDeNota));
        }

        public bool EhRegencia(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.EhRegencia);
        }

        public bool EhBaseNacional(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(x => x.IdComponenteCurricular == Codigo && x.EhBaseNacional);
        }

        public ComponenteCurricularGrupoMatriz ObterGrupoMatriz(IEnumerable<ComponenteCurricularApiEol> componentesApiEol, IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz)
        {
            var componente = componentesApiEol.FirstOrDefault(x => x.IdComponenteCurricular == Codigo);

            if (componente == null)
                return null;

            return gruposMatriz?.FirstOrDefault(x => componente.IdGrupoMatriz == x.Id);
        }
    }
}
