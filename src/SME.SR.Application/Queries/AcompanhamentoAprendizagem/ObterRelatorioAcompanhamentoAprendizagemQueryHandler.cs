using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQueryHandler : IRequestHandler<ObterRelatorioAcompanhamentoAprendizagemQuery, RelatorioAcompanhamentoAprendizagemDto>
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ObterRelatorioAcompanhamentoAprendizagemQueryHandler(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public Task<RelatorioAcompanhamentoAprendizagemDto> Handle(ObterRelatorioAcompanhamentoAprendizagemQuery request, CancellationToken cancellationToken)
        {
            var turma = request.Turma;
            var alunosEol = request.AlunosEol;
            var professores = request.Professores;
            var acompanhamentoTurma = request.AcompanhamentoTurma;
            var frequenciaAlunos = request.FrequenciaAlunos;
            var registrosIndividuais = request.RegistrosIndividuais;
            var ocorrencias = request.Ocorrencias;
            var filtro = request.Filtro;
            var quantidadeAulasDadas = request.QuantidadeAulasDadas;

            var bimestres = ObterBimestresPorSemestre(filtro.Semestre);

            var relatorio = new RelatorioAcompanhamentoAprendizagemDto
            {
                Cabecalho = MontarCabecalho(turma, professores, filtro),
                Alunos = MontarAlunos(acompanhamentoTurma, alunosEol, frequenciaAlunos, registrosIndividuais, ocorrencias, quantidadeAulasDadas, bimestres),
            };

            return Task.FromResult(relatorio);
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto MontarCabecalho(Turma turma, IEnumerable<ProfessorTitularComponenteCurricularDto> professores, FiltroRelatorioAcompanhamentoAprendizagemDto filtro)
        {
            var professoresCabecalho = "";

            if (professores != null && professores.Any())
                professoresCabecalho = String.Join(", ", professores.Select(p => p.NomeProfessor).ToArray());

            var cabecalho = new RelatorioAcompanhamentoAprendizagemCabecalhoDto
            {
                Dre = turma.Dre.Abreviacao,
                Ue = turma.Ue.NomeComTipoEscola,
                Turma = turma.NomeRelatorio,
                Semestre = filtro.SemestreFormatado(),
                Professores = professoresCabecalho,
            };

            return cabecalho;
        }

        private List<RelatorioAcompanhamentoAprendizagemAlunoDto> MontarAlunos(IEnumerable<AcompanhamentoAprendizagemTurmaDto> acompanhamentoTurma, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas, int[] bimestres)
        {
            var alunosRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoDto>();

            foreach (var alunoEol in alunosEol)
            {                
                var acompanhamentoAluno = acompanhamentoTurma.First().Alunos.FirstOrDefault(a => long.Parse(a.AlunoCodigo) == alunoEol.AlunoCodigo);

                var acompanhamento = acompanhamentoTurma.First();

                if (alunoEol == null)
                    throw new NegocioException("AlunoEol não encontrado");

                var alunoRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoDto();
                alunoRelatorio.NomeEol = alunoEol.NomeAluno;
                alunoRelatorio.Nome = alunoEol.NomeRelatorio;
                alunoRelatorio.DataNascimento = alunoEol.DataNascimentoFormatado();
                alunoRelatorio.CodigoEol = alunoEol.AlunoCodigo.ToString();
                alunoRelatorio.Situacao = alunoEol.SituacaoRelatorio;
                alunoRelatorio.Responsavel = alunoEol.ResponsavelFormatado();
                alunoRelatorio.Telefone = alunoEol.ResponsavelCelularFormatado();
                alunoRelatorio.RegistroPercursoTurma = acompanhamentoAluno != null ? (acompanhamento.PercursoTurmaFormatado() ?? "") : "";
                alunoRelatorio.Observacoes = acompanhamentoAluno != null ? (acompanhamentoAluno.ObservacoesFormatado() ?? "") : "";
                if (acompanhamentoAluno != null)
                    alunoRelatorio.PercursoTurmaImagens = acompanhamento.PercursoTurmaImagens;

                if (acompanhamentoAluno != null)
                {
                    foreach (var foto in acompanhamentoAluno.Fotos)
                    {
                        var fotoBase64 = ArquivoBase64(foto);
                        if (!String.IsNullOrEmpty(fotoBase64))
                        {
                            alunoRelatorio.Fotos.Add(new RelatorioAcompanhamentoAprendizagemAlunoFotoDto
                            {
                                Caminho = fotoBase64
                            });
                        }
                    }
                }

                alunoRelatorio.Frequencias = MontarFrequencias(alunoRelatorio.CodigoEol, frequenciasAlunos, quantidadeAulasDadas, bimestres);
                alunoRelatorio.RegistrosIndividuais = MontarRegistrosIndividuais(alunoRelatorio.CodigoEol, registrosIndividuais);
                alunoRelatorio.Ocorrencias = MontarOcorrencias(alunoRelatorio.CodigoEol, ocorrencias);

                alunosRelatorio.Add(alunoRelatorio);
            }
            return alunosRelatorio;
        }
        private List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto> MontarFrequencias(string alunoCodigo, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas, int[] bimestres)
        {
            var freqenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();

            foreach (var bimestre in bimestres.OrderBy(b => b))
            {
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
                    Frequencia = frequenciaAluno == null ? "100%" : $"{frequenciaAluno.PercentualFrequencia}%",
                };
                freqenciasRelatorio.Add(freqenciaRelatorio);
            }
            return freqenciasRelatorio;
        }
        private string ArquivoBase64(AcompanhamentoAprendizagemAlunoFotoDto foto)
        {
            var diretorio =  Path.Combine(variaveisAmbiente.PastaArquivosSGP, @"Arquivos/FotoAluno");

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            var nomeArquivo = $"{foto.Id}.{foto.Extensao}";
            var caminhoArquivo = Path.Combine(diretorio, nomeArquivo);
            if (!File.Exists(caminhoArquivo))
                return "";

            var arquivo = File.ReadAllBytes(caminhoArquivo);
            return $"data:{foto.TipoArquivo};base64,{Convert.ToBase64String(arquivo)}";
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
