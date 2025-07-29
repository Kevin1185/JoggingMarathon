using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure.Models.SearchModels.Account;
using Jogging.Persistence.Context;
using Jogging.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;

namespace Jogging.Infrastructure.Repositories.MysqlRepos
{
    public class AuthRepo : IAuthenticationRepo
    {
        private readonly JoggingContext _dbJoggingContext;
        private readonly IPersonRepo _personRepo;
        private readonly IMapper _mapper;


        public AuthRepo(JoggingContext joggingContext, IPersonRepo personRepo, IMapper mapper)
        {
            _dbJoggingContext = joggingContext;
            _personRepo = personRepo;
            _mapper = mapper;
        }

        public async Task<PersonDom> SignInAsync(string email, string password)
        {
            //var user = await _dbJoggingContext.AuthUsers.FirstOrDefaultAsync(u => u.Username == email);
            //var user = await _dbJoggingContext.People.FirstOrDefaultAsync(u => u.Email == email);
            var authUser = await _dbJoggingContext.AuthUsers.FirstOrDefaultAsync(u => u.Username == email);

            if (authUser == null)
            {
                throw new AuthException("The given user information was incorrect");
            }

            var hashedPassword = HashPassword(password);

            if (authUser.Password != hashedPassword)
            {
                throw new AuthException("The given user information was incorrect");
            }

            //by guid???? which guid?
            var person = await _personRepo.GetByGuidAsync(authUser.Id.ToString());

            if (person == null)
            {
                throw new PersonException("Something went wrong while getting your account information");
            }

            return _mapper.Map<PersonDom>(person);
        }


        //TODO: CHECK AND CORRECT + ADD HASHING witg bcrypt?
        public async Task<string> SignUpAsync(string email, string? password)
        {
            // Generate a random password if not provided
            if (string.IsNullOrEmpty(password))
            {
                password = Guid.NewGuid().ToString().Substring(0, 8);
            }

            // Hash the password (ensure a hashing utility is implemented)
            var hashedPassword = HashPassword(password);

            // Create a new user
            var newUser = new AuthUser
            {
                Username = email,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            // Save the user to the database
            _dbJoggingContext.AuthUsers.Add(newUser);
            await _dbJoggingContext.SaveChangesAsync();

            // Return the generated user ID
            return newUser.Id.ToString();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public async Task ChangePassword(PasswordChangeDom passwordChangeInfo)
        {
            if (string.IsNullOrWhiteSpace(passwordChangeInfo.UserId.ToString()) ||
                string.IsNullOrWhiteSpace(passwordChangeInfo.oldPassword) ||
                string.IsNullOrWhiteSpace(passwordChangeInfo.newPassword))
            {
                throw new ArgumentException("Invalid password change information.");
            }

            string hashedOldPassword = HashPassword(passwordChangeInfo.oldPassword);
            string hashedNewPassword = HashPassword(passwordChangeInfo.newPassword);

            using var transaction = await _dbJoggingContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _dbJoggingContext.AuthUsers
                    .FirstOrDefaultAsync(u => u.Id == passwordChangeInfo.UserId);

                if (user == null)
                {
                    throw new AuthException("User not found");
                }

                if (user.Password != hashedOldPassword)
                {
                    throw new AuthException("Current password is incorrect");
                }

                user.Password = hashedNewPassword;

                _dbJoggingContext.AuthUsers.Update(user);
                await _dbJoggingContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to change the password. Please try again.", ex);
            }
        }

        public async Task<string> ResetUserConfirmToken(string email)
        {
            string confirmToken = TokenGenerator.GenerateEmailToken(email);

            try
            {
                await _dbJoggingContext.Database.ExecuteSqlRawAsync(
                    "CALL set_email_confirm_token(@confirm_token, @Email)",
                    new MySqlParameter("@confirm_token", confirmToken),
                    new MySqlParameter("@Email", email)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to set the email confirm token. Please try again.", ex);
            }

            return confirmToken;
        }

        public async Task ConfirmEmail(ConfirmTokenDom confirmTokenDom)
        {
            try
            {
                await _dbJoggingContext.Database.ExecuteSqlRawAsync(
                    "CALL confirm_email(@confirm_token)",
                    new MySqlParameter("@confirm_token", confirmTokenDom.confirm_token)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to confirm the email. Please try again.", ex);
            }
        }

        public async Task<string> ResetUserPasswordToken(string email)
        {
            string resetToken = TokenGenerator.GenerateEmailToken(email);

            try
            {
                await _dbJoggingContext.Database.ExecuteSqlRawAsync(
                    "CALL set_password_recovery_token(@recovery_token, @Email)",
                    new MySqlParameter("@recovery_token", resetToken),
                    new MySqlParameter("@Email", email)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to set the password recovery token. Please try again.", ex);
            }
            return resetToken;
        }

        public async Task CheckDuplicateEmailAddressAsync(string email)
        {
            var existingUser = await _dbJoggingContext.AuthUsers
         .FirstOrDefaultAsync(u => u.Username == email);

            if (existingUser != null)
            {
                throw new DuplicateEmailException("This email address is already registered.");
            }
        }

        public async Task RemoveUserEmailAsync(string email)
        {
            try
            {
                await _dbJoggingContext.Database.ExecuteSqlRawAsync(
                    "CALL remove_user_email(@Email)",
                    new MySqlParameter("@Email", email)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to remove the user email. Please try again.", ex);
            }
        }


        public async Task UpdateUserEmail(string oldEmail, string newEmail)
        {
            try
            {
                await _dbJoggingContext.Database.ExecuteSqlRawAsync(
                    "CALL update_user_email(@old_email_address, @new_email_address)",
                    new MySqlParameter("@old_email_address", oldEmail),
                    new MySqlParameter("@new_email_address", newEmail)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update the user email. Please try again.", ex);
            }
        }

        public async Task ResetPassword(PasswordResetDom passwordReset)
        {
            string hashedNewPassword = HashPassword(passwordReset.newPassword);

            try
            {
                await _dbJoggingContext.Database.ExecuteSqlRawAsync(
                    "CALL update_user_password_recovery_token(@recovery_token, @new_password)",
                    new MySqlParameter("@recovery_token", passwordReset.recovery_token),
                    new MySqlParameter("@new_password", hashedNewPassword)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to reset the password. Please try again.", ex);
            }
        }
    }
}