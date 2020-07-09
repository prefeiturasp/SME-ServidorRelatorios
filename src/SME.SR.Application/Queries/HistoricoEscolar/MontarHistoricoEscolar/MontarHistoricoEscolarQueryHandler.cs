using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarQueryHandler : IRequestHandler<MontarHistoricoEscolarQuery, HistoricoEscolarDTO>
    {
        public MontarHistoricoEscolarQueryHandler()
        {

        }

        public async Task<HistoricoEscolarDTO> Handle(MontarHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            var retorno = new HistoricoEscolarDTO();

            foreach (var turmaCodigo in request.TurmasCodigo)
            {
                retorno.NomeDre = request.Dre.Nome;
                retorno.Cabecalho = ObterCabecalho(request.Ue);

                var componentesDaTurma = request.ComponentesCurricularesTurmas.FirstOrDefault(cc => cc.Key == turmaCodigo);

                retorno.GruposComponentesCurriculares = ObterGruposComponentesCurriculares(componentesDaTurma, request.AreasConhecimento);

                foreach (var aluno in request.AlunosTurmas.Where(a => a.Turmas.Select(t => t.Codigo).Contains(turmaCodigo)))
                {
                }
            }

            return retorno;

        }

        private CabecalhoDto ObterCabecalho(Ue ue)
        {
            return new CabecalhoDto()
            {
                NomeUe = ue.NomeRelatorio,
            };
        }

        private List<GruposComponentesCurricularesDto> ObterGruposComponentesCurriculares(IEnumerable<ComponenteCurricularPorTurma> componentesCurricularesDaTurma,
                                                                                          IEnumerable<AreaDoConhecimento> areasDoConhecimentos)
        {
            var gruposComponentes = new List<GruposComponentesCurricularesDto>();

            var componentesPorGrupoMatriz = componentesCurricularesDaTurma.GroupBy(cc => cc.GrupoMatriz);

            gruposComponentes.AddRange(componentesPorGrupoMatriz.Select(gp => new GruposComponentesCurricularesDto
            {
                Nome = gp.Key.Nome,
                AreasDeConhecimento = areasDoConhecimentos.Where(ac => gp.Select(c => c.CodDisciplina)
                                      .Contains(ac.CodigoComponenteCurricular)).Select(ac => new AreaDeConhecimentoDto()
                                      {
                                          Nome = ac.Nome,
                                          ComponentesCurriculares = gp.Select(cc => new ComponenteCurricularDto()
                                          {
                                              Nome = cc.Disciplina,
                                              Codigo = cc.CodDisciplina.ToString()
                                          }).ToList()
                                      }).ToList()
            }));

            return gruposComponentes;
        }
    }
}
