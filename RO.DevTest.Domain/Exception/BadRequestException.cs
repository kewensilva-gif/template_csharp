using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace RO.DevTest.Domain.Exception;

public class BadRequestException : ApiException
{
    public object? ErrorDetails { get; }
    
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(IdentityResult result) 
        : base("Falha na operação de identidade")
    {
        ErrorDetails = result.Errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray());
    }

    public BadRequestException(ValidationResult validationResult) 
        : base("Um ou mais erros de validação ocorreram")
    {
        ErrorDetails = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
    }
}