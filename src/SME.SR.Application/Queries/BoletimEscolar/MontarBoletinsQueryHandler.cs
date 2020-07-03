using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarBoletinsQueryHandler : IRequestHandler<MontarBoletinsQuery, BoletimEscolarDto>
    {
        public async Task<BoletimEscolarDto> Handle(MontarBoletinsQuery request, CancellationToken cancellationToken)
        {
            var turmas = request.Turmas;
            var dre = request.Dre;
            var ue = request.Ue;
            var alunos = request.AlunosPorTuma;
            var componentesCurriculares = request.ComponentesCurricularesPorTurma;
            var notasFrequencia = request.NotasFrequencia;

            foreach (var turma in turmas)
            {
                var boletimEscolarAlunoDto = new BoletimEscolarAlunoDto();

                boletimEscolarAlunoDto.Cabecalho = ObterCabecalhoInicial(dre, ue, turma);

                boletimEscolarAlunoDto.Grupos = MapearGruposEComponentes(componentesCurriculares.FirstOrDefault(cc => cc.Key == turma.Codigo));

                foreach (var aluno in alunos.Where(a => a.Key == turma.Codigo))
                {

                }

            }

            return await Task.FromResult(new BoletimEscolarDto());

        }

        private BoletimEscolarCabecalhoDto ObterCabecalhoInicial(Dre dre, Ue ue, Turma turma)
        {
            return new BoletimEscolarCabecalhoDto()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),
                NomeDre = dre.Abreviacao,
                NomeUe = ue.NomeRelatorio
            };
        }

        private List<GrupoMatrizComponenteCurricularDto> MapearGruposEComponentes(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesPorTurma)
        {
            var gruposMatrizes = componentesCurricularesPorTurma.GroupBy(cc => cc.GrupoMatriz);

            var gruposRetorno = new List<GrupoMatrizComponenteCurricularDto>();

            foreach (var grupoMatriz in gruposMatrizes)
            {
                var grupoParaAdd = new GrupoMatrizComponenteCurricularDto()
                {
                    Id = (int)grupoMatriz.Key.Id,
                    Nome = "Grupo 1",
                    Descricao = grupoMatriz.Key.Nome
                };

                foreach(var componente in grupoMatriz)
                {
                }
            }

            return gruposRetorno;
        }
    }
}
