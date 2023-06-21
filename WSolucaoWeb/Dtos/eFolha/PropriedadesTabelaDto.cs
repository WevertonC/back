namespace WSolucaoWeb.Dtos.eFolha
{
    public class PropriedadesTabelaDto
    {
        public PropriedadesTabelaDto()
        {
            NomeColunaOrdem = "Id";
            Ordem = "Id";
            Page = 1;
            Size = 10;
            Filtro = "";
            FiltroArray = null;
        }

        public string NomeColunaOrdem { get; set; }

        public string Ordem { get; set; }

        public int Page { get; set; }

        public int Size { get; set; }

        public string Filtro { get; set; }

        public string[] FiltroArray { get; set; }
    }
}
