using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoAprendizagemQuery, RelatorioAcompanhamentoAprendizagemDto>
    {
        private readonly IMediator mediator;
        public ObterRelatorioAcompanhamentoAprendizagemQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioAcompanhamentoAprendizagemDto> Handle(ObterRelatorioAcompanhamentoAprendizagemQuery request, CancellationToken cancellationToken)
        {
            var turma = request.Turma;
            var alunosEol = request.AlunosEol;
            var professores = request.Professores;
            var acompanhamentoAlunos = request.AcompanhamentoTurma;
            var frequenciaAlunos = request.FrequenciaAlunos;
            var ocorrencias = request.Ocorrencias;
            var filtro = request.Filtro;
            var quantidadeAulasDadas = request.QuantidadeAulasDadas;
            var periodoId = request.PeriodoId;

            var bimestres = ObterBimestresPorSemestre(filtro.Semestre);

            var relatorio = new RelatorioAcompanhamentoAprendizagemDto
            {
                Cabecalho = MontarCabecalho(turma, professores, filtro),
                Alunos = await MontarAlunos(acompanhamentoAlunos, alunosEol, frequenciaAlunos, ocorrencias, quantidadeAulasDadas, bimestres, turma.AnoLetivo, periodoId, turma,request.RelatorioEscolaAqui),
            };

            return relatorio;
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto MontarCabecalho(Turma turma, IEnumerable<ProfessorTitularComponenteCurricularDto> professores, FiltroRelatorioAcompanhamentoAprendizagemDto filtro)
        {
            var professoresCabecalho = "";
            int quantidadeProfessoresValidosTurma = 0;

            if (professores != null && professores.Any())
            {
                quantidadeProfessoresValidosTurma = professores.Count() - professores.Where(p => p.NomeProfessor.Contains("Não há professor titular")).Count();

                professoresCabecalho = quantidadeProfessoresValidosTurma == 0 ?
                   "Não há professor titular" : String.Join(", ", professores.Where(p => !p.NomeProfessor.Contains("Não há professor titular")).Select(a => a.NomeProfessor).Distinct().ToArray());
            }  

            var cabecalho = new RelatorioAcompanhamentoAprendizagemCabecalhoDto
            {
                Dre = turma.Dre.Abreviacao,
                Ue = turma.Ue.NomeComTipoEscola,
                Turma = turma.NomeRelatorio,
                Semestre = filtro.SemestreFormatado(turma.AnoLetivo),
                Professores = professoresCabecalho,
            };

            return cabecalho;
        }

        private async Task<List<RelatorioAcompanhamentoAprendizagemAlunoDto>> MontarAlunos(IEnumerable<AcompanhamentoAprendizagemAlunoDto> acompanhamentoAlunos,
                                                                                           IEnumerable<AlunoRetornoDto> alunosEol,
                                                                                           IEnumerable<FrequenciaAluno> frequenciasAlunos,
                                                                                           IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias,
                                                                                           IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas,
                                                                                           int[] bimestres,
                                                                                           int ano,
                                                                                           long periodoId,
                                                                                           Turma turma, bool relatorioEscolaAqui)
        {
            var alunosRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoDto>();

            var acompanhamento = acompanhamentoAlunos.Any() ? acompanhamentoAlunos?.First() : null;
            var percursoFormatado = acompanhamento != null ?
                await FormatarHtml(acompanhamento.PercursoColetivoTurmaFormatado(), relatorioEscolaAqui) : "";

            foreach (var alunoEol in alunosEol)
            {
                AcompanhamentoAprendizagemAlunoDto acompanhamentoAluno = null;

                if (acompanhamento != null)
                    acompanhamentoAluno = acompanhamentoAlunos.FirstOrDefault(a => long.Parse(a.AlunoCodigo) == alunoEol.AlunoCodigo);

                if (alunoEol == null)
                    throw new NegocioException("AlunoEol não encontrado");

                var alunoRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoDto
                {
                    NomeEol = alunoEol.NomeAluno,
                    Nome = alunoEol.NomeRelatorio,
                    DataNascimento = alunoEol.DataNascimentoFormatado(),
                    CodigoEol = alunoEol.AlunoCodigo.ToString(),
                    Situacao = alunoEol.SituacaoRelatorio,
                    Responsavel = alunoEol.ResponsavelFormatado(),
                    Telefone = alunoEol.ResponsavelCelularFormatado(),
                    PercursoColetivoTurma = percursoFormatado,
                    Observacoes = acompanhamentoAluno != null ? await FormatarHtml(acompanhamentoAluno.Observacoes, relatorioEscolaAqui) : "",
                    PercursoIndividual = acompanhamentoAluno != null ? await FormatarHtml(acompanhamentoAluno.PercursoIndividual, relatorioEscolaAqui) : "",
                    ModalidadeTurma = turma.ModalidadeCodigo
                };

                alunoRelatorio.Frequencias = await MontarFrequencias(alunoRelatorio.CodigoEol, frequenciasAlunos, quantidadeAulasDadas, bimestres, periodoId, turma);
                alunoRelatorio.Ocorrencias = MontarOcorrencias(alunoRelatorio.CodigoEol, ocorrencias);

                alunosRelatorio.Add(alunoRelatorio);
            }

            return alunosRelatorio;
        }

        private async Task<string> FormatarHtml(string html, bool relatorioEscolaAqui)
        {
            if (relatorioEscolaAqui)
                return await mediator.Send(new ObterHtmlComImagensBase64Query(html, escalaHorizontal: 360f, escalaVertical: 640f));
            else
                return await mediator.Send(new ObterHtmlComImagensBase64Query(html));
        }

        private async Task<List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>> MontarFrequencias(string alunoCodigo, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas, int[] bimestres, long periodoId, Turma turma)
        {
            var frequenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();

            if (turma.ModalidadeCodigo == Modalidade.Infantil)
            {
                int totalAulasInfantil = 0;
                string valorFrequencia = string.Empty;
                int totalAusencias = 0;
                int totalCompensacoes = 0;
                int quantidadeAulas = 0;
                
                foreach (var bimestre in bimestres.OrderBy(b => b))
                {
                    var frequenciaAluno = frequenciasAlunos?.FirstOrDefault(f => f.CodigoAluno == alunoCodigo && f.Bimestre == bimestre);
                    var aulasDadas = quantidadeAulasDadas.FirstOrDefault(a => a.Bimestre == bimestre);

                    quantidadeAulas = quantidadeAulasDadas.Count() == 0 ?
                        0 :
                        aulasDadas != null ?
                        aulasDadas.Quantidade :
                        0;

                    totalAulasInfantil += frequenciaAluno == null ? quantidadeAulas : frequenciaAluno.TotalAulas;
                    totalAusencias += frequenciaAluno == null ? 0 : frequenciaAluno.TotalAusencias;
                    totalCompensacoes += frequenciaAluno == null ? 0 : frequenciaAluno.TotalCompensacoes;
                }
                var frequenciaAlunoPercentualFrequencia = new FrequenciaAluno() 
                { 
                    TotalAulas = totalAulasInfantil,
                    TotalAusencias = totalAusencias,
                    TotalCompensacoes = totalCompensacoes
                };
                
                valorFrequencia = frequenciaAlunoPercentualFrequencia.PercentualFrequenciaFormatado;

                var frequenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto
                {
                    Bimestre = "",
                    Aulas = totalAulasInfantil,
                    Ausencias = totalAusencias,
                    Frequencia = $"{valorFrequencia}%",
                };
                frequenciasRelatorio.Add(frequenciaRelatorio);

            }
            else
            {
                foreach (var bimestre in bimestres.OrderBy(b => b))
                {
                    var turmaPossuiFrequenciaRegistrada = await mediator.Send(new ExisteFrequenciaRegistradaPorTurmaComponenteCurricularQuery(turma.Codigo, String.Empty, periodoId, new int[] { bimestre }));
                    var frequenciaAluno = frequenciasAlunos?.FirstOrDefault(f => f.CodigoAluno == alunoCodigo && f.Bimestre == bimestre);
                    var aulasDadas = quantidadeAulasDadas.FirstOrDefault(a => a.Bimestre == bimestre);

                    var quantidadeAulas = quantidadeAulasDadas.Count() == 0 ?
                        0 :
                        aulasDadas != null ?
                        aulasDadas.Quantidade :
                        0;

                    var frequenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto
                    {
                        Bimestre = $"{bimestre}º",
                        Aulas = frequenciaAluno == null ? quantidadeAulas : frequenciaAluno.TotalAulas,
                        Ausencias = frequenciaAluno == null ? 0 : frequenciaAluno.TotalAusencias,
                        Frequencia = frequenciaAluno != null && (frequenciaAluno.TotalAulas > 0 || quantidadeAulas > 0) ? $"{frequenciaAluno.PercentualFrequenciaFormatado}%" : (turmaPossuiFrequenciaRegistrada || aulasDadas != null) ? "100.00%" : string.Empty,
                    };
                    frequenciasRelatorio.Add(frequenciaRelatorio);
                }
            }

            return frequenciasRelatorio;
        }

        private List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto> MontarOcorrencias(string alunoCodigo, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias)
        {
            var ocorrenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto>();

            if (ocorrencias == null || !ocorrencias.Any())
                return ocorrenciasRelatorio;

            var ocorrenciasFiltradas = ocorrencias.Where(r => r.AlunoCodigo == long.Parse(alunoCodigo));

            if (ocorrenciasFiltradas == null || !ocorrenciasFiltradas.Any())
                return ocorrenciasRelatorio;

            foreach (var ocorrencia in ocorrenciasFiltradas.OrderByDescending(o => o.DataOcorrencia))
            {
                var ocorrenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoOcorrenciaDto
                {
                    Data = ocorrencia.DataRelatorio(),
                    Tipo = ocorrencia.TipoOcorrencia,
                    Descricao = ocorrencia.DescricaoFormatada(),
                    Titulo = ocorrencia.TituloOcorrencia,
                };
                ocorrenciasRelatorio.Add(ocorrenciaRelatorio);
            }
            return ocorrenciasRelatorio;
        }

        private static int[] ObterBimestresPorSemestre(int semestre)
        {
            if (semestre == 1)
                return new int[] { 1, 2 };
            else return new int[] { 3, 4 };
        }
    }
}
