using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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
        private readonly ITurmaRepository turmaRepository;

        public ObterDadosLeituraComunicadosQueryHandler(IComunicadosRepository comunicadosRepository, IAlunoRepository alunoRepository, ITurmaRepository turmaRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
            this.comunicadosRepository = comunicadosRepository ?? throw new ArgumentNullException(nameof(comunicadosRepository));
        }

        public async Task<IEnumerable<LeituraComunicadoDto>> Handle(ObterDadosLeituraComunicadosQuery request, CancellationToken cancellationToken)
        {
            var comunicados = await comunicadosRepository.ObterComunicadosPorFiltro(request.Filtro);

            if (!comunicados.Any())
                throw new NegocioException("Não foi encontrado nenhum comunicado ou todos os comunicados neste período estão expirados.");

            var dadosComunicados = await comunicadosRepository.ObterComunicadoDadosSMEPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));

            if (dadosComunicados.Any())
            {
                foreach (var comunicado in comunicados)
                {
                    var dadosComunicado = dadosComunicados.FirstOrDefault(c => c.ComunicadoId == comunicado.ComunicadoId);
                    comunicado.NaoInstalado = dadosComunicado != null ? dadosComunicado.NaoInstalado : 0;
                    comunicado.NaoVisualizado = dadosComunicado != null ? dadosComunicado.NaoVisualizado : 0;
                    comunicado.Visualizado = dadosComunicado != null ? dadosComunicado.Visualizado : 0;
                }
            }

            if (request.Filtro.CodigoUe == "-99")
                return comunicados.OrderByDescending(c => c.DataEnvio);

            if (comunicados.Any())
            {
                var usuariosApp = await comunicadosRepository.ObterUsuariosApp();
                var comunicadosTurmas = await comunicadosRepository.ObterComunicadoTurmasPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));
                var comunicadosApp = await comunicadosRepository.ObterComunicadoTurmasAppPorComunicadosIds(comunicados.Select(a => a.ComunicadoId));
                if (comunicadosTurmas.Any())
                {
                    foreach (var comunicado in comunicados)
                    {
                        if(comunicado.ComunicadoId == 741)
                        {
                            var i = 0;
                        }

                        if (request.Filtro.ListarResponsaveisEstudantes)
                        {
                            foreach (var comunicadoTurma in comunicadosTurmas.Where(c => c.ComunicadoId == comunicado.ComunicadoId))
                            {
                                var estudantes = await turmaRepository.ObterDadosAlunos(comunicadoTurma.TurmaCodigo);

                                var estudantesComunicadoDireto = await comunicadosRepository.ObterComunicadoTurmasAlunosPorComunicadoId(comunicadoTurma.ComunicadoId);

                                if (estudantesComunicadoDireto.Any())
                                {
                                    var comunicadosDireto = new List<Data.Aluno>();
                                    foreach (var estudanteComunicadoDireto in estudantesComunicadoDireto)
                                    {
                                        var estudante = estudantes.FirstOrDefault(a => a.CodigoAluno.ToString() == estudanteComunicadoDireto.ToString());
                                        if(estudante != null)
                                            comunicadosDireto.Add(estudante);
                                    }
                                    estudantes = comunicadosDireto;
                                }

                                var responsaveis = await comunicadosRepository.ObterResponsaveisPorAlunosIds(estudantes.Select(a => a.CodigoAluno).ToArray());
                                var statusReponsaveis = await comunicadosRepository.ObterComunicadoTurmasEstudanteAppPorComunicadosIds(new long[] { comunicadoTurma.ComunicadoId });
                                //var dadosAlunos = await alunoRepository.ObterDadosAlunosPorCodigosEAnoLetivo(estudantes, request.Filtro.AnoLetivo);

                                foreach (var responsavel in responsaveis)
                                {
                                    if(estudantes.Any())
                                    {
                                        LeituraComunicadoEstudanteDto estudante = new LeituraComunicadoEstudanteDto();

                                        estudante.NumeroChamada = estudantes.FirstOrDefault(a => a.CodigoAluno.ToString() == responsavel.AlunoId && a.NumeroAlunoChamada != null)?.NumeroAlunoChamada;
                                        estudante.CodigoEstudante = responsavel.AlunoId;
                                        estudante.Estudante = estudantes.FirstOrDefault(a => a.CodigoAluno.ToString() == responsavel.AlunoId && a.NumeroAlunoChamada != null)?.NomeAluno;
                                        estudante.Responsavel = responsavel.ResponsavelNome;
                                        estudante.TipoResponsavel = responsavel.TipoResponsavel.Name();
                                        estudante.ContatoResponsavel = responsavel.Contato;
                                        var situacao = statusReponsaveis.FirstOrDefault(a => a.CodigoEstudante == responsavel.AlunoId)?.Situacao;
                                        var instalado = usuariosApp.FirstOrDefault(c => c == responsavel.CPF);

                                        estudante.Situacao = situacao == null && instalado == null ? "Não instalado" : (situacao == "True" ? "Visualizado" : "Não Visualizado");

                                        comunicadoTurma.LeituraComunicadoEstudantes.Add(estudante);
                                    }
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

            return comunicados.OrderByDescending(c => c.DataEnvio);
        }

       
    }
}
