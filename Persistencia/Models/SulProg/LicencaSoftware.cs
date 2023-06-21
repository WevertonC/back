using Persistencia.Models.Enums;
using System;

namespace Persistencia.Models.SulProg
{
    public class LicencaSoftware
    {
        public int Id { get; set; }

        public string NumeroSerie { get; set; }

        public int ClienteId { get; set; }

        public virtual ClienteSulProg Cliente { get; set; }

        public int NumeroBases { get; set; } // Limite de Bases a serem criadas pela licença

        public int NumeroUsuariosSimultaneos { get; set; } // Limite de Usuários Simultâneos que podem acessar o sistema Wsolução

        public StatusLicenca StatusLicenca { get; set; } // Ativo, Cancelada

        public DateTime DataContrato { get; set; } // Data do Contrato

        public DateTime? DataCancelamento { get; set; } // Preenchido Quando Status = Cancelado

        public string MotivoCancelamento { get; set; } // Preenchido Quando Status = Cancelado

        public DateTime DataRegistro { get; set; } // Data/Hora em que o registro foi criado

        public bool ImplantadaNoCliente { get; set; } // True se a licença já está ativa (sendo executada) no cliente
    }
}
