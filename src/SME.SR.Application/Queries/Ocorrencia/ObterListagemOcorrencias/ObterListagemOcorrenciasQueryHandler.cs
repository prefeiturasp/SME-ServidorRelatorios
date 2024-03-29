﻿using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterListagemOcorrenciasQueryHandler : IRequestHandler<ObterListagemOcorrenciasQuery, RelatorioListagemOcorrenciasDto>
    {
        private readonly IMediator mediator;
        private readonly IOcorrenciaRepository ocorrenciaRepository;

        public ObterListagemOcorrenciasQueryHandler(IMediator mediator, IOcorrenciaRepository ocorrenciaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.ocorrenciaRepository = ocorrenciaRepository ?? throw new ArgumentNullException(nameof(ocorrenciaRepository));
        }

        public async Task<RelatorioListagemOcorrenciasDto> Handle(ObterListagemOcorrenciasQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioListagemOcorrenciasDto();
            relatorio.Registros = await ocorrenciaRepository.ObterListagemOcorrenciasAsync(request.AnoLetivo, request.CodigoDre, request.CodigoUe, request.Modalidade, request.Semestre, request.CodigosTurma, request.DataInicio, request.DataFim, request.OcorrenciaTipoIds, request.ImprimirDescricaoOcorrencia);

            var ocorrenciaIds = relatorio.Registros.Select(t => t.OcorrenciaId).ToArray();
            var alunos = await ocorrenciaRepository.ObterAlunosOcorrenciasPorIdsAsync(ocorrenciaIds);
            var servidores = await ocorrenciaRepository.ObterServidoresOcorrenciasPorIds(ocorrenciaIds);

            if (alunos.Any())
            {
                var codigosAlunos = alunos.Select(t => t.CodigoAluno).ToArray();
                var nomesAlunos = await mediator.Send(new ObterNomesAlunosPorCodigosQuery(codigosAlunos));
                foreach (var aluno in alunos)
                {
                    var nomeAluno = nomesAlunos.FirstOrDefault(t => t.Codigo == aluno.CodigoAluno);
                    if (nomeAluno != null)
                        aluno.Nome = nomeAluno.NomeSocial ?? nomeAluno.Nome;
                }
                nomesAlunos = null;
            }

            if (servidores.Any())
            {
                var codigosRfs = servidores.Select(t => t.CodigoRF).ToArray();
                var nomesServidores = await mediator.Send(new ObterNomesServidoresPorRfsQuery(codigosRfs));
                foreach (var servidor in servidores)
                {
                    var nomeServidor = nomesServidores.FirstOrDefault(t => t.CodigoRF == int.Parse(servidor.CodigoRF));
                    if (nomeServidor != null)
                        servidor.Nome = nomeServidor.NomeServidor;
                }
                nomesServidores = null;
            }

            foreach (var registro in relatorio.Registros)
            {
                if (alunos.Any(t => t.OcorrenciaId == registro.OcorrenciaId))
                    registro.Alunos = alunos.Where(t => t.OcorrenciaId == registro.OcorrenciaId);

                if (servidores.Any(t => t.OcorrenciaId == registro.OcorrenciaId))
                    registro.Servidores = servidores.Where(t => t.OcorrenciaId == registro.OcorrenciaId);
            }

            relatorio.Registros = relatorio.Registros.OrderByDescending(t => t.DataOcorrencia);
            return relatorio;
        }
    }
}
