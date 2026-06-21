namespace MediSync.Web.ViewModels;

public class LoginViewModel
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public bool RememberMe { get; set; } = false;

    // Where to redirect after successful login — security best practice
    public string? ReturnUrl { get; set; }
}
