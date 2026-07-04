using MediatR;
using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Domain.Aggregates;
using MediSync.Auth.Domain.Interfaces;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.BuildingBlocks.Common;
using System.Net;

namespace MediSync.Auth.Application.Commands.RegisterUser;

/// <summary>
/// Handles the user registration process by creating both the domain user
/// and the corresponding identity user.
/// </summary>
public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    // Provides access to user data.
    private readonly IUserRepository _userRepository;

    // Manages authentication user creation.
    private readonly IIdentityService _identityService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserHandler"/> class.
    /// </summary>
    /// <param name="userRepository">Provides user data operations.</param>
    /// <param name="identityService">Provides identity management operations.</param>
    public RegisterUserHandler(IUserRepository userRepository, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
    }


    /// <summary>
    /// Registers a new user by creating the domain user and the corresponding
    /// identity user.
    /// </summary>
    /// <param name="request">Contains the registration details.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    /// Returns the newly created user's identifier when the registration succeeds;
    /// otherwise returns a failure result.
    /// </returns>
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Ensure that the email address is not already registered.
        var emailExists = await _userRepository.ExistsByEmailAsync(request.email, cancellationToken);

        if (emailExists is true)
        {
            return Result<Guid>.Failure(Error.Conflict(HttpStatusCode.Conflict.ToString(), "This email address is already registered. Please use a different email or sign in."));
        }

        // Create the appropriate domain user based on the selected role.
        var user = request.role switch
        {
            UserRole.Doctor => User.CreateDoctor(request.firstName, request.lastName, request.email),
            UserRole.Patient => User.CreatePatient(request.firstName, request.lastName, request.email),
            UserRole.Admin => User.CreateAdmin(request.firstName, request.lastName, request.email),
            UserRole.LabTechnician => User.CreateLabTechnician(request.firstName, request.lastName, request.email),
            UserRole.Pharmacist => User.CreatePharmacist(request.firstName, request.lastName, request.email),
            _ => throw new InvalidOperationException("Invalid user role")
        };

        // Save the domain user.
        await _userRepository.AddAsync(user, cancellationToken);

        // Create the authentication user in ASP.NET Identity.
        IdentityCreationResult identityCreationResult = await _identityService.CreateUserAsync(user.Id, request.firstName, request.lastName, request.email, request.password, request.role.ToString(), cancellationToken);

        // Remove the domain user if the identity user could not be created.
        if (!identityCreationResult.Succeeded)
        {
            await _userRepository.DeleteAsync(user, cancellationToken);

            return Result<Guid>.Failure(Error.Failure(identityCreationResult?.ErrorCode!, "Unable to create the account. Please try again."));
        }

        // Return the identifier of the newly registered user.
        return Result<Guid>.Success(user.Id);
    }
}