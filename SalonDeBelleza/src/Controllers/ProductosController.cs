using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;
using System;
namespace SalonDeBelleza.src.Controllers
{

    [Route("api/inventario")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los productos
        [HttpGet("productos")]
        public async Task<ActionResult<IEnumerable<Producto>>> ObtenerProductos()
        {
            var productos = await _context.Productos.ToListAsync();
            return Ok(productos);
        }

        // Crear un nuevo producto
        [HttpPost("crearProducto")]
        public async Task<ActionResult> CrearProducto([FromBody] Producto producto)
        {
            if (producto == null)
            {
                return BadRequest();
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return Ok(producto);
        }

        // Actualizar el stock de un producto
        [HttpPost("actualizarStock/{id}")]
        public async Task<IActionResult> ActualizarStock(int id, [FromQuery] int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            producto.Stock += cantidad;

            // Notificar al administrador si el stock es menor al mínimo
            if (producto.Stock <= producto.StockMinimo)
            {
                // await _notificacionService.EnviarNotificacionAdministrador("El stock de " + producto.Nombre + " ha alcanzado el nivel mínimo.");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Eliminar un producto
        [HttpDelete("eliminarProducto/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Obtener categorías (puedes agregar categorías predeterminadas)
        [HttpGet("categorias")]
        public ActionResult<IEnumerable<string>> ObtenerCategorias()
        {
            var categorias = new List<string> { "Bebida", "Ropa", "Barra" };
            return Ok(categorias);
        }

        // Obtener un producto por su ID
        [HttpGet("producto/{id}")]
        public async Task<ActionResult<Producto>> ObtenerProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }
        

        // Actualizar un producto
        [HttpPut("actualizarProducto/{id}")]
        public async Task<ActionResult> ActualizarProducto(int id, Producto productoActualizado)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            producto.Nombre = productoActualizado.Nombre;
            producto.Descripcion = productoActualizado.Descripcion;
            producto.Stock = productoActualizado.Stock;
            producto.StockMinimo = productoActualizado.StockMinimo;
            producto.Categoria = productoActualizado.Categoria;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return NoContent(); // Respuesta exitosa pero sin contenido (por convención)
        }

    }


}
