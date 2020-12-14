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
        private readonly IAlunoRepository alunoRepository;

        public ObterDadosLeituraComunicadosQueryHandler(IComunicadosRepository comunicadosRepository, IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.comunicadosRepository = comunicadosRepository ?? throw new ArgumentNullException(nameof(comunicadosRepository));
        }

        public async Task<IEnumerable<LeituraComunicadoDto>> Handle(ObterDadosLeituraComunicadosQuery request, CancellationToken cancellationToken)
        {
            var comunicados = await comunicadosRepository.ObterComunicadosPorFiltro(request.Filtro);

            if (comunicados.Any())
            {
                var comunicadosTurmas = await comunicadosRepository.ObterComunicadoTurmasPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));
                var comunicadosApp = await comunicadosRepository.ObterComunicadoTurmasAppPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));
                if (comunicadosTurmas.Any())
                {
                    foreach (var comunicado in comunicados)
                    {
                        if (request.Filtro.ListarResponsavelEstudante)
                        {
                            foreach (var comunicadoTurma in comunicadosTurmas.Where(c => c.ComunicadoId == comunicado.ComunicadoId))
                            {
                                var estudantes = await comunicadosRepository.ObterComunicadoTurmasAlunosPorComunicadoId(comunicadoTurma.ComunicadoId);
                                var responsaveis = await comunicadosRepository.ObterResponsaveisPorAlunosIds(estudantes);
                                var statusReponsaveis = await comunicadosRepository.ObterComunicadoTurmasEstudanteAppPorComunicadosIds(new long[] { comunicadoTurma.ComunicadoId });
                                var dadosAlunos = await alunoRepository.ObterDadosAlunosPorCodigosEAnoLetivo(estudantes, request.Filtro.AnoLetivo);

                                foreach (var responsavel in responsaveis)
                                {
                                    LeituraComunicadoEstudanteDto estudante = new LeituraComunicadoEstudanteDto();

                                    estudante.NumeroChamada = dadosAlunos.FirstOrDefault(a => a.CodigoAluno.ToString() == responsavel.AlunoId).NumeroAlunoChamada;
                                    estudante.CodigoEstudante = responsavel.AlunoId;
                                    estudante.Estudante = dadosAlunos.FirstOrDefault(a => a.CodigoAluno.ToString() == responsavel.AlunoId).NomeAluno;
                                    estudante.Responsavel = responsavel.ResponsavelNome;
                                    estudante.TipoResponsavel = responsavel.TipoResponsavel;
                                    estudante.ContatoResponsavel = responsavel.Contato;
                                    estudante.Situacao = statusReponsaveis.FirstOrDefault(a => a.CodigoEstudante == responsavel.AlunoId).Situacao;

                                    comunicadoTurma.LeituraComunicadoEstudantes.Add(estudante);

                                }
                            }
                        }

                        foreach (var comunicadoTurma in comunicadosTurmas.Where(c => c.ComunicadoId == comunicado.ComunicadoId))
                        {
                            var comunicadoTurmaApp = comunicadosApp.FirstOrDefault(c => c.TurmaCodigo == comunicadoTurma.TurmaCodigo && c.ComunicadoId == comunicado.ComunicadoId);
                            if (comunicadoTurmaApp != null)
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

        private List<LeituraComunicadoEstudanteDto> MapearEstudanteDto(IEnumerable<LeituraComunicadoResponsavelDto> responsaveis)
        {
            var estudantes = new List<LeituraComunicadoEstudanteDto>();

            foreach (var responsavel in responsaveis)
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
