using AutoMapper;
using EmailService;
using GrannysKitchen.API.Authorization;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;

namespace GrannysKitchen.API.Services
{
    public interface IUserService
    {
        Users GetById(int id);
        ChefUsers GetChefUserById(int id);
        ApiResponseMessage ChefRegistration(RegisterRequest model);
        ApiResponseMessage UserRegistration(RegisterRequest model);
        AuthenticateResponse UserAuthenticate(AuthenticateRequest model);
        AuthenticateResponse ChefAuthenticate(AuthenticateRequest model);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ResetPassword(ResetPasswordRequest model);
    }
    public class UserService : IUserService
    {
        private GrannysKitchenDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private IEmailSender _emailSender;

        public UserService(
            GrannysKitchenDbContext context,
            IJwtUtils jwtUtils,
            IMapper mapper,
            IEmailSender emailSender)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _emailSender = emailSender;
        }
        public Users GetById(int id)
        {
            return getUser(id);
        }
        public ChefUsers GetChefUserById(int id)
        {
            return getChefUser(id);
        }
        private Users getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
        private ChefUsers getChefUser(int id)
        {
            var user = _context.ChefUsers.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        public ApiResponseMessage ChefRegistration(RegisterRequest model)
        {
            ApiResponseMessage response = new ApiResponseMessage();
            // validate
             if (_context.ChefUsers.Any(x => x.Username == model.Username))
            {
                response.ErrorMessage = "Username '" + model.Username + "' is already taken";
                response.IsSuccess = false;
                return response;
            }


            // map model to new user object
            var user = _mapper.Map<ChefUsers>(model);

            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save user
            _context.ChefUsers.Add(user);
            _context.SaveChanges();
            response.SuccessMessage = "Registration successful";
            response.IsSuccess = true;
            return response;
        }
        public AuthenticateResponse ChefAuthenticate(AuthenticateRequest model)
        {
            var user = _context.ChefUsers.Where(x => x.IsActive == true).SingleOrDefault(x => x.Username == model.Username);
            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var authenticateResponse = new AuthenticateResponse()
                {
                    ErrorMessage = "Username or password is incorrect",
                    ResponseMesssage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                };
                return authenticateResponse;
            }
            // authentication successful
            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = _jwtUtils.GenerateToken(user);
            response.ResponseMesssage = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }
        public ApiResponseMessage UserRegistration(RegisterRequest model)
        {
            ApiResponseMessage response = new ApiResponseMessage();
            // validate
            if (_context.Users.Any(x => x.Username == model.Username))
            {
                response.ErrorMessage = "Username '" + model.Username + "' is already taken";
                response.IsSuccess = false;
                return response;
            }
            // map model to new user object
            var user = _mapper.Map<Users>(model);
            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            // save user
            _context.Users.Add(user);
            _context.SaveChanges();
            response.SuccessMessage = "Registration successful";
            response.IsSuccess = true;
            return response;
        }
        public AuthenticateResponse UserAuthenticate(AuthenticateRequest model)
        {
            var user = _context.Users.Where(x => x.IsActive == true).SingleOrDefault(x => x.Username == model.Username);
            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var authenticateResponse = new AuthenticateResponse()
                {
                    ErrorMessage = "Username or password is incorrect",
                    ResponseMesssage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                };
                return authenticateResponse;
            }
            // authentication successful
            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = _jwtUtils.GenerateToken(user);
            response.ResponseMesssage = new HttpResponseMessage(HttpStatusCode.OK);
            return response;
        }
        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            ChefUsers chefUser = null;
            Users user = null;
            if (model.IsChefUser)
            {
                chefUser = _context.ChefUsers.SingleOrDefault(x => x.Email == model.Email);
            }
            else
            {
                user = _context.Users.SingleOrDefault(x => x.Email == model.Email);
            }
            // always return ok response to prevent email enumeration
            if (chefUser == null && user == null) return;
            // create reset token that expires after 1 day
            var resetToken = generateResetToken();
            ResetPasswordTokens resetPasswordToken = new ResetPasswordTokens
            {
                IsChefUser = model.IsChefUser,
                IsActive = true,
                ResetToken = resetToken,
                UserId = model.IsChefUser ? chefUser.Id : user.Id,
                EmailId = model.Email,
                CreatedDate = DateTime.UtcNow,
                ResetTokenExpires = DateTime.UtcNow.AddHours(6)
            };
            _context.ResetPasswordTokens.Add(resetPasswordToken);
            _context.SaveChanges();
            //// send email
            sendPasswordResetEmail(resetPasswordToken, origin);
        }
        private string generateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            // ensure token is unique by checking against db
            var tokenIsUnique = !_context.ResetPasswordTokens.Any(x => x.ResetToken == token);
            if (!tokenIsUnique)
                return generateResetToken();
            return token;
        }
        private void sendPasswordResetEmail(ResetPasswordTokens account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/resetpassword?PasswordResetToken={account.ResetToken}";
                message = $@"<p>Please use the below token to reset your password with the <code>/resetpassword</code> api route:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/resetpassword</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
            }
            var contentMessage = new Message(new string[] { "nutakki.shivaramakrishna99@gmail.com" }, "Sign-up Verification API - Reset Password", $@"<h4>Reset Password Email</h4>{message}");
            _emailSender.SendEmail(contentMessage, true);
        }
        public void ResetPassword(ResetPasswordRequest model)
        {
            var resetPasswordToken = GetUserByResetToken(model.PasswordResetToken);
            ChefUsers chefUser;
            Users user;
            if (resetPasswordToken != null && resetPasswordToken.IsChefUser)
            {
                chefUser = GetChefUserById(resetPasswordToken.UserId);
                chefUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                chefUser.Password = model.Password;
                _context.ChefUsers.Update(chefUser);
            }
            if (resetPasswordToken != null && !resetPasswordToken.IsChefUser)
            {
                user = GetById(resetPasswordToken.UserId);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                user.Password = model.Password;
                _context.Users.Update(user);
            }
            resetPasswordToken.IsActive = false;
            resetPasswordToken.ModifiedDate = DateTime.UtcNow;
            _context.ResetPasswordTokens.Update(resetPasswordToken);
            _context.SaveChanges();
        }
        private ResetPasswordTokens GetUserByResetToken(string token)
        {
            var resetPasswordToken = _context.ResetPasswordTokens.AsNoTracking().FirstOrDefault(x =>
                x.ResetToken == token && x.IsActive == true && x.ResetTokenExpires > DateTime.UtcNow);
            return resetPasswordToken;
        }

    }
}
