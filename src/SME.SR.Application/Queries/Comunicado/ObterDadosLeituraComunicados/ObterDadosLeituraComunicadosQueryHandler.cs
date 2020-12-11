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
                var comunicadosApp = await comunicadosRepository.ObterComunicadoTurmasAppPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));
                if (comunicadosTurmas.Any())
                {
                    foreach(var comunicado in comunicados)
                    {
                        foreach(var comunicadoTurma in comunicadosTurmas.Where(c => c.ComunicadoId == comunicado.ComunicadoId))
                        {
                            var comunicadoTurmaApp = comunicadosApp.FirstOrDefault(c => c.TurmaCodigo == comunicadoTurma.TurmaCodigo && c.ComunicadoId == comunicado.ComunicadoId);
                            if(comunicadoTurmaApp != null)
                            {
                                comunicadoTurma.NaoInstalado = comunicadoTurmaApp.NaoInstalado;
                                comunicadoTurma.NaoVisualizado = comunicadoTurmaApp.NaoVisualizado;
                                comunicadoTurma.Visualizado = comunicadoTurmaApp.Visualizado;
                            }
                                
                            comunicado.LeituraComunicadoTurma.Add(comunicadoTurma);
                        }
                    }
                }
            }
                
            // Carrega comunicados Turma -> comunicados.Select(a => a.id)

            return comunicados;
        }
    }
}
