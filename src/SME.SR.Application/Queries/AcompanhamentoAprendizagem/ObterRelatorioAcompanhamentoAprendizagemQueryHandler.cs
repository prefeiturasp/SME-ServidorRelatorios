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
            var acompanhamentoTurma = request.AcompanhamentoTurma;
            var frequenciaAlunos = request.FrequenciaAlunos;
            var ocorrencias = request.Ocorrencias;
            var filtro = request.Filtro;
            var quantidadeAulasDadas = request.QuantidadeAulasDadas;
            var periodoId = request.PeriodoId;

            var bimestres = ObterBimestresPorSemestre(filtro.Semestre);

            var relatorio = new RelatorioAcompanhamentoAprendizagemDto
            {
                Cabecalho = MontarCabecalho(turma, professores, filtro),
                Alunos = await MontarAlunos(acompanhamentoTurma, alunosEol, frequenciaAlunos, ocorrencias, quantidadeAulasDadas, bimestres, turma.AnoLetivo, periodoId, turma),
            };

            return relatorio;
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto MontarCabecalho(Turma turma, IEnumerable<ProfessorTitularComponenteCurricularDto> professores, FiltroRelatorioAcompanhamentoAprendizagemDto filtro)
        {
            var professoresCabecalho = "";

            if (professores != null && professores.Any())
                professoresCabecalho = professores.FirstOrDefault().NomeProfessor.Contains("Não há professor titular") ?
                    "Não há professor titular" : String.Join(", ", professores.Select(p => p.NomeProfessor).ToArray());

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

        private async Task<List<RelatorioAcompanhamentoAprendizagemAlunoDto>> MontarAlunos(IEnumerable<AcompanhamentoAprendizagemTurmaDto> acompanhamentoTurma, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas, int[] bimestres, int ano, long periodoId, Turma turma)
        {
            var alunosRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoDto>();

            var acompanhamento = acompanhamentoTurma.Count() > 0 ? acompanhamentoTurma?.First() : null;
            var quantidadeImagensParam = await mediator.Send(new ObterParametroSistemaPorTipoAnoQuery(ano, TipoParametroSistema.QuantidadeImagensPercursoTurma));
            var percursoFormatado = acompanhamento != null ? (acompanhamento.PercursoTurmaFormatado(int.Parse(quantidadeImagensParam)) ?? "") : "";

            List<AcompanhamentoAprendizagemPercursoTurmaImagemDto> percursoTurmaImagens = new List<AcompanhamentoAprendizagemPercursoTurmaImagemDto>();

            if (acompanhamento != null && acompanhamento.PercursoTurmaImagens.Count > 0)
            {
                foreach (var imagem in acompanhamento.PercursoTurmaImagens)
                {
                    var arr = imagem.Imagem.Split("/");
                    var codigo = arr[arr.Length - 1];
                    codigo = codigo.Split(".")[0];
                    var arquivo = await mediator.Send(new ObterArquivoPorCodigoQuery(Guid.Parse(codigo)));

                    var fotoBase64 = await mediator.Send(new TransformarArquivoBase64Command(arquivo));
                    if (!String.IsNullOrEmpty(fotoBase64))
                    {
                        percursoTurmaImagens.Add(new AcompanhamentoAprendizagemPercursoTurmaImagemDto
                        {
                            NomeImagem = imagem.NomeImagem,
                            Imagem = fotoBase64
                        });
                    }

                }
            }

            foreach (var alunoEol in alunosEol)
            {
                AcompanhamentoAprendizagemAlunoDto acompanhamentoAluno = null;

                if (acompanhamento != null)
                    acompanhamentoAluno = acompanhamento.Alunos.FirstOrDefault(a => long.Parse(a.AlunoCodigo) == alunoEol.AlunoCodigo);

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
                    RegistroPercursoTurma = percursoFormatado,
                    Observacoes = acompanhamentoAluno != null ? (acompanhamentoAluno.ObservacoesFormatado() ?? "") : "",
                    PercursoIndividual = acompanhamentoAluno != null ? (acompanhamentoAluno.PercursoIndividualFormatado() ?? "") : "",
                    PercursoTurmaImagens = percursoTurmaImagens
                };


                if (acompanhamentoAluno != null)
                {
                    foreach (var foto in acompanhamentoAluno.Fotos)
                    {
                        var fotoBase64 = await mediator.Send(new TransformarArquivoBase64Command(foto));
                        if (!String.IsNullOrEmpty(fotoBase64))
                        {
                            alunoRelatorio.Fotos.Add(new RelatorioAcompanhamentoAprendizagemAlunoFotoDto
                            {
                                Caminho = fotoBase64
                            });
                        }
                    }
                }

                alunoRelatorio.Frequencias = await MontarFrequencias(alunoRelatorio.CodigoEol, frequenciasAlunos, quantidadeAulasDadas, bimestres, periodoId, turma);
                alunoRelatorio.Ocorrencias = MontarOcorrencias(alunoRelatorio.CodigoEol, ocorrencias);

                alunosRelatorio.Add(alunoRelatorio);
            }

            return alunosRelatorio;
        }
        private async Task<List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>> MontarFrequencias(string alunoCodigo, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas, int[] bimestres, long periodoId, Turma turma)
        {
            var frequenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();

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

                var freqenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto
                {
                    Bimestre = $"{bimestre}º",
                    Aulas = frequenciaAluno == null ? quantidadeAulas : frequenciaAluno.TotalAulas,
                    Ausencias = frequenciaAluno == null ? 0 : frequenciaAluno.TotalAusencias,
                    Frequencia = frequenciaAluno != null ? $"{frequenciaAluno.PercentualFrequencia}%" : (turmaPossuiFrequenciaRegistrada || aulasDadas!=null) ? "100%" :"",
                };
                frequenciasRelatorio.Add(freqenciaRelatorio);
            }
            return frequenciasRelatorio;
        }

        private List<RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto> MontarRegistrosIndividuais(string alunoCodigo, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais)
        {
            var registrosIndividuaisRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto>();

            if (registrosIndividuais == null || !registrosIndividuais.Any())
                return registrosIndividuaisRelatorio;

            var registrosIndividuaisFiltrados = registrosIndividuais.Where(r => r.AlunoCodigo == long.Parse(alunoCodigo));

            if (registrosIndividuaisFiltrados == null || !registrosIndividuaisFiltrados.Any())
                return registrosIndividuaisRelatorio;

            foreach (var registro in registrosIndividuaisFiltrados.OrderByDescending(b => b.DataRegistro))
            {
                var registroIndividualRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoRegistroIndividualDto
                {
                    Data = registro.DataRelatorio,
                    Descricao = registro.RegistroFormatado(),
                };

                registrosIndividuaisRelatorio.Add(registroIndividualRelatorio);
            }
            return registrosIndividuaisRelatorio;
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
