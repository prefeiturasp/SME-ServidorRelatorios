using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Data
{
    public class ComponenteCurricular : ICloneable
    {
        public ComponenteCurricular()
        {
            ComponentePlanejamentoRegencia = false;
        }
        public string CodigoAluno { get; set; }
        public string CodigoTurma { get; set; }
        public string AnoTurma { get; set; }
        public long Codigo { get; set; }
        public long CodigoComponenteCurricularTerritorioSaber { get; set; }
        public string Descricao { get; set; }
        public string DescricaoFormatada => string.IsNullOrWhiteSpace(Descricao) ? string.Empty : Descricao.Trim();
        public bool TerritorioSaber { get; set; }
        public string TipoEscola { get; set; }
        public int TurnoTurma { get; set; }
        public bool ComponentePlanejamentoRegencia { get; set; }
        public long? CodComponentePai { get; set; }
        public long GrupoMatrizId { get; set; }
        public bool Compartilhada { get; set; }
        public bool LancaNota { get; set; }
        public bool Frequencia { get; set; }
        public bool BaseNacional { get; set; }
        public string DescricaoInfantil { get; set; }
        public string Professor { get; set; }

        public long? CodigoComponentePai(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesApiEol) => componentesApiEol?
                                          .FirstOrDefault(w => w.Codigo == Codigo)?.CodComponenteCurricularPai;

        public bool EhCompartilhada(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.Codigo == Codigo && w.Compartilhada);
        }

        public bool PodeLancarNota(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.Codigo == Codigo) ||
                       componentesApiEol.Any(w => w.Codigo == Codigo && w.LancaNota));
        }

        public bool ControlaFrequencia(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.Codigo == Codigo) ||
                       componentesApiEol.Any(w => w.Codigo == Codigo && w.RegistraFrequencia));
        }

        public bool EhRegencia(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.Codigo == Codigo && w.EhRegencia);
        }

        public bool EhBaseNacional(IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(x => x.Codigo == Codigo && x.BaseNacional);
        }

        public ComponenteCurricularGrupoMatriz ObterGrupoMatriz(IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz)
        {
            return gruposMatriz?.FirstOrDefault(x => GrupoMatrizId == x.Id);
        }

        public AreaDoConhecimento ObterAreaDoConhecimento(IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            return areasDoConhecimentos.FirstOrDefault(x => x.CodigoComponenteCurricular == Codigo || x.CodigoComponenteCurricular == CodigoComponenteCurricularTerritorioSaber);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
