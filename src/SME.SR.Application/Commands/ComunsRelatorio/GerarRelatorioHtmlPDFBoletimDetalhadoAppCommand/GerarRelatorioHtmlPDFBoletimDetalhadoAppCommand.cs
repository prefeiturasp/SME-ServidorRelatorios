﻿using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFBoletimDetalhadoAppCommand : IRequest<string>
    {
        public GerarRelatorioHtmlPDFBoletimDetalhadoAppCommand(object model, Guid codigoCorrelacao, Modalidade modalidade, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true, string mensagemDados = null)
        {
            CodigoCorrelacao = codigoCorrelacao;
            Model = model;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;
            Modalidade = modalidade;
            MensagemDados = mensagemDados;
        }

        public Guid CodigoCorrelacao { get; set; }
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public bool EnvioPorRabbit { get; set; }
        public Modalidade Modalidade { get; set; }
        public string MensagemDados { get; set; }
    }
}