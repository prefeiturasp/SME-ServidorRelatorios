using MediatR;
using SME.SR.Application.Services.Codaf;
using SME.SR.Data.Interfaces.Conecta;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Codaf;
using SME.SR.Infra.Dtos.Relatorios.Conecta;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.Conecta.GerarPlanilhaCodaf
{
    public class GerarPlanilhaCodafCommandHandler : IRequestHandler<GerarPlanilhaCodafCommand, byte[]>
    {
        private readonly IRelatorioCodafRepository _repository;
        private readonly IGeradorRelatorioCodafService _geradorRelatorioCodafService;

        public GerarPlanilhaCodafCommandHandler(IRelatorioCodafRepository repository, IGeradorRelatorioCodafService geradorRelatorioCodafService)
        {
            _repository = repository;
            _geradorRelatorioCodafService = geradorRelatorioCodafService;
        }
        public async Task<byte[]> Handle(GerarPlanilhaCodafCommand request, CancellationToken cancellationToken)
        {
            var dadosBrutoRelatorio = await _repository.ObterDadosRelatorioAsync(request.CodafId);

            if (dadosBrutoRelatorio == null)
                throw new NegocioException("Nenhuma informação encontrada para o codaf informado.");

            var relatorioDto = MapearParaDtoEstruturado(dadosBrutoRelatorio);

            var streamExcel = _geradorRelatorioCodafService.GerarRelatorio(relatorioDto);
            var fileBytes = streamExcel.ToArray();
            return fileBytes;
        }

        private static RelatorioCodafDto MapearParaDtoEstruturado(DadosPrincipaisRelatorioCodafDto dadosBruto)
        {
            var tipoFormacao = dadosBruto.TipoFormacao switch
            {
                TipoFormacaoConecta.Curso => TipoFormacaoRelatorioCodaf.Curso,
                TipoFormacaoConecta.Evento => TipoFormacaoRelatorioCodaf.Evento,
                _ => TipoFormacaoRelatorioCodaf.NaoInformado
            };
            var modalidade = dadosBruto.TipoFormato switch
            {
                TipoFormatoConecta.Presencial => ModalidadeRelatorioCodaf.Presencial,
                TipoFormatoConecta.Distancia => ModalidadeRelatorioCodaf.Distancia,
                TipoFormatoConecta.Hibrido => ModalidadeRelatorioCodaf.Hibrido,
                _ => ModalidadeRelatorioCodaf.NaoInformado
            };

            var previaInscritosSme = new PreviaInscritosRelatorioCodafDto
            {
                TotalInscritos = dadosBruto.Participantes.Count(p => p.TemRf),
                TotalAprovados = dadosBruto.Participantes.Count(p => p.TemRf && p.Aprovado),
                TotalReprovados = dadosBruto.Participantes.Count(p => p.TemRf && !p.Aprovado)
            };

            var previaInscritosSemRf = new PreviaInscritosRelatorioCodafDto
            {
                TotalInscritos = dadosBruto.Participantes.Count(p => !p.TemRf),
                TotalAprovados = dadosBruto.Participantes.Count(p => !p.TemRf && p.Aprovado),
                TotalReprovados = dadosBruto.Participantes.Count(p => !p.TemRf && !p.Aprovado)
            };

            var numeroSequencial = 0;

            var alunosAprovadosMunicipal = new GrupoAlunosRelatorioCodafDto
            {
                TituloBloco = "PARTICIPANTES APROVADOS",
                EhRedeParceira = false,
                Alunos = MapearAlunos(dadosBruto.Participantes.Where(p => p.TemRf && p.Aprovado).ToList(), ref numeroSequencial)
            };

            var alunosAprovadosParceira = new GrupoAlunosRelatorioCodafDto
            {
                EhRedeParceira = true,
                Alunos = MapearAlunos(dadosBruto.Participantes.Where(p => !p.TemRf && p.Aprovado).ToList(), ref numeroSequencial)
            };

            var alunosReprovadosMunicipal = new GrupoAlunosRelatorioCodafDto
            {
                TituloBloco = "PARTICIPANTES DESISTENTES E REPROVADOS",
                EhRedeParceira = false,
                Alunos = MapearAlunos(dadosBruto.Participantes.Where(p => p.TemRf && !p.Aprovado).ToList(), ref numeroSequencial)
            };

            var alunosReprovadosParceira = new GrupoAlunosRelatorioCodafDto
            {
                EhRedeParceira = true,
                Alunos = MapearAlunos(dadosBruto.Participantes.Where(p => !p.TemRf && !p.Aprovado).ToList(), ref numeroSequencial)
            };

            var turma = new TurmaRelatorioCodafDto
            {
                NomeTurma = dadosBruto.NomeTurma,
                Cabecalho = new CabecalhoRelatorioCodafDto
                {
                    AreaPromotora = dadosBruto.NomeAreaPromotora,
                    TipoFormacao = tipoFormacao,
                    NomeFormacao = dadosBruto.NomeFormacao,
                    QuantidadeTurmas = dadosBruto.QuantidadeTurmas,
                    DataPeriodoRealizacaoInicio = dadosBruto.PeriodoRealizacoInicio,
                    DataPeriodoRealizacaoFim = dadosBruto.PeriodoRealizacoFim,
                    TipoCertificacao = dadosBruto.CursoComCertificado ? TipoCertificacaoRelatorioCodaf.ComCertificacao : TipoCertificacaoRelatorioCodaf.SemCertificacao,
                    NumeroHomologacao = dadosBruto.NumeroHomologacao,
                    CodigoEventoSigpec = dadosBruto.CodigoEventoSigpec,
                    CargaHorariaTotal = dadosBruto.CargaHorariaTotal,
                    CargaHorariaDistancia = dadosBruto.CargaHorariaDistancia.ConverterHoraMinutoParaInteiro(),
                    CargaHorariaPresencial = dadosBruto.CargaHorariaPresencial.ConverterHoraMinutoParaInteiro() +
                                             dadosBruto.CargaHorariaSincrona.ConverterHoraMinutoParaInteiro(),
                    Modalidade = modalidade,
                    NumeroComunicado = dadosBruto.NumeroComunicado,
                    DataComunicado = dadosBruto.DataPublicacao,
                    DataPublicacaoDom = dadosBruto.DataPublicacaoDom,
                    PaginaDom = dadosBruto.PaginaComunicadoDom,
                    PreviaInscritosSme = previaInscritosSme,
                    PreviaInscritosSemRf = previaInscritosSemRf,
                    NomeTurma = dadosBruto.NomeTurma,
                    NumeroVagas = dadosBruto.QuantidadeVagasTurma,
                    NomeDre = dadosBruto.NomeDre,
                    Observacao = dadosBruto.Observacao,
                    DataDasAulasSincronas = ExpandirDataAulas(dadosBruto.DataAulas),
                    Retificacoes = dadosBruto.Retificacoes.Select(r => new RetificacaoRelatorioCodafDto
                    {
                        Data = r.Data,
                        NumeroPagina = r.Pagina
                    }).ToList()
                },
                RegentesDaTurma = dadosBruto.RegentesTurma.Select(r => new RegenteTurmaRelatorioCodafDto
                {
                    NomeRegente = r.Nome,
                    RfRegente = r.RegistroFuncional,
                    NumeroRegistro = r.NumeroRegistro
                }).ToList(),
                AlunosAprovadosMunicipal = alunosAprovadosMunicipal,
                AlunosAprovadosParceira = alunosAprovadosParceira,
                AlunosReprovadosMunicipal = alunosReprovadosMunicipal,
                AlunosReprovadosParceira = alunosReprovadosParceira
            };

            var relatorioDto = new RelatorioCodafDto
            {
                Turmas = new List<TurmaRelatorioCodafDto> { turma }
            };
            return relatorioDto;
        }

        private static List<AlunoRelatorioCodafDto> MapearAlunos(List<DadosParticipanteRelatorioCodafDto> participantes, ref int numeroSequencial)
        {
            var alunos = new List<AlunoRelatorioCodafDto>();

            foreach (var participante in participantes)
            {
                var aluno = new AlunoRelatorioCodafDto
                {
                    NumeroSequencial = ++numeroSequencial,
                    NomeAluno = participante.Nome,
                    DocumentoAluno = participante.Documento,
                    PercentualFrequencia = (int)participante.PercentualFrequencia,
                    AtividadeObrigatoria = participante.AtividadeObrigatoria,
                    ConceitoFinal = participante.ConceitoFinal,
                    CodigoCertificado = participante.CodigoCertificado
                };
                alunos.Add(aluno);
            }

            return alunos;
        }

        private static List<DateTime> ExpandirDataAulas(IEnumerable<DataAulaTurmaRelatorioCodafDto> periodos)
        {
            if (periodos == null || !periodos.Any())
                return new List<DateTime>();
            var datasExpandidas = new List<DateTime>();

            foreach (var periodo in periodos)
            {
                if (!periodo.DataFim.HasValue || periodo.DataInicio.Date == periodo.DataFim.Value.Date)
                {
                    datasExpandidas.Add(periodo.DataInicio.Date);
                    continue;
                }

                var dataInicio = periodo.DataInicio;
                var dataFim = periodo.DataFim;
                for (var date = dataInicio; date <= dataFim; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        continue;
                    datasExpandidas.Add(date);
                }
            }
            return datasExpandidas
                .Distinct()
                .OrderBy(d => d)
                .ToList();
        }
    }
}