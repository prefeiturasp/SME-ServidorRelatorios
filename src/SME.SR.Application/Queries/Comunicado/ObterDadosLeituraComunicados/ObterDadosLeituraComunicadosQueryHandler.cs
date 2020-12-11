using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosLeituraComunicadosQueryHandler : IRequestHandler<ObterDadosLeituraComunicadosQuery, IEnumerable<LeituraComunicadoDto>>
    {
        private readonly IComunicadosRepository comunicadosRepository;

        public ObterDadosLeituraComunicadosQueryHandler(IComunicadosRepository comunicadosRepository)
        {
            this.comunicadosRepository = comunicadosRepository ?? throw new ArgumentNullException(nameof(comunicadosRepository));
        }

        public async Task<IEnumerable<LeituraComunicadoDto>> Handle(ObterDadosLeituraComunicadosQuery request, CancellationToken cancellationToken)
        {
            var comunicados = await comunicadosRepository.ObterComunicadosPorFiltro(request.Filtro);

            if(comunicados.Any())
            {
                var comunicadosTurmas = await comunicadosRepository.ObterComunicadoTurmasPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));
                if(comunicadosTurmas.Any())
                {
                    if(request.Filtro.ListarResponsavelEstudante)
                    {
                        foreach(var comunicadoTurma in comunicadosTurmas)
                        {
                            var estudantes = await comunicadosRepository.ObterComunicadoTurmasAlunosPorComunicadoId(comunicadoTurma.ComunicadoId);
                            var responsaveis = await comunicadosRepository.ObterResponsaveisPorAlunosIds(estudantes);

                            comunicadoTurma.LeituraComunicadoEstudantes.AddRange(MapearEstudanteDto(responsaveis));
                        }

                    }

                    foreach(var comunicado in comunicados)
                    {
                        comunicado.LeituraComunicadoTurma.AddRange(comunicadosTurmas.Where(c => c.ComunicadoId == comunicado.ComunicadoId));
                    }
                }
            }
                
            // Carrega comunicados Turma -> comunicados.Select(a => a.id)

            return comunicados;
        }

        private List<LeituraComunicadoEstudanteDto> MapearEstudanteDto(IEnumerable<LeituraComunicadoResponsaveoDto> responsaveis)
        {
            var estudantes = new List<LeituraComunicadoEstudanteDto>();

            foreach(var responsavel in responsaveis)
            {
                estudantes.Add(new LeituraComunicadoEstudanteDto()
                {
                    NumeroChamada = "",
                    CodigoEstudante = responsavel.AlunoId,
                    Estudante = "",
                    Responsavel = responsavel.ResponsavelNome,
                    TipoResponsavel = responsavel.TipoResponsavel,
                    ContatoResponsavel = responsavel.Contato,
                    Situacao = ""
                }); 
            }

            return estudantes;
        }
    }
}
