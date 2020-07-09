using MediatR;
using Newtonsoft.Json;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioHistoricoEscolarUseCase : IRelatorioHistoricoEscolarUseCase
    {
        private readonly IMediator mediator;

        public RelatorioHistoricoEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {

            var filtros = request.ObterObjetoFiltro<FiltroHistoricoEscolarDto>();

            var cabecalho = await MontarCabecalho(filtros);

            //Obter Alunos e Turmas
            var alunosTurmas = await MontarAlunosTurmas(filtros);

            var turmas = new List<Turma>();

            foreach (var aluno in alunosTurmas)
            {
                foreach (var turma in aluno.Turmas)
                {
                    if (!turmas.Any(a => a.Codigo == turma.Codigo))
                        turmas.Add(turma);
                }                
            }

            var alunosCodigo = alunosTurmas.Select(a => a.Aluno.Codigo);

            var turmasCodigo = turmas.Select(a => a.Codigo).Distinct();

            var componentesCurriculares = await ObterComponentesCurricularesTurmasRelatorio(turmasCodigo.ToArray(), filtros.UeCodigo, filtros.Modalidade, filtros.Usuario);

            var areasDoConhecimento = await ObterAreasConhecimento(componentesCurriculares);

            var dre = await ObterDrePorCodigo(filtros.DreCodigo);

            var ue = await ObterUePorCodigo(filtros.UeCodigo);

            var enderecoAtoUe = await ObterEnderecoAtoUe(filtros.UeCodigo);

            var tipoNotas = await ObterTiposNotaRelatorio(filtros.AnoLetivo, dre.Id, ue.Id, filtros.Semestre, filtros.Modalidade, turmas);

            var notas = await ObterNotasAlunos(turmasCodigo.ToArray(), alunosCodigo.ToArray());
            var frequencias = await ObterFrequenciasAlunos(turmasCodigo.ToArray(), alunosCodigo.ToArray());

            var mediasFrequencia = await ObterMediasFrequencia();

            var resultadoFinal = await mediator.Send(new MontarHistoricoEscolarQuery(dre, ue, areasDoConhecimento, componentesCurriculares, alunosTurmas, mediasFrequencia, notas, frequencias, turmasCodigo.ToArray(), cabecalho));

            var jsonString = "";

            if (resultadoFinal != null)
            {
                jsonString = JsonConvert.SerializeObject(resultadoFinal);
            }

            await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioHistoricoEscolarFundamental/HistoricoEscolar", jsonString, TipoFormatoRelatorio.Pdf, request.CodigoCorrelacao));
        }

        private async Task<IEnumerable<EnderecoEAtosDaUeDto>> ObterEnderecoAtoUe(string ueCodigo)
        {
            return await mediator.Send(new ObterEnderecoEAtosDaUeQuery(ueCodigo));
        }

        private async Task<Dre> ObterDrePorCodigo(string dreCodigo)
        {
            return await mediator.Send(new ObterDrePorCodigoQuery()
            {
                DreCodigo = dreCodigo
            });
        }

        private async Task<Ue> ObterUePorCodigo(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoQuery()
            {
                UeCodigo = ueCodigo
            });
        }
        private async Task<IDictionary<string, string>> ObterTiposNotaRelatorio(int anoLetivo, long dreId, long ueId, int semestre, Modalidade modalidade, IEnumerable<Turma> turmas)
        {
            return await mediator.Send(new ObterTiposNotaRelatorioBoletimQuery()
            {
                AnoLetivo = anoLetivo,
                DreId = dreId,
                UeId = ueId,
                Semestre = semestre,
                Modalidade = modalidade,
                Turmas = turmas
            });
        }
        private async Task<IEnumerable<AreaDoConhecimento>> ObterAreasConhecimento(IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurriculares)
        {
            //TODO: MELHORAR CODIGO
            var listaCodigosComponentes = new List<long>();

            listaCodigosComponentes.AddRange(componentesCurriculares.SelectMany(a => a).Select(a => a.CodDisciplina).Distinct());

            return await mediator.Send(new ObterAreasConhecimentoComponenteCurricularQuery(listaCodigosComponentes.ToArray()));
        }

        private async Task<IEnumerable<AlunoTurmasHistoricoEscolarDto>> MontarAlunosTurmas(FiltroHistoricoEscolarDto filtros)
        {

            var alunosCodigos = filtros.AlunosCodigo.Select(long.Parse).ToArray();

            var turmaCodigo = string.IsNullOrEmpty(filtros.TurmaCodigo) ? 0 : long.Parse(filtros.TurmaCodigo);

            return await mediator.Send(new ObterAlunosETurmasHistoricoEscolarQuery(turmaCodigo, alunosCodigos));
        }

        private async Task<CabecalhoDto> MontarCabecalho(FiltroHistoricoEscolarDto filtros)
        {
            var enderecosEAtos = await mediator.Send(new ObterEnderecoEAtosDaUeQuery(filtros.UeCodigo));
            if (!enderecosEAtos.Any())
                throw new NegocioException("Não foi possível obter os dados de endereço e atos da UE.");

            return MontaCabecalhoComBaseNoEnderecoEAtosDaUe(enderecosEAtos);
        }

        private static CabecalhoDto MontaCabecalhoComBaseNoEnderecoEAtosDaUe(IEnumerable<EnderecoEAtosDaUeDto> enderecosEAtos)
        {
            if (enderecosEAtos.Any())
            {
                var cabecalho = new CabecalhoDto();
                cabecalho.NomeUe = enderecosEAtos?.FirstOrDefault()?.NomeUe;
                cabecalho.Endereco = enderecosEAtos?.FirstOrDefault()?.Endereco;
                cabecalho.AtoCriacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "1")?.Atos;
                cabecalho.AtoAutorizacao = enderecosEAtos?.FirstOrDefault(teste => teste.TipoOcorrencia == "7")?.Atos;
                return cabecalho;
            }
            else return default;
        }

        private async Task<IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>>> ObterComponentesCurricularesTurmasRelatorio(string[] turmaCodigo, string codigoUe, Modalidade modalidade, Usuario usuario)
        {
            return await mediator.Send(new ObterComponentesCurricularesTurmasRelatorioBoletimQuery()
            {
                CodigosTurma = turmaCodigo,
                CodigoUe = codigoUe,
                Modalidade = modalidade,
                Usuario = usuario
            });
        }

        private async Task<IEnumerable<IGrouping<string, NotasAlunoBimestre>>> ObterNotasAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterNotasRelatorioBoletimQuery()
            {
                CodigosAlunos = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<IEnumerable<IGrouping<string, FrequenciaAluno>>> ObterFrequenciasAlunos(string[] turmasCodigo, string[] alunosCodigo)
        {
            return await mediator.Send(new ObterFrequenciasRelatorioBoletimQuery()
            {
                CodigosAluno = alunosCodigo,
                CodigosTurma = turmasCodigo
            });
        }

        private async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            return await mediator.Send(new ObterParametrosMediaFrequenciaQuery());
        }
    }
}
