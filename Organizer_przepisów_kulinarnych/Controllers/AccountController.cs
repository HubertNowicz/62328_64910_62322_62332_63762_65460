using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.Interfaces;
using Organizer_przepisów_kulinarnych.Models;
using System.Security.Claims;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using AutoMapper;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AccountController(IMapper mapper, IUserService userService)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var principal = _userService.AuthenticateUser(model.Username, model.Password);
        if (principal == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        return role == UserRole.Admin.ToString()
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = _mapper.Map<UserRegistrationDto>(model);
        var result = await _userService.RegisterUserAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage!);
            return View(model);
        }

        var principal = _userService.AuthenticateUser(model.Username, model.Password);
        if (principal == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }
}

