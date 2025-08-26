using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentationController : ControllerBase
{
    [HttpGet("user-service-endpoints")]
    [AllowAnonymous]
    public IActionResult GetUserServiceEndpoints()
    {
        return Ok(new
        {
            Service = "User Service Endpoints",
            Description = "These endpoints are routed through the API Gateway to the User Service",
            Endpoints = new object[]
            {
                new
                {
                    Method = "POST",
                    Path = "/api/auth/register",
                    Description = "Register a new user",
                    Body = new
                    {
                        Email = "string",
                        Password = "string",
                        FirstName = "string",
                        LastName = "string",
                        DateOfBirth = "DateTime?",
                        Gender = "Gender?",
                        PhoneNumber = "string?"
                    }
                },
                new
                {
                    Method = "POST",
                    Path = "/api/auth/login",
                    Description = "Authenticate user",
                    Body = new
                    {
                        Email = "string",
                        Password = "string"
                    }
                },
                new
                {
                    Method = "GET",
                    Path = "/api/users/profile",
                    Description = "Get user profile",
                    Authorization = "Bearer token required"
                },
                new
                {
                    Method = "PUT",
                    Path = "/api/users/profile",
                    Description = "Update user profile",
                    Authorization = "Bearer token required",
                    Body = new
                    {
                        FirstName = "string?",
                        LastName = "string?",
                        DateOfBirth = "DateTime?",
                        Gender = "Gender?",
                        PhoneNumber = "string?"
                    }
                },
                new
                {
                    Method = "PUT",
                    Path = "/api/users/password",
                    Description = "Change user password",
                    Authorization = "Bearer token required",
                    Body = new
                    {
                        CurrentPassword = "string",
                        NewPassword = "string"
                    }
                },
                new
                {
                    Method = "GET",
                    Path = "/api/addresses",
                    Description = "Get user addresses",
                    Authorization = "Bearer token required"
                },
                new
                {
                    Method = "POST",
                    Path = "/api/addresses",
                    Description = "Create new address",
                    Authorization = "Bearer token required",
                    Body = new
                    {
                        Type = "AddressType",
                        Street = "string",
                        City = "string",
                        State = "string",
                        PostalCode = "string",
                        Country = "string",
                        IsDefault = "bool"
                    }
                },
                new
                {
                    Method = "GET",
                    Path = "/api/admin/users",
                    Description = "Get all users (Admin only)",
                    Authorization = "Bearer token with Admin role required"
                }
            }
        });
    }
}
