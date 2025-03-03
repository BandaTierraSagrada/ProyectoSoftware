using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalonDeBelleza.src.models;
using System.Threading.Tasks;

public class UsuariosController : Controller
{
    private readonly UserManager<Usuario> _userManager;

    public UsuariosController(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    // Crear usuario
    [HttpPost]
    public async Task<IActionResult> Create(Usuario usuario, string password)
    {
        var result = await _userManager.CreateAsync(usuario, password);
        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }
        // Manejar errores
        return View(usuario);
    }

    // Leer usuarios
    public IActionResult Index()
    {
        // Lógica para obtener y mostrar usuarios
        return View();
    }

    // Actualizar usuario
    public async Task<IActionResult> Edit(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        return View(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Usuario usuario)
    {
        var result = await _userManager.UpdateAsync(usuario);
        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }
        // Manejar errores
        return View(usuario);
    }

    // Eliminar usuario
    public async Task<IActionResult> Delete(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        await _userManager.DeleteAsync(usuario);
        return RedirectToAction("Index");
    }
}