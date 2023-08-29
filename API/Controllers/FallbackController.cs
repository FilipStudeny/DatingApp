using System.Security.Claims;
using API.Data;
using API.DTO;
using API.EXTENSIONS;
using API.LIB.HELPERS;
using API.LIB.INTERFACES;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class FallbackController : Controller
{

    public ActionResult Index(){
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
    }

}
