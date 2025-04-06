namespace SalonDeBelleza.src.models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }  // El nivel mínimo de stock
        public string Categoria { get; set; }  // Categoría del producto
    }
}
