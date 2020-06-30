using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorIdsQueryHandler : IRequestHandler<ObterComponentesCurricularesPorIdsQuery, IEnumerable<ComponenteCurricularPorTurma>>
    {
        private IComponenteCurricularRepository _componenteCurricularRepository;

        public ObterComponentesCurricularesPorIdsQueryHandler(IComponenteCurricularRepository componenteCurricularRepository)
        {
            this._componenteCurricularRepository = componenteCurricularRepository;
        }

        public async Task<IEnumerable<ComponenteCurricularPorTurma>> Handle(ObterComponentesCurricularesPorIdsQuery request, CancellationToken cancellationToken)
        {
            var lstComponentes = await _componenteCurricularRepository.ListarComponentes();

            lstComponentes = lstComponentes
                   .Where(w => request.ComponentesCurricularesIds.Contains(w.Codigo))
                   .ToList();

            lstComponentes = lstComponentes.Concat(await _componenteCurricularRepository.ListarComponentesTerritorioSaber(request.ComponentesCurricularesIds.Select(x => x.ToString()).ToArray()));

            if (lstComponentes != null && lstComponentes.Any())
            {
                var componentesApiEol = await _componenteCurricularRepository.ListarApiEol();
                var gruposMatriz = await _componenteCurricularRepository.ListarGruposMatriz();

                return lstComponentes.Select(x => new ComponenteCurricularPorTurma
                {
                    CodDisciplina = x.Codigo,
                    CodDisciplinaPai = ObterCodDisciplinaPai(x.Codigo, componentesApiEol),
                    Disciplina = x.Descricao.Trim(),
                    Regencia = VerificaSeRegencia(x.Codigo, componentesApiEol),
                    Compartilhada = VerificaSeCompartilhada(x.Codigo, componentesApiEol),
                    Frequencia = VerificaSeRegistraFrequencia(x.Codigo, componentesApiEol),
                    LancaNota = VerificarSeLancaNota(x.Codigo, componentesApiEol),
                    TerritorioSaber = x.TerritorioSaber,
                    BaseNacional = VerificaSeBaseNacional(x.Codigo, componentesApiEol),
                    GrupoMatriz = ObterGrupoMatriz(x.Codigo, componentesApiEol, gruposMatriz)
                });
            }

            return Enumerable.Empty<ComponenteCurricularPorTurma>();
        }

        private bool VerificaSeBaseNacional(long codDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol)
        {
            return disciplinasApiEol.FirstOrDefault(x => x.IdComponenteCurricular == codDisciplina)?.EhBaseNacional ?? false;
        }

        private ComponenteCurricularGrupoMatriz ObterGrupoMatriz(long codDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol, IEnumerable<ComponenteCurricularGrupoMatriz> grupoMatrizes)
        {
            var disciplinaEol = disciplinasApiEol.FirstOrDefault(x => x.IdComponenteCurricular == codDisciplina);

            if (disciplinaEol == null)
                return null;

            return grupoMatrizes.FirstOrDefault(x => x.Id == disciplinaEol.IdGrupoMatriz);
        }

        private bool VerificarSeLancaNota(long codDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol)
        {
            return !disciplinasApiEol.Any(w => w.IdComponenteCurricular == codDisciplina) ||
                   disciplinasApiEol.Any(w => w.IdComponenteCurricular == codDisciplina && w.PermiteLancamentoDeNota);
        }

        private bool VerificaSeRegencia(long codigoDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol)
        {
            return disciplinasApiEol.Any(w => w.IdComponenteCurricular == codigoDisciplina && w.EhRegencia);
        }

        private bool VerificaSeCompartilhada(long codDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol)
        {
            return disciplinasApiEol.Any(w => w.IdComponenteCurricular == codDisciplina && w.EhCompartilhada);
        }

        private bool VerificaSeRegistraFrequencia(long codDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol)
        {
            var disciplina = disciplinasApiEol.FirstOrDefault(x => x.IdComponenteCurricular == codDisciplina);

            return disciplina == null || disciplina.PermiteRegistroFrequencia;
        }

        private long? ObterCodDisciplinaPai(long codigoDisciplina, IEnumerable<ComponenteCurricularApiEol> disciplinasApiEol)
        {
            return disciplinasApiEol.FirstOrDefault(w => w.IdComponenteCurricular == codigoDisciplina)?.IdComponenteCurricularPai;
        }


    }
}
