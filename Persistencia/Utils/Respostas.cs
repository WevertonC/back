using System;
using Persistencia.Models.Serviços;

namespace Persistencia.Utils
{
    public class Respostas
    {
        public static object MensagemDeErroDoModelo(string mensagens)
        {
            return new
            {
                code = "Atenção!",
                message = "O preenchimento do(s) campo(s) está incorreto!",
                detailedMessage = mensagens
            };
        }

        public static object EscritorioNaoCadastrado()
        {
            return new
            {
                code = "Atenção!",
                message = "O escritório informado não possui o módulo Web"
            };
        }

        public static object EmailNaoEncontrado()
        {
            return new
            {
                code = "Falha!",
                message = "Email não encontrado! Verifique o Email ou fale com o administrador do sistema!"
            };
        }

        public static object UsuarioInativo()
        {
            return new
            {
                code = "Falha!",
                message = "Esse usuário está inativo! Entre em contato com o administrador do sistema!"
            };
        }

        public static object Erro(string errorMessage)
        {
            return new
            {
                code = "Erro!",
                message = errorMessage,
            };
        }

        public static object Atencao(string message)
        {
            return new
            {
                message
            };
        }


        public static object Custom(string message, string type = "success", object objeto = null)
        {
            MensagemSucesso mensagemSucesso = new MensagemSucesso
            {
                Type = type,
                Message = message
            };

            return objeto is null ? mensagemSucesso.Message
            : new
            {
                objeto,
                _messages = mensagemSucesso.Message
            };
        }

        public static object Sucesso(string message, string type = "success", object objeto = null)
        {
            MensagemSucesso mensagemSucesso = new MensagemSucesso
            {
                Type = type,
                Message = message
            };

            return objeto is null ? mensagemSucesso.Message
            : new
            {
                objeto,
                _messages = mensagemSucesso.Message
            };
        }
    }
}
