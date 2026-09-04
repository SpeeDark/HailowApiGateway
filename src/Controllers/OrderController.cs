using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using HailowApiGateway.Attributes;
using Microsoft.AspNetCore.Mvc;
using HailowApiGateway.Protos.AuthService;
using HailowApiGateway.Services;
using HailowApiGateway.DTOs.Auth;

namespace HailowApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController: ControllerBase
{
    
}