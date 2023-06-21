using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Persistencia.Utils
{
    public class SulprogUtil
    {
        private readonly IConfiguration _configuration;
        public SulprogUtil(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<bool> VerificarSeBaseDeDadosJaExiste(string numeroDeSerieEscritorio)
        {
            // Faz conexão com o banco de dados postgres

            string stringDeConexao;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                stringDeConexao = Environment.GetEnvironmentVariable("ConnectionDesenvolvimento");
            }
            else
            {
                stringDeConexao = Environment.GetEnvironmentVariable("ConnectionProducao");
            }
            await using var conn = new NpgsqlConnection(string.Format(stringDeConexao, "postgres"));
            try
            {
                await conn.OpenAsync();

                // Faz uma consulta para verificar se a base de dados ja esta criada
                string scriptSql = $"select datname::varchar from pg_database where datistemplate = false and datname::varchar = \'escritorio{numeroDeSerieEscritorio}\'";
                await using var cmd = new NpgsqlCommand(scriptSql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                    return true;
                else
                    return false;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }
    }

    public class CustomValidationCPFCNPJAttribute : ValidationAttribute
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public CustomValidationCPFCNPJAttribute()
        {
        }
        /// <summary>
        /// Validação server
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;

            return IsCpf(value.ToString()) || IsCnpj(value.ToString());
        }


        // <summary>
        /// Remove caracteres não numéricos
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            //for (int j = 0; j < 10; j++)
            //if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
            //return false;

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        private static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }

    public class ValidarEstado : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string strValue = value as string;

            if (!string.IsNullOrEmpty(strValue))
            {
                return new string[]
                {
                "AC", "AL", "AP", "AM", "BA",
                "CE", "DF", "ES", "GO", "MA",
                "MT", "MS", "MG", "PA", "PB",
                "PR", "PE", "PI", "RJ", "RN",
                "RS", "RO", "RR", "SC", "SP",
                "SE", "TO"
                }.Contains(strValue.ToUpper());
            }
            return true;
        }

    }
}
