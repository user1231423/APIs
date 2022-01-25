using API.Authentication.DTO.Users;
using Data.Authentication.Globalization.Errors;
using FluentValidation;

namespace API.Authentication.Validators.User
{
    /// <summary>
    /// Create User DTO validator
    /// </summary>
    public class UserDTOValidator : AbstractValidator<CreateUserDTO>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(1, 50);

            RuleFor(x => x.LastName)
                .Length(1, 50);

            RuleFor(x => x.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotNull()
                .Length(6, 50);
        }
    }
}
