using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PreparadorDeRelatorioPaginadoSondagemPorTurmaMatematica
    {
        private const int TOTAL_DE_COLUNA_ALUNO = 2;

        private RelatorioSondagemComponentesPorTurmaRelatorioDto _dtoSondagemTurma;

        public PreparadorDeRelatorioPaginadoSondagemPorTurmaMatematica(RelatorioSondagemComponentesPorTurmaRelatorioDto dtoSondagem)
        {
            this._dtoSondagemTurma = dtoSondagem;
        }

        public RelatorioPaginadoDto ObtenhaRelatorioPaginadoDto()
        {
            return new RelatorioPaginadoDto()
            {
                Cabecalho = ObtenhaCabecalho(),
                Paginas = ObtenhaPaginaDoRelatorio(),
                ViewCustomConteudo = "SondagemTurma/Conteudo.cshtml",
                ViewCustomParametroCabecalho = "SondagemTurma/Cabecalho.cshtml"
            };
        }

        private CabecalhoPaginadoDto ObtenhaCabecalho()
        {
            var sondagemCabecalho = this._dtoSondagemTurma.Cabecalho;

            return new CabecalhoPaginadoDto()
            {
                Ano = sondagemCabecalho.Ano,
                AnoLetivo = sondagemCabecalho.AnoLetivo,
                ComponenteCurricular = sondagemCabecalho.ComponenteCurricular,
                Dre = sondagemCabecalho.Dre,
                NomeRelatorio = "RELATÓRIO DE SONDAGEM",
                Periodo = sondagemCabecalho.Periodo,
                Proficiencia = sondagemCabecalho.Proficiencia,
                Rf = sondagemCabecalho.Rf,
                Turma = sondagemCabecalho.Turma,
                Ue = sondagemCabecalho.Ue,
                Usuario = sondagemCabecalho.Usuario
            };
        }

        private List<Pagina> ObtenhaPaginaDoRelatorio()
        {
            var lista = new List<Pagina>();

            lista.AddRange(ObtenhaPaginas());
            lista.AddRange(ObtenhaPaginasComGrafico());

            return lista;
        }

        private List<Pagina> ObtenhaPaginas()
        {
            var parametro = new ParametroRelatorioPaginadoPorColuna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>()
            {
                AlturaDaLinha = 40,
                TipoDePapel = TipoPapel.A4,
                UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL,
                Valores = this._dtoSondagemTurma.Planilha.Linhas
            };
            RelatorioPaginadoPorColuna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> relatorioPaginado;

            if (this._dtoSondagemTurma.Cabecalho.Ordens.Count > 0)
            {
                relatorioPaginado = new RelatorioPaginadoSubColuna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>(parametro, ObtenhaDicionarioSubColuna());
            } else
            {
                relatorioPaginado = new RelatorioPaginadoColuna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>(parametro, ObtenhaDicionarioColuna());
            }

            return relatorioPaginado.Paginas();
        }

        private List<IColuna> ObtenhaColunasChave()
        {
            var lista = new List<IColuna>();

            lista.Add(new Coluna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>() { Chave = true, Largura = 7, Propriedade = prop => prop.Aluno.Codigo, Titulo = "Cod. EOL", UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL });
            lista.Add(new Coluna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>() { Chave = true, Largura = 25, Propriedade = prop => prop.Aluno.Nome, Titulo = "Nome do Estudante", UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL });

            return lista;
        }

        private List<IColuna> ObtenhaDicionarioColuna()
        {
            var lista = new List<IColuna>();

            lista.AddRange(ObtenhaColunasChave());

            foreach (var perguntas in this._dtoSondagemTurma.Cabecalho.Perguntas)
            {
                lista.Add(new Coluna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>()
                {
                    Largura = 17,
                    Titulo = perguntas.Nome,
                    Nome = perguntas.Id.ToString(),
                    UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL
                });
            }

            return lista;
        }

        private Dictionary<SubColuna, List<IColuna>> ObtenhaDicionarioSubColuna()
        {
            var dicionario = new Dictionary<SubColuna, List<IColuna>>();

            dicionario.Add(new SubColuna() { Chave = true, ColSpan = 2 }, ObtenhaColunasChave());

            foreach(var subCabecalho in this._dtoSondagemTurma.Cabecalho.Ordens)
            {
                var perguntasOrdem = this._dtoSondagemTurma.Cabecalho.Perguntas.FindAll(pergunta => pergunta.Id == subCabecalho.Id);

                if (perguntasOrdem != null)
                {
                    var sub = new SubColuna() { Titulo = $"ORDEM {subCabecalho.Id} - {subCabecalho.Descricao}", ColSpan = perguntasOrdem.Count };
                    var lista = new List<IColuna>();

                    foreach (var perguntas in perguntasOrdem)
                    {
                        lista.Add(new Coluna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>()
                        {
                            Largura = 17,
                            Titulo = perguntas.Nome,
                            Nome = perguntas.PerguntaId,
                            UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL
                        });
                    }

                    dicionario.Add(sub, lista);
                }
            }

            return dicionario;
        }

        private List<Pagina> ObtenhaPaginasComGrafico()
        {
            var parametro = new ParametroRelatorioPaginadoGrafico()
            {
                Graficos = this._dtoSondagemTurma.GraficosBarras,
                TotalDeGraficosPorLinha = 2,
                TotalDeLinhas = 2
            };

            var relatorioPaginado = new RelatorioPaginadoGrafico(parametro);

            return relatorioPaginado.Paginas();
        }

        private int ObtenhaTotalDeColunas()
        {
            return this._dtoSondagemTurma.Cabecalho.Perguntas.Count + TOTAL_DE_COLUNA_ALUNO;
        }
    }
}
