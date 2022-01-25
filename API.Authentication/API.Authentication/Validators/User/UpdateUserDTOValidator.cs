using API.Authentication.DTO.Users;
using FluentValidation;

namespace API.Authentication.Validators.User
{
    /// <summary>
    /// Update User DTO validator
    /// </summary>
    public class UpdateDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .Length(1, 50);

            RuleFor(x => x.LastName)
                .Length(1, 50);

            RuleFor(x => x.Status)
                .Must(x => x > -1 && x < 1);
        }
    }
}
