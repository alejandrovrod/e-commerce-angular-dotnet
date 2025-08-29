namespace ECommerce.BuildingBlocks.Common.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public BusinessException(string message, string code = "BUSINESS_ERROR", int statusCode = 400) 
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public BusinessException(string message, Exception innerException, string code = "BUSINESS_ERROR", int statusCode = 400) 
        : base(message, innerException)
    {
        Code = code;
        StatusCode = statusCode;
    }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string message) 
        : base(message, "NOT_FOUND", 404)
    {
    }

    public NotFoundException(string entity, object id) 
        : base($"{entity} with id '{id}' was not found", "NOT_FOUND", 404)
    {
    }
}

public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message = "Unauthorized access") 
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}

public class ForbiddenException : BusinessException
{
    public ForbiddenException(string message = "Access forbidden") 
        : base(message, "FORBIDDEN", 403)
    {
    }
}

public class ValidationException : BusinessException
{
    public Dictionary<string, string[]> ValidationErrors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation errors occurred", "VALIDATION_ERROR", 422)
    {
        ValidationErrors = errors;
    }

    public ValidationException(string field, string error) 
        : this(new Dictionary<string, string[]> { { field, new[] { error } } })
    {
    }
}

public class DuplicateException : BusinessException
{
    public DuplicateException(string message) 
        : base(message, "DUPLICATE", 409)
    {
    }

    public DuplicateException(string entity, string field, object value) 
        : base($"{entity} with {field} '{value}' already exists", "DUPLICATE", 409)
    {
    }
}








