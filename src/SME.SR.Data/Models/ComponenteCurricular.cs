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
        public long CodigoTerritorioSaber { get; set; }
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
        public int? OrdemTerritorioSaber { get; set; }
        public string Professor { get; set; }

        public long? CodigoComponentePai(IEnumerable<ComponenteCurricularApiEol> componentesApiEol) => componentesApiEol?
                                            .FirstOrDefault(w => w.IdComponenteCurricular == Codigo)?.IdComponenteCurricularPai;

        public long? CodigoComponentePai(IEnumerable<ComponenteCurricular> componentesApiEol) => componentesApiEol?
                                          .FirstOrDefault(w => w.Codigo == Codigo)?.CodComponentePai;

        public bool EhCompartilhada(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.EhCompartilhada);
        }

        public bool EhCompartilhada(IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.Codigo == Codigo && w.Compartilhada);
        }

        public bool PodeLancarNota(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo) ||
                       componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.PermiteLancamentoDeNota));
        }

        public bool PodeLancarNota(IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.Codigo == Codigo) ||
                       componentesApiEol.Any(w => w.Codigo == Codigo && w.LancaNota));
        }

        public bool ControlaFrequencia(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo) ||
                       componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.PermiteRegistroFrequencia));
        }

        public bool ControlaFrequencia(IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            return componentesApiEol != null && (!componentesApiEol.Any(w => w.Codigo == Codigo) ||
                       componentesApiEol.Any(w => w.Codigo == Codigo && w.Frequencia));
        }

        public bool EhRegencia(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.IdComponenteCurricular == Codigo && w.EhRegencia);
        }

        public bool EhRegencia(IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(w => w.Codigo == Codigo && w.ComponentePlanejamentoRegencia);
        }

        public bool EhBaseNacional(IEnumerable<ComponenteCurricularApiEol> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(x => x.IdComponenteCurricular == Codigo && x.EhBaseNacional);
        }

        public bool EhBaseNacional(IEnumerable<ComponenteCurricular> componentesApiEol)
        {
            return componentesApiEol != null && componentesApiEol.Any(x => x.Codigo == Codigo && x.BaseNacional);
        }

        public ComponenteCurricularGrupoMatriz ObterGrupoMatriz(IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz)
        {
            return gruposMatriz?.FirstOrDefault(x => GrupoMatrizId == x.Id);
        }

        public AreaDoConhecimento ObterAreaDoConhecimento(IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            return areasDoConhecimentos.FirstOrDefault(x => x.CodigoComponenteCurricular == Codigo);
        }

        public ComponenteCurricularGrupoMatriz ObterGrupoMatrizSgp(IEnumerable<DisciplinaDto> disciplina, IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz)
        {
            var componente = disciplina.FirstOrDefault(x => x.Id == Codigo);

            if (componente == null)
                return null;

            return gruposMatriz?.FirstOrDefault(x => componente.GrupoMatrizId == x.Id);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
