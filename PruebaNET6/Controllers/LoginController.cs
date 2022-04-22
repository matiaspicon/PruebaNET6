using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrameworkMVC.Security;
using FrameworkMVC.Login;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace PruebaNET6.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]        
        public IActionResult Login(string ID)
        {

            WCFUsuarioLogeado? user = null;

            if (!User.Identity.IsAuthenticated)
            {
                user = LoginService.LoginUniversalCallback(ID);

                if (user == null)
                {
                    return Redirect(LoginService.GetURLLoginUniversal());
                }
            }


            if (user != null)
            {               

                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                // El lo que luego obtendré al acceder a User.Identity.Name
                identity.AddClaim(new Claim(ClaimTypes.Name, user.NombreUsuario));

                // Se utilizará para la autorización por roles
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Grupos.GrupoDescripcion));

                // Lo utilizaremos para acceder al Id del usuario que se encuentra en el sistema.
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                // Lo utilizaremos cuando querramos mostrar el nombre del usuario logueado en el sistema.
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.NombrePersona + user.ApellidoPersona));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // En este paso se hace el login del usuario al sistema
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

                return RedirectToAction(nameof(HomeController.Index), "Home");
                
            }            

            return View();
        }
    }
}
