using System;

namespace Persistencia.Models.TabelasPadroes
{
    public class Pais
    {
        public int Id { get; set; }

        public int Cod_pais { get; set; }

        public string Nom_pais { get; set; }

        public DateTime Dt_ini { get; set; }

        public DateTime? Dt_fim { get; set; }
    }
}
