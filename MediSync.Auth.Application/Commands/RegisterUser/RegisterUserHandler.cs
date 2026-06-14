using MediatR;
using MediSync.Auth.Domain.Aggregates;
using MediSync.Auth.Domain.Interfaces;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.Auth.Infrastrucure.Identity;
using MediSync.BuildingBlocks.Common;
using Microsoft.AspNetCore.Identity;

namespace MediSync.Auth.Application.Commands.RegisterUser;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    // Dependencies injected via constructor — Dependency Inversion Principle
    // Handler depends on INTERFACES (IUserRepository) not implementations (UserRepository)
    // This means we can swap SQL Server for MongoDB without touching this class
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly IPasswordHasher _passwordHasher;  // interface — not BCrypt directly

    public RegisterUserHandler(IUserRepository userRepository, UserManager<ApplicationUser> userManager) //, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        // _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // ── Step 1: Check business rule — email must be unique ──────
        // This is a BUSINESS rule, not a format rule
        // The validator checked format. Handler checks business logic.
        var emailExists = await _userRepository.ExistsByEmailAsync(request.email, cancellationToken);

        if (emailExists is true)
        {
            return Result<Guid>.Failure(Error.Failure("Email already exists", "EMAIL_EXISTS"));
        }

        // ── Step 2: Hash the password BEFORE creating the domain user ─
        // Plain text password must be destroyed as soon as possible
        // The domain User never sees the plain text password
        //_passwordHasher.CreatePasswordHash(request.password, out var passwordHash, out var passwordSalt);

        // ── Step 3: Create the domain user via factory method ──────────
        // Handler calls the factory method — it does NOT use new User()
        // The factory method ensures User is always valid from birth
        // UserRegisteredEvent is raised inside CreatePatient/CreateDoctor
        var user = request.role == UserRole.Doctor
            ? User.CreateDoctor(request.firstName, request.lastName, request.email)
            : User.CreatePatient(request.firstName, request.lastName, request.email);

        // ── Step 4: Persist to database ─────────────────────────────
        // Repository handles the actual SQL — handler does not know about SQL
        await _userRepository.AddAsync(user, cancellationToken);

        var applicationUser = new ApplicationUser
        {
            UserName = request.email,
            Email = request.email,
            FirstName = request.firstName,
            LastName = request.lastName,
            Role = request.role,
            Status = UserStatus.PendingVerification,
            CreatedAt = DateTime.UtcNow,
            DomainUserId = user.Id
        };

        IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, request.password);

        if (!identityResult.Succeeded)
        {
            await _userRepository.DeleteAsync(user, cancellationToken);

            var error = identityResult.Errors.First();

            return Result<Guid>.Failure(Error.Failure(error.Code, "IDENTITY_CREATION_FAILED"));
        }

        // ── Step 5: Domain events are published by infrastructure ────
        // After SaveChanges(), the infrastructure reads DomainEvents
        // and publishes them to Azure Service Bus automatically
        // The handler does NOT manually publish events


        // ── Step 6: Return success with the new user's ID ───────────
        return Result<Guid>.Success(user.Id);
    }
}