using Marketplace.Api.Features.Admin.Attributes.Create;
using Marketplace.Api.Features.Admin.Attributes.Update;
using Marketplace.Api.Features.Admin.Categories.Create;
using Marketplace.Api.Features.Admin.Categories.Update;
using Marketplace.Api.Features.Admin.Sellers.Create;
using Marketplace.Api.Features.Admin.Sellers.Update;
using Marketplace.Api.Features.Admin.SubCategories.Create;
using Marketplace.Api.Features.Admin.SubCategories.Update;
using Marketplace.Api.Features.Admin.Users.Create;
using Marketplace.Api.Features.Admin.Users.Update;

namespace Marketplace.Api.Features.Admin.Shared;

public static class AdminValidationPolicy
{
    public static Dictionary<string, string[]> ValidateUser(CreateUserRequest request, bool passwordRequired)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        AddRequired(errors, "login", request.Login, "Login obrigatorio.");

        if (passwordRequired)
        {
            AddRequired(errors, "password", request.Password, "Senha obrigatoria.");
        }

        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateUser(UpdateUserRequest request, bool passwordRequired)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        AddRequired(errors, "login", request.Login, "Login obrigatorio.");

        if (passwordRequired)
        {
            AddRequired(errors, "password", request.Password, "Senha obrigatoria.");
        }

        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateSeller(CreateSellerRequest request, bool passwordRequired)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        AddRequired(errors, "login", request.Login, "Login obrigatorio.");

        if (passwordRequired)
        {
            AddRequired(errors, "password", request.Password, "Senha obrigatoria.");
        }

        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateSeller(UpdateSellerRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateCategory(CreateCategoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "title", request.Title, "Titulo obrigatorio.");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateCategory(UpdateCategoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "title", request.Title, "Titulo obrigatorio.");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateSubCategory(CreateSubCategoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "title", request.Title, "Titulo obrigatorio.");
        if (request.CategoryId <= 0)
        {
            errors["categoryId"] = ["Categoria obrigatoria."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateSubCategory(UpdateSubCategoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "title", request.Title, "Titulo obrigatorio.");
        if (request.CategoryId <= 0)
        {
            errors["categoryId"] = ["Categoria obrigatoria."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateAttribute(CreateAttributeRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateAttribute(UpdateAttributeRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        return errors;
    }

    public static string? NormalizeDocument(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }

    private static void AddRequired(Dictionary<string, string[]> errors, string key, string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[key] = [message];
        }
    }

    private static void ValidateDocument(Dictionary<string, string[]> errors, string? value, string key)
    {
        var normalized = NormalizeDocument(value);
        if (!string.IsNullOrWhiteSpace(normalized) && normalized.Length != 11)
        {
            errors[key] = ["CPF deve conter 11 digitos."];
        }
    }
}
