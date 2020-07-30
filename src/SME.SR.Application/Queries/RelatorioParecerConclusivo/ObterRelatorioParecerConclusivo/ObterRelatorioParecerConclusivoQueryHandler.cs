using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoQuery, RelatorioParecerConclusivoDto>
    {
        private readonly IParecerConclusivoRepository parecerConclusivoRepository;
        private readonly ITurmaRepository turmaRepository;

        public ObterRelatorioParecerConclusivoQueryHandler(IParecerConclusivoRepository parecerConclusivoRepository, ITurmaRepository turmaRepository)
        {
            this.parecerConclusivoRepository = parecerConclusivoRepository ?? throw new System.ArgumentNullException(nameof(parecerConclusivoRepository));
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }
        public async Task<RelatorioParecerConclusivoDto> Handle(ObterRelatorioParecerConclusivoQuery request, CancellationToken cancellationToken)
        {
            var retorno = new RelatorioParecerConclusivoDto();

            var parecesParaTratar = await parecerConclusivoRepository.ObterPareceresFinais(request.filtroRelatorioParecerConclusivoDto.AnoLetivo,
                request.filtroRelatorioParecerConclusivoDto.DreCodigo, request.filtroRelatorioParecerConclusivoDto.UeCodigo, request.filtroRelatorioParecerConclusivoDto.Modalidade,
                request.filtroRelatorioParecerConclusivoDto.Semestre, request.filtroRelatorioParecerConclusivoDto.CicloId, request.filtroRelatorioParecerConclusivoDto.Anos,
                request.filtroRelatorioParecerConclusivoDto.ParecerConclusivoId);


            if (parecesParaTratar == null || !parecesParaTratar.Any())
                throw new NegocioException("Não foi possível localizar informações com o(s) filtro(s) informado(s).");

            var turmasCodigos = parecesParaTratar.Select(a => a.TurmaId).Distinct();

            var alunosDasTurmas = await turmaRepository.ObterAlunosPorTurmas(turmasCodigos);

            if (alunosDasTurmas == null || !alunosDasTurmas.Any())
                throw new NegocioException("Não foi possível localizar informações dos alunos.");

            MontaCabecalho(request, retorno, parecesParaTratar);            

            MontaSecoes(retorno, parecesParaTratar, alunosDasTurmas);

            return await Task.FromResult(retorno);
        }

        private static void MontaSecoes(RelatorioParecerConclusivoDto retorno, IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, IEnumerable<Aluno> alunosDasTurmas)
        {
            var dresCodigos = parecesParaTratar.Select(a => a.DreCodigo).Distinct();

            foreach (var dreCodigo in dresCodigos)
            {
                var dreParaAdicionar = new RelatorioParecerConclusivoDreDto();
                dreParaAdicionar.Codigo = dreCodigo;
                dreParaAdicionar.Nome = parecesParaTratar.FirstOrDefault(a => a.DreCodigo == dreCodigo).DreNome;

                TrataUes(parecesParaTratar, alunosDasTurmas, dreParaAdicionar, dreCodigo);
                retorno.Dres.Add(dreParaAdicionar);
            }
        }

        private static void TrataUes(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, IEnumerable<Aluno> alunosDasTurmas, RelatorioParecerConclusivoDreDto dreParaAdicionar, string dreCodigo)
        {
            var uesCodigos = parecesParaTratar
                    .Where(a => a.DreCodigo == dreCodigo)
                    .Select(a => a.UeCodigo)
                    .Distinct();

            foreach (var ueCodigo in uesCodigos)
            {

                var ueParaAdicionar = new RelatorioParecerConclusivoUeDto();
                ueParaAdicionar.Nome = parecesParaTratar.FirstOrDefault(a => a.UeCodigo == ueCodigo).UeNome;
                ueParaAdicionar.Codigo = ueCodigo;

                TrataCiclos(parecesParaTratar, alunosDasTurmas, ueCodigo, ueParaAdicionar);
                dreParaAdicionar.Ues.Add(ueParaAdicionar);
            }
        }

        private static void TrataCiclos(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, IEnumerable<Aluno> alunosDasTurmas, string ueCodigo, RelatorioParecerConclusivoUeDto ueParaAdicionar)
        {
            var ciclosIds = parecesParaTratar
                            .Where(a => a.UeCodigo == ueCodigo)
                            .Select(a => a.CicloId)
                            .Distinct();

            foreach (var cicloId in ciclosIds)
            {
                var cicloParaAdicionar = new RelatorioParecerConclusivoCicloDto();
                cicloParaAdicionar.Codigo = cicloId.ToString();
                cicloParaAdicionar.Nome = parecesParaTratar.FirstOrDefault(a => a.CicloId == cicloId).Ciclo;
                TrataAnos(parecesParaTratar, alunosDasTurmas, ueCodigo, cicloId, cicloParaAdicionar);
                ueParaAdicionar.Ciclos.Add(cicloParaAdicionar);
            }
        }

        private static void TrataAnos(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, IEnumerable<Aluno> alunosDasTurmas, string ueCodigo, long cicloId, RelatorioParecerConclusivoCicloDto cicloParaAdicionar)
        {
            var anosParaAdicionar = parecesParaTratar
                        .Where(a => a.CicloId == cicloId)
                        .Select(a => a.Ano)
                        .Distinct();

            foreach (var anoParaAdicionar in anosParaAdicionar)
            {
                var anoParaIncluir = new RelatorioParecerConclusivoAnoDto();
                anoParaIncluir.Nome = anoParaAdicionar + "º Ano";

                TrataTurmas(parecesParaTratar, alunosDasTurmas, ueCodigo, cicloId, anoParaAdicionar, anoParaIncluir);
                cicloParaAdicionar.Anos.Add(anoParaIncluir);
            }
        }

        private static void TrataTurmas(IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar, IEnumerable<Aluno> alunosDasTurmas, string ueCodigo, long cicloId, string anoParaAdicionar, RelatorioParecerConclusivoAnoDto anoParaIncluir)
        {
            var pareceresFiltradoParaAdicionar = parecesParaTratar
                .Where(a => a.CicloId == cicloId && a.Ano == anoParaAdicionar && a.UeCodigo == ueCodigo)
                .Distinct()
                .ToList();

            //Loopar turmas
            var turmasIdsParaTratar = pareceresFiltradoParaAdicionar
                    .Select(a => a.TurmaId)
                    .Distinct();

            foreach (var turmaIdParaTratar in turmasIdsParaTratar)
            {
                var alunosDaTurmaParaIncluir = alunosDasTurmas
                    .Where(a => a.CodigoTurma == (int)turmaIdParaTratar)
                    .Distinct();

                foreach (var alunoParaIncluir in alunosDaTurmaParaIncluir)
                {
                    var parecerParaIncluir = new RelatorioParecerConclusivoAlunoDto();
                    parecerParaIncluir.AlunoNomeCompleto = alunoParaIncluir.ObterNomeFinal();
                    parecerParaIncluir.AlunoNumeroChamada = alunoParaIncluir.NumeroAlunoChamada ?? "";
                    parecerParaIncluir.TurmaNome = parecesParaTratar.FirstOrDefault(a => a.TurmaId == turmaIdParaTratar).TurmaNome;

                    var parecerFiltradoParaIncluir = parecesParaTratar.FirstOrDefault(a => a.TurmaId == turmaIdParaTratar
                                                     && a.AlunoCodigo == alunoParaIncluir.CodigoAluno && a.Ano == anoParaAdicionar
                                                     && a.CicloId == cicloId);

                    if (parecerFiltradoParaIncluir == null)
                        parecerParaIncluir.ParecerConclusivoDescricao = "Sem Parecer";
                    else parecerParaIncluir.ParecerConclusivoDescricao = parecerFiltradoParaIncluir.ParecerConclusivo;

                    anoParaIncluir.PareceresConclusivos.Add(parecerParaIncluir);

                }
            }
            anoParaIncluir.PareceresConclusivos = anoParaIncluir.PareceresConclusivos.OrderBy(a => a.AlunoNomeCompleto).ToList();
        }

        private static void MontaCabecalho(ObterRelatorioParecerConclusivoQuery request, RelatorioParecerConclusivoDto retorno, System.Collections.Generic.IEnumerable<RelatorioParecerConclusivoRetornoDto> parecesParaTratar)
        {
            retorno.Ano = (request.filtroRelatorioParecerConclusivoDto.Anos != null && request.filtroRelatorioParecerConclusivoDto.Anos.Length > 0) ? request.filtroRelatorioParecerConclusivoDto.Anos[0] : "Todos" ;

            if (request.filtroRelatorioParecerConclusivoDto.CicloId == 0)
                retorno.Ciclo = "Todos";
            else
                retorno.Ciclo = parecesParaTratar.FirstOrDefault(a => a.CicloId == request.filtroRelatorioParecerConclusivoDto.CicloId).Ciclo;

            retorno.Data = DateTime.Now.ToString("dd/MM/yyyy");

            if (string.IsNullOrEmpty(request.filtroRelatorioParecerConclusivoDto.DreCodigo))
                retorno.DreNome = "Todas";
            else retorno.DreNome = parecesParaTratar.FirstOrDefault(a => a.DreCodigo == request.filtroRelatorioParecerConclusivoDto.DreCodigo).DreNome;
            

            if (string.IsNullOrEmpty(request.filtroRelatorioParecerConclusivoDto.UeCodigo))
                retorno.UeNome = "Todas";
            else retorno.UeNome = parecesParaTratar.FirstOrDefault(a => a.UeCodigo == request.filtroRelatorioParecerConclusivoDto.UeCodigo).UeNome;

            retorno.RF = request.UsuarioRf;
            retorno.Usuario = request.filtroRelatorioParecerConclusivoDto.UsuarioNome;
        }
    }
}
