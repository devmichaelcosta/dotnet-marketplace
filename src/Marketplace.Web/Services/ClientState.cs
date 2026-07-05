namespace Marketplace.Web.Services;

public sealed class ClientState
{
    public event Action? Changed;

    public string? Token { get; private set; }
    public string? UserName { get; private set; }
    public string[] Roles { get; private set; } = [];
    public string CartId { get; private set; } = Guid.NewGuid().ToString("N");

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public bool IsAdmin => Roles.Contains("admin");
    public bool IsSeller => Roles.Contains("vendedor");
    public bool IsCustomer => Roles.Contains("comum");

    public AuthSnapshot Snapshot => new(Token, UserName, Roles, CartId);

    public void SignIn(string token, string userName, string[] roles)
    {
        Token = token;
        UserName = userName;
        Roles = roles;
        Changed?.Invoke();
    }

    public void Restore(AuthSnapshot snapshot)
    {
        if (string.IsNullOrWhiteSpace(snapshot.Token) || string.IsNullOrWhiteSpace(snapshot.UserName))
        {
            return;
        }

        Token = snapshot.Token;
        UserName = snapshot.UserName;
        Roles = snapshot.Roles ?? [];
        if (!string.IsNullOrWhiteSpace(snapshot.CartId))
        {
            CartId = snapshot.CartId;
        }

        Changed?.Invoke();
    }

    public void UpdateCartId(string cartId)
    {
        if (!string.IsNullOrWhiteSpace(cartId) && CartId != cartId)
        {
            CartId = cartId;
            Changed?.Invoke();
        }
    }

    public void SignOut()
    {
        Token = null;
        UserName = null;
        Roles = [];
        Changed?.Invoke();
    }

    public void NotifyChanged() => Changed?.Invoke();
}

public sealed record AuthSnapshot(string? Token, string? UserName, string[]? Roles, string? CartId);
