using Marketplace.Api.Modules.Attributes.Admin.Create;
using Marketplace.Api.Modules.Categories.Admin.Create;
using Marketplace.Api.Modules.Sellers.Admin.Create;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Modules.Users.Admin.Create;

namespace Marketplace.Tests;

public sealed class AdminValidationPolicyTests
{
    [Fact]
    public void User_validation_requires_core_fields()
    {
        var errors = AdminValidationPolicy.ValidateUser(new CreateUserRequest(string.Empty, string.Empty, string.Empty, null, "123", "comum"), passwordRequired: true);

        Assert.Contains("name", errors.Keys);
        Assert.Contains("lastName", errors.Keys);
        Assert.Contains("login", errors.Keys);
        Assert.Contains("password", errors.Keys);
    }

    [Fact]
    public void Seller_creation_validation_requires_password_and_login()
    {
        var errors = AdminValidationPolicy.ValidateSeller(new CreateSellerRequest(string.Empty, string.Empty, string.Empty, string.Empty, null, null, null, null, null, null, null, null, null), passwordRequired: true);

        Assert.Contains("name", errors.Keys);
        Assert.Contains("lastName", errors.Keys);
        Assert.Contains("login", errors.Keys);
        Assert.Contains("password", errors.Keys);
    }

    [Fact]
    public void Category_and_attribute_validation_requires_title_and_name()
    {
        Assert.Contains("title", AdminValidationPolicy.ValidateCategory(new CreateCategoryRequest(string.Empty, null)).Keys);
        Assert.Contains("name", AdminValidationPolicy.ValidateAttribute(new CreateAttributeRequest(string.Empty)).Keys);
    }
}
