using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoAprendizagemUseCase : IRelatorioAcompanhamentoAprendizagemUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoAprendizagemUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto filtro)
        {
            await GerarRelatorioMock();
            return;

            var parametros = filtro.ObterObjetoFiltro<FiltroRelatorioAcompanhamentoAprendizagemDto>();

            var turma = await mediator.Send(new ObterComDreUePorTurmaIdQuery(parametros.TurmaId));

            if (turma == null)
                throw new NegocioException("Turma não encontrada");

            var turmaCodigo = new string[1] { turma.Codigo };
            var professores = await mediator.Send(new ObterProfessorTitularComponenteCurricularPorTurmaQuery(turmaCodigo));

            var alunosEol = await mediator.Send(new ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(turma.Codigo, parametros.AlunoCodigo, turma.AnoLetivo));
            if (alunosEol == null || !alunosEol.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var acompanhmentosAlunos = await mediator.Send(new ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), parametros.Semestre));

            var bimestres = ObterBimestresPorSemestre(parametros.Semestre);

            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre));

            var periodoInicioFim = await ObterInicioFimPeriodo(tipoCalendarioId, bimestres, parametros.Semestre);

            var quantidadeAulasDadas = await mediator.Send(new ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery(turma.Codigo, tipoCalendarioId, bimestres));

            var frequenciaAlunos = await mediator.Send(new ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), bimestres));


            var ocorrencias = await mediator.Send(new ObterOcorenciasPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo, periodoInicioFim.DataInicio, periodoInicioFim.DataFim));

            var relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(turma, alunosEol, professores, acompanhmentosAlunos, frequenciaAlunos, ocorrencias, parametros, quantidadeAulasDadas, periodoInicioFim.Id));

            await mediator.Send(new GerarRelatorioHtmlPDFAcompAprendizagemCommand(relatorioDto, filtro.CodigoCorrelacao));
        }

        private async Task GerarRelatorioMock()
        {
            var relatorioDto = new RelatorioAcompanhamentoAprendizagemDto();

            relatorioDto.Cabecalho = new RelatorioAcompanhamentoAprendizagemCabecalhoDto()
            {
                Dre = "DRE - JT",
                Ue = "CEU CEMEI - NOVO MUNDO",
                Turma = "EI - 5A",
                Professores = "MARA APARECIDA RAMIRES DOS SANTOS, JACIRA KUSTER COELHO",
            };

            relatorioDto.Alunos = new System.Collections.Generic.List<RelatorioAcompanhamentoAprendizagemAlunoDto>()
            {
                new RelatorioAcompanhamentoAprendizagemAlunoDto()
                {
                    DataNascimento = "06/10/2016",
                    CodigoEol = "707145",
                    Situacao = "REMATRICULADO EM 19/10/2021",
                    Responsavel = "ISABEL ARTEAGA FLORES (FILIAÇÃO 1)",
                    Telefone = "(11) 96269-6485 (Atualizado - 14/01/2021)",
                    Nome = "Nº24 - AGATHA HAIANNE VASQUEZ ARTEAGA",
                    NomeEol = "AGATHA HAIANNE VASQUEZ ARTEAGA",
                    Frequencias = new System.Collections.Generic.List<RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto>()
                    {
                        new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto() { Bimestre = "1º", Aulas = 36, Ausencias = 0, Frequencia = "100%" },
                        new RelatorioAcompanhamentoAprendizagemAlunoFrequenciaDto() { Bimestre = "2º", Aulas = 79, Ausencias = 23, Frequencia = "70.89%" }
                    },
                    PercursoColetivoTurma = "<p><span style='font-size: 18px;'><br>REGISTRO DO PERCURSO PEDAGÓGICO EM TEMPOS DE PANDEMIA – 2021<br>Esse registro tem a intenção de apresentar o percurso pedagógico desenvolvido durante o primeiro semestre de 2021 a todas as famílias. O objetivo principal do acompanhamento individual dos bebês e das crianças é registrar a trajetória de suas aprendizagens e de seu desenvolvimento, revelando suas conquistas, fazendo anotações ao longo da semana sobre as contribuições e evoluções de cada bebê e criança. Como esse seguimento não foi possível, optamos por relatar as nossas experiências e vivências planejadas durante o período de pandemia, assim como a nossa organização e planejamento para continuar atendendo a todas as famílias.<br>Em 2021 o nosso ano escolar continuou diferente do esperado e do planejado. Começamos com grandes expectativas em relação ao novo prédio e à nova unidade escolar, à comunidade que seria atendida e ao aprendizado dos nossos pequenos. Porém, em decorrência da pandemia e também das condições organizacionais do prédio, não tivemos condições de iniciar o atendimento dos trinta e cinco por cento recomendados pelos protocolos sanitários. Então desde o mês de fevereiro todas as nossas ações pedagógicas ocorreram por meio do trabalho remoto (Facebook e Google sala de aula).<br>Ainda em fevereiro enfrentamos diversas situações que perduraram por vários meses, como a falta de linha telefônica e internet, falta de materiais essenciais para o atendimento presencial, além de uma grande dificuldade de conseguir contato com todas as famílias, partindo de todas essas premissas nosso primeiro desafio foi a acolhida e a nossa adaptação com a realidade e o ambiente escolar, organizamos nossa rotina de trabalho a distância, por meio de reuniões nas plataformas Teams e Google meet, construímos e alinhamos nossa carta pedagógica individual à coletiva, discutimos sobre a construção e elaboração do Projeto Político Pedagógico e sobre o que almejávamos para as crianças durante este ano.<br>Como pensaríamos os processos de aprendizagens pautados na ludicidade e na brincadeira, como linguagem primeira da infância sem o contato, sem a proximidade e sem as tão necessárias interações e trocas que vemos ocorrer no dia-a-dia das crianças, nos espaços escolares voltados para as infâncias e que o atual Currículo da Educação Infantil defende.<br>Pensamos em estratégias, que facilitassem o nosso contato com as famílias, utilizando o Facebook e o Google Sala de Aula como um instrumento de comunicação, para assim proporcionarmos interações entre a família e a escola, amenizando o afastamento e contribuindo com orientações educativas.  As nossas postagens ocorrem semanalmente no Google e no Facebook. O nosso planejamento tem como objetivo:<br>✔	Criar vínculos e manter contato com as famílias, procurando tranquilizar a todos mediante a situação da pandemia, orientando para os cuidados necessários, seguindo as orientações da Organização Mundial da Saúde;<br>✔	Considerar com cautela o tempo, as condições do ambiente e a situação econômica que a família atendida pode estar passando em casa;<br>✔	Apresentar vídeos com as imagens dos profissionais para que os bebês e as crianças tenham contato e conheçam seus educadores;<br>✔	Criar e indicar atividades que envolvam brincadeiras, utilizando recursos existentes em casa e sugerindo experiências nos diferentes momentos da rotina familiar;<br>Mas, mesmo com muitos esforços e dedicação, não tínhamos um retorno favorável diante das nossas propostas. Então no mês de maio o grupo de professores em reunião com a gestão optou por criar e utilizar os grupos da plataforma WhatsApp para assim tentarmos estar mais próximos das famílias. Iniciamos um movimento de busca ativa, contatando os responsáveis, inserindo-os nos grupos e criando uma relação de parceria entre professores e famílias, este foi um movimento que tem surtido muitos efeitos positivos, pois através dos grupos começamos a ter mais devolutivas positivas sobre nosso trabalho e estamos podendo de alguma maneira acompanhar o desenvolvimento das crianças, durante os diálogos percebemos que as maiores dificuldades encontram-se em recursos tecnológicos, questões de vulnerabilidade e falta de tempo.<br>A partir do Decreto nº 59.283/2020, que declara situação de emergência no Município de São Paulo e define outras medidas para o enfrentamento da pandemia decorrente do corona vírus, as ações pedagógicas sofreram adaptações em razão do novo contexto social. A Secretaria Municipal de Educação de São Paulo e a Diretoria Regional de Educação junto com o CEMEI, estiveram em constante movimentação no sentido de orientar as famílias/ responsáveis para que os bebês e as crianças continuassem recebendo atenção tanto afetiva quanto educativa. Com a intenção de ampliar as possibilidades de brincadeiras e interações, foi enviado o material Trilhas de Aprendizagens para a residência de bebês e crianças. A Secretaria Municipal de Educação, definiu a Plataforma Google Sala de Aula (Classroom) como meio oficial para postagens de propostas de vivências a serem realizadas pelas famílias com seus bebês e crianças, e nós incluímos as mesmas atividades do Classroom no Facebook do CEMEI Novo Mundo. Posteriormente, começamos a colocar nos grupos de WhatsApp dos agrupamentos. Muitas famílias relatam, via WhatsApp ou telefone, que encontram dificuldades para esse acompanhamento virtual, mesmo através do Facebook, por conta de gasto com dados móveis do celular. As postagens consistem em propostas de vivências, brincadeiras, leituras, cuidados, memórias, todas em consonância com o Currículo da Cidade e Material Trilhas de Aprendizagens mesclando com vídeos informativos sobre cuidados necessários para prevenção do Corona Vírus. Sabendo que não é apropriado incentivar que bebês e crianças fiquem expostos às telas, smartphones, computadores decidimos que as nossas propostas, sempre que possível, seriam destinadas às famílias, a fim de orientar aos adultos sobre o que poderiam realizar e propor aos pequenos no ambiente de suas casas.<br>Relatório de acompanhamento<br>Tendo por base o decreto de nº 59.283 de 16 de março de 2020 declara a situação de emergência no município de São Paulo, define outras medidas para o enfrentamento da pandemia decorrente do coronavírus, colocando hoje conforme a mídia veicula uma necessidade cada vez mais acentuada com a vida; com a não proliferação do vírus como diz Edgar Morin “A vida é uma navegação em um oceano de incertezas”, estamos vivendo isso e o caminho para alterarmos esse quadro inicialmente são os cuidados com a higienização e o distanciamento.<br>Os professores da rede fizeram curso no sentido de multiplicar os cuidados focando no protocolo como também se abriu a oportunidade para mães da comunidade serem contratadas pela prefeitura para reforçar esse apoio preventivo (mães do pot). Muitas reuniões foram realizadas pontuando os entraves observados na Unidade como falta de telefone, internet, lavanderia entre outros e elaboramos um manifesto o qual foi enviado à Diretoria Regional com vistas a normalizar essa questão tão urgente e de interesse de todos; direcionando também nosso olhar aos pequenos... o que preciso pensar para eles nessa nova realidade que se desenha?<br>Como foi ventilado na carta de intenções há urgência do acolhimento de forma segura e responsável e para tanto acolhemos o encaminhamento dos órgãos superiores de funcionamento de forma parcial, priorizando os mais vulneráveis com distanciamento e máscara, exceto berçários por precisar de lavanderia e esta não encontra-se em funcionamento.<br>As proposições tinham um cunho humano e emocional, aliado aos documentos, agora a família como protagonista de novas práticas; sugestões são enviadas às famílias pelo Google Sala de Aula, WhatsApp e são solicitadas às mesmas devolutivas de como foi o processo qualificando o brincar em casa, intensificando a parceria. Houve também envio de cesta básica da Prefeitura aos mais vulneráveis e agora um segundo momento que veio contemplar a todos com uma cesta saudável de alimentos, frutas e legumes.<br>A gestão tem se desdobrado muitíssimo para dar conta dos desafios, chagando a ir à casa das famílias levar cesta, benefícios em geral e se colocando ao lado da comunidade, dando-lhe voz e acolhendo sugestões.<br>O nosso percurso está sendo construído, como também nosso PPP, com reflexões e redirecionamentos conforme as experiências vão acontecendo, respeitando o direito da criança, como diz Paulo Freire... “a gente vai sendo no mundo e com os outros”. A criança está em processo de construção, auxiliemo-la!</span><br></p>",
                    PercursoIndividual = @"<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif; text-align: center;' align='center'><strong><span style='font-size: 19px; line-height: 107%;'>DIRETORIA REGIONAL DE EDUCA&Ccedil;&Atilde;O CAMPO LIMPO</span></strong></p>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif; text-align: center;' align='center'><strong><span style='font-size: 19px; line-height: 107%;'>2021</span></strong></p>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif; text-align: center;' align='center'><strong style='mso-bidi-font-weight: normal;'><u><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>RELAT&Oacute;RIO DESCRITIVO DE AVALIA&Ccedil;&Atilde;O INDIVIDUAL DA CRIAN&Ccedil;A &ndash; 1&deg; SEMESTRE/2021</span></u></strong></p>
<table class='MsoTableGrid' border='1' cellspacing='0' cellpadding='0'>
<tbody>
<tr style='mso-yfti-irow: 0; mso-yfti-firstrow: yes;'>
<td style='width: 177px; padding: 0px 7px;' valign='top' width='266'>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif;'><strong style='mso-bidi-font-weight: normal;'><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>NOME DA CRIAN&Ccedil;A</span></strong></p>
</td>
<td style='width: 528px; padding: 0px 7px;' valign='top' width='795'>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif;'><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>Wesley Eduardo Candido Santana</span></p>
</td>
</tr>
<tr style='mso-yfti-irow: 1;'>
<td style='width: 177px; padding: 0px 7px;' valign='top' width='266'>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif;'><strong style='mso-bidi-font-weight: normal;'><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>TURMA</span></strong></p>
</td>
<td style='width: 528px; padding: 0px 7px;' valign='top' width='795'>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif;'><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>5 H</span></p>
</td>
</tr>
<tr style='mso-yfti-irow: 2; mso-yfti-lastrow: yes;'>
<td style='width: 177px; padding: 0px 7px;' valign='top' width='266'>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif;'><strong style='mso-bidi-font-weight: normal;'><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>PROFESSOR</span></strong></p>
</td>
<td style='width: 528px; padding: 0px 7px;' valign='top' width='795'>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif;'><span style='font-size: 16px; line-height: 107%; font-family: Arial, sans-serif;'>PROFESSORAS IONE / PALOMA</span></p>
</td>
</tr>
</tbody>
</table>
<p class='MsoNormal' style='margin: 0px 0px 11px; line-height: 107%; font-size: 15px; font-family: Calibri, sans-serif; text-align: center;' align='center'>&nbsp;</p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'> Isabella &eacute; uma crian&ccedil;a bastante tranquila e segura e sua adapta&ccedil;&atilde;o se deu de forma tranquila, logo interagiu com todos os colegas e professoras estabelecendo v&iacute;nculos afetivos.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-indent: 36pt; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Se concentra na hora da hist&oacute;ria, participando e ouvindo com aten&ccedil;&atilde;o e entusiasmo, participa sempre com depoimentos pertinentes aos assuntos. No in&iacute;cio do ano apresentava uma certa timidez para se colocar, mas aos poucos foi se sentindo segura.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Em suas produ&ccedil;&otilde;es de registro &eacute; f&aacute;cil notar a organiza&ccedil;&atilde;o espacial e clareza nas &ldquo;ideias&rdquo; revelando que entende as propostas da atividade. J&aacute; identifica seu nome e o escreve sem ajuda de crach&aacute;, come&ccedil;a a identificar as letras em nossas produ&ccedil;&otilde;es de textos coletivos. Em suas produ&ccedil;&otilde;es de escrita espont&acirc;nea &eacute; comum v&ecirc;-la tentando escrever os nomes das colegas, da professora e at&eacute; mesmo letras do alfabeto. Gosta muito de escrever na lousa e &agrave;s vezes auxilia as outras crian&ccedil;as no tra&ccedil;ado quando est&atilde;o fazendo desenhos espont&acirc;neos.&nbsp;&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Em uma conversa com os colegas, respondeu prontamente a d&uacute;vida de uma crian&ccedil;a sobre quando chegaria a sexta-feira e disse: &ldquo;</span><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: italic; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Primeiro hoje, da&iacute; chega amanh&atilde; e a&iacute; sim depois de amanh&atilde;</span><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>&rdquo; com essa afirma&ccedil;&atilde;o revela compreender a passagem do tempo em dias.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>&Eacute; bem organizada com seus pertences e com os materiais, nota-se sempre uma organiza&ccedil;&atilde;o no &ldquo;pote&rdquo;, no uso do caderno, na mochila.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Notamos algumas dificuldades na oralidade quanto &agrave; dic&ccedil;&atilde;o. Orientamos que a fam&iacute;lia se atente a essa necessidade para solicitar uma avalia&ccedil;&atilde;o do pediatra e poss&iacute;vel encaminhamento para o fonoaudi&oacute;logo.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Na atividade de pesquisa se saiu muito bem e conseguiu socializar com os colegas da turma.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'><span style='border: none; display: inline-block; overflow: hidden; width: 277px; height: 276px;'><img style='margin-left: 0px; margin-top: 0px;' src='https://lh5.googleusercontent.com/VAsk6-BExvfGpoK6lkUXDd0pA8H1Z-NhDLl7APN9Ens_iAmP4irkI0w6D3dB27Dr5IRB8iz5igAMOdBLj4HxoA0cRZT4FwvHjcszAT_-1RrcV68_ujMDajlSpgeVZXjuwDPVjvxl=s1600' width='277' height='276' /></span></span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Em mais uma proposta de artes e dan&ccedil;a ficou encantada com a produ&ccedil;&atilde;o do &ldquo;balangand&atilde;&rdquo; e o que foi capaz de fazer com o corpo e com o brinquedo.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'><span style='border: none; display: inline-block; overflow: hidden; width: 270px; height: 268px;'><img style='margin-left: 0px; margin-top: 0px;' src='https://lh5.googleusercontent.com/8PZZSzIKaGG6GoBeq9if-j5tkyF-eoJBiPRrYseyZsbkCDNtFajf_O_DVbr4ll9_XjqBaYpedEZHj2-448JvkguUUZ2o4k4LSFF3_CrHB-B35L-dh_huDaP27j0VX_Q0hia7w5xy=s1600' width='270' height='268' /></span></span></p>
<p><strong style='font-weight: normal;'>&nbsp;</strong></p>
<p dir='ltr' style='line-height: 1.38; text-indent: 36pt; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>Desde o in&iacute;cio compreendeu bem o protocolo de higiene, lava corretamente as m&atilde;os, faz troca de m&aacute;scaras com seguran&ccedil;a e a fam&iacute;lia colabora com esse processo sempre mandando as m&aacute;scaras limpas para a troca. E como qualquer outra crian&ccedil;a precisa de orienta&ccedil;&atilde;o constante quanto aos distanciamentos.</span></p>
<p dir='ltr' style='line-height: 1.38; text-indent: 36pt; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>A fam&iacute;lia est&aacute; sempre atenta aos recados da agenda e as necessidades da Isabella, sempre que necess&aacute;rio comunica a escola.&nbsp;</span></p>
<p dir='ltr' style='line-height: 1.38; text-indent: 36pt; text-align: justify; margin-top: 12pt; margin-bottom: 12pt;'><span style='font-size: 12pt; font-family: Arial; color: #000000; background-color: transparent; font-weight: 400; font-style: normal; font-variant: normal; text-decoration: none; vertical-align: baseline; white-space: pre-wrap;'>A aluna &eacute; bem ass&iacute;dua.&nbsp;</span></p>
<p>&nbsp;</p>"
                }
            };

            await mediator.Send(new GerarRelatorioHtmlPDFAcompAprendizagemCommand(relatorioDto, Guid.NewGuid()));
        }

        private async Task<PeriodoEscolarDto> ObterInicioFimPeriodo(long tipoCalendarioId, int[] bimestres, int semestre)
        {
            var periodosEscolares = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));
            int ano = periodosEscolares.FirstOrDefault().PeriodoInicio.Year;

            if (semestre == 1)
            {
                var periodoEscolar = periodosEscolares.FirstOrDefault(p => p.Bimestre == bimestres.Last());
                return new PeriodoEscolarDto()
                {
                    Id = periodoEscolar.Id,
                    DataInicio = new DateTime(ano, 1, 1),
                    DataFim = periodoEscolar.PeriodoFim
                };
            }
            else
            {
                var periodoEscolar = periodosEscolares.FirstOrDefault(p => p.Bimestre == bimestres.First());
                return new PeriodoEscolarDto()
                {
                    Id = periodoEscolar.Id,
                    DataInicio = periodoEscolar.PeriodoInicio,
                    DataFim = new DateTime(ano, 12, 31)
                };
            }
        }

        private static int[] ObterBimestresPorSemestre(int semestre)
        {
            if (semestre == 1)
                return new int[] { 1, 2 };
            else return new int[] { 3, 4 };
        }
    }
}
