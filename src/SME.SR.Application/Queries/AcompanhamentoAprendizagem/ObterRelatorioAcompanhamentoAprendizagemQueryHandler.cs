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
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioAcompanhamentoAprendizagemDto> Handle(ObterRelatorioAcompanhamentoAprendizagemQuery request, CancellationToken cancellationToken)
        {

            var alunosEol = request.AlunosEol;
            var professores = request.Professores;
            var acompanhmentosAlunos = request.AcompanhamentosAlunos;
            var frequenciaAlunos = request.FrequenciaAlunos;
            var registrosIndividuais = request.RegistrosIndividuais;
            var ocorrencias = request.Ocorrencias;

            var relatorio = new RelatorioAcompanhamentoAprendizagemDto
            {
                Cabecalho = MontarCabecalho(acompanhmentosAlunos.FirstOrDefault(), professores),
                Alunos = MontarAlunos(acompanhmentosAlunos, alunosEol, frequenciaAlunos, registrosIndividuais, ocorrencias),
            };            

            return relatorio;            
        }

        private RelatorioAcompanhamentoAprendizagemCabecalhoDto MontarCabecalho(AcompanhamentoAprendizagemAlunoRetornoDto acompanhamentoAluno, IEnumerable<ProfessorTitularComponenteCurricularDto> professores)
        {
            var professoresCabecalho = "";

            if (professores != null && professores.Any())
                professoresCabecalho = String.Join(", ", professores.Select(p => p.NomeProfessor).ToArray());

            var cabecalho = new RelatorioAcompanhamentoAprendizagemCabecalhoDto
            {
                Dre = acompanhamentoAluno.DreAbreviacao,
                Ue = acompanhamentoAluno.UeNomeFormatado(),
                Turma = acompanhamentoAluno.TurmaNome,
                Semestre = acompanhamentoAluno.SemestreFormatado(),
                Professores = professoresCabecalho,
            };

            return cabecalho;
        }

        private List<RelatorioAcompanhamentoAprendizagemAlunoDto> MontarAlunos(IEnumerable<AcompanhamentoAprendizagemAlunoRetornoDto> alunosAcompanhamento, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<FrequenciaAluno> frequenciasAlunos, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> Ocorrencias)
        {
            var alunosRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoDto>();           

            foreach (var aluno in alunosAcompanhamento)
            {                
                var alunoEol = alunosEol.FirstOrDefault(a => a.AlunoCodigo == long.Parse(aluno.AlunoCodigo));

                if (alunoEol == null)
                    throw new NegocioException("AlunoEol não encontrado");

                var alunoRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoDto();
                alunoRelatorio.Nome = alunoEol.NomeRelatorio;
                alunoRelatorio.DataNascimento = alunoEol.DataNascimentoFormatado();
                alunoRelatorio.CodigoEol = alunoEol.AlunoCodigo.ToString();
                alunoRelatorio.Situacao = alunoEol.SituacaoRelatorio;
                alunoRelatorio.Responsavel = alunoEol.ResponsavelFormatado();
                alunoRelatorio.Telefone = alunoEol.ResponsavelCelularFormatado();
                alunoRelatorio.RegistroPercursoTurma = aluno.PercusoTurmaFormatado();
                alunoRelatorio.Observacoes = aluno.ObservacoesFormatado();

                // TODO : Verificar como recuperar o caminho da foto
                foreach (var foto in aluno.Fotos)
                {
                    alunoRelatorio.Fotos.Add(new RelatorioAcompanhamentoAprendizagemAlunoFotoDto{ Caminho = "https://media.gazetadopovo.com.br/viver-bem/2017/03/criancadocumento-600x401-ce1bce00.jpg" });
                }

                alunoRelatorio.Frequencias = MontarFrequencias(alunoRelatorio.CodigoEol, frequenciasAlunos);
                alunoRelatorio.RegistrosIndividuais = MontarRegistrosIndividuais(alunoRelatorio.CodigoEol, registrosIndividuais);
                alunoRelatorio.Ocorrencias = MontarOcorrencias(alunoRelatorio.CodigoEol, Ocorrencias);

                alunosRelatorio.Add(alunoRelatorio);
            }
            return alunosRelatorio;
        }


        private List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto> MontarFrequencias(string alunoCodigo, IEnumerable<FrequenciaAluno> frequenciasAlunos)
        {
            var freqenciasRelatorio = new List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>();           

            if (frequenciasAlunos == null || !frequenciasAlunos.Any())
                return freqenciasRelatorio;

            var frequenciasFiltrada = frequenciasAlunos.Where(f => f.CodigoAluno == alunoCodigo);

            if (frequenciasFiltrada == null || !frequenciasFiltrada.Any())
                return freqenciasRelatorio;

            foreach (var frequencia in frequenciasFiltrada.OrderBy(b => b.Bimestre))
            {
                var freqenciaRelatorio = new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto
                {
                    Bimestre = $"{frequencia.Bimestre}º",
                    Aulas = frequencia.TotalAulas,
                    Ausencias = frequencia.TotalAusencias,
                    Frequencia = $"{frequencia.PercentualFrequencia}%",
                };               

                freqenciasRelatorio.Add(freqenciaRelatorio);
            }
            return freqenciasRelatorio;
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
    }
}
