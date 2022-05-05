using MediatR;
using Newtonsoft.Json;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosAlunosEscolaQueryHandler : IRequestHandler<ObterDadosAlunosEscolaQuery, IEnumerable<DadosAlunosEscolaDto>>
    {
        private readonly IAlunoRepository alunoRepository;
        private readonly IRepositorioCache repositorioCache;

        public ObterDadosAlunosEscolaQueryHandler(IAlunoRepository alunoRepository, IRepositorioCache repositorioCache)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
        }

        public async Task<IEnumerable<DadosAlunosEscolaDto>> Handle(ObterDadosAlunosEscolaQuery request, CancellationToken cancellationToken)
        {
            var cacheChave = $"dados-alunos-escola:{request.CodigoEscola}";
            var cacheAlunos = repositorioCache.Obter(cacheChave);

            if (cacheAlunos != null)
                return JsonConvert.DeserializeObject<List<DadosAlunosEscolaDto>>(cacheAlunos);
            else
            {
                var listaAlunos = await alunoRepository.ObterDadosAlunosEscola(request.CodigoEscola, request.AnoLetivo,request.CodigosAlunos);
                var json = JsonConvert.SerializeObject(listaAlunos);
                await repositorioCache.SalvarAsync(cacheChave, json);
                return listaAlunos;
            }
        }
    }
}
