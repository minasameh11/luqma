using Luqma.Domain.AccountsDtos;
using Luqma.Domain.Enums;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using Luqma.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Luqma.Infrastructure.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly LuqmaContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountManager> _logger;

        public AccountManager(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration,
            LuqmaContext context,
            IEmailService emailService,
            ILogger<AccountManager> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Role == UserRole.Admin)
                return "لا يمكن التسجيل بهذه الصلاحية";

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return "هذا البريد مستخدم بالفعل";

            var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
            if (existingUserByPhone != null)
                return "رقم الهاتف هذا مستخدم بالفعل";

            ApplicationUser user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FullName = registerDto.UserName,
                Address = registerDto.Address,
                PhoneNumber = registerDto.PhoneNumber,
                CreateAt = Luqma.Domain.Helpers.EgyptTime.Now
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"خطأ في التسجيل: {errors}";
            }

            string role = registerDto.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
            await _userManager.AddToRoleAsync(user, role);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? "User")
            };

            await _userManager.AddClaimsAsync(user, claims);

            var otp = new Random().Next(100000, 999999).ToString();

            var oldOtps = await _context.EmailOtps.Where(x => x.Email == user.Email).ToListAsync();
            _context.EmailOtps.RemoveRange(oldOtps);

            await _context.EmailOtps.AddAsync(new EmailOtp
            {
                Email = user.Email,
                Code = otp,
                ExpiryTime = Luqma.Domain.Helpers.EgyptTime.Now.AddMinutes(10)
            });
            await _context.SaveChangesAsync();

            string emailBody = GetEmailTemplate("تأكيد البريد الإلكتروني", "أهلاً بك في تطبيق Luqma! لإكمال عملية التسجيل وتفعيل حسابك، يُرجى استخدام رمز التحقق التالي:", otp);
            try
            {
                await _emailService.SendEmailAsync(user.Email, "تطبيق Luqma - تأكيد البريد الإلكتروني", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register: OTP email failed for {Email}. Account kept, client should call ResendOtp.", user.Email);
                var createdOtp = await _context.EmailOtps.Where(x => x.Email == user.Email && x.Code == otp).ToListAsync();
                _context.EmailOtps.RemoveRange(createdOtp);
                await _context.SaveChangesAsync();
                return "تم إنشاء الحساب لكن فشل إرسال رمز التحقق إلى بريدك الإلكتروني، يُرجى طلب إعادة إرسال الرمز عبر ResendOtp";
            }

            return "تم التسجيل بنجاح، برجاء تأكيد البريد الإلكتروني";
        }

        public async Task<string> RegisterAdminAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return "هذا البريد مستخدم بالفعل";

            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = "Admin",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"خطأ في التسجيل: {errors}";
            }

            string adminRole = "SuperAdmin";
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
            }
            await _userManager.AddToRoleAsync(user, adminRole);

            adminRole = "Admin";
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
            }
            await _userManager.AddToRoleAsync(user, adminRole);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? "Admin")
            };

            await _userManager.AddClaimsAsync(user, claims);

            return "تم تسجيل المشرف بنجاح وتفعيل الحساب";
        }
        public async Task<AuthUserResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

          
            if (user == null || string.IsNullOrEmpty(loginDto.Password))
                return null;

            if (!user.EmailConfirmed)
            {
                throw new Exception("يرجى تأكيد البريد الإلكتروني قبل تسجيل الدخول.");
            }

            var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
            if (!isValid)
                return null;

            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(claims);

            return new AuthUserResponseDto
            {
                UserId = user.Id.ToString(),
                UserName = user.FullName ?? user.UserName,
                Role = roles.FirstOrDefault(),
                Token = token,
                ExpiresAt = Luqma.Domain.Helpers.EgyptTime.Now.AddMonths(10)
            };
        }

        public async Task<AuthUserResponseDto?> LoginAdminAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || string.IsNullOrEmpty(loginDto.Password))
                return null;

            var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
            if (!isValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin"))
            {
                 throw new UnauthorizedAccessException("غير مصرح لك بالدخول كمسئول.");
            }

            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(claims);

            return new AuthUserResponseDto
            {
                UserId = user.Id.ToString(),
                UserName = user.FullName ?? user.UserName,
                Role = roles.FirstOrDefault(),
                Token = token,
                ExpiresAt = Luqma.Domain.Helpers.EgyptTime.Now.AddMonths(10)
            };
        }
        public async Task<string> ResendOtpAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return "البريد الإلكتروني مطلوب";

                email = email.Trim();

                _logger.LogInformation(
                    "ResendOtp started for {Email}",
                    email
                );

                // Find user
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning(
                        "ResendOtp failed: User not found for {Email}",
                        email
                    );

                    return "المستخدم غير موجود";
                }

                var canonicalEmail = user.Email ?? email;

                _logger.LogInformation(
                    "ResendOtp: User found. Email={Email}, EmailConfirmed={EmailConfirmed}",
                    canonicalEmail,
                    user.EmailConfirmed
                );

                // Email already confirmed
                if (user.EmailConfirmed)
                {
                    _logger.LogWarning(
                        "ResendOtp failed: Email already confirmed for {Email}",
                        canonicalEmail
                    );

                    return "البريد الإلكتروني مؤكد بالفعل، يمكنك تسجيل الدخول";
                }

                // Remove old OTPs (case-insensitive to avoid orphans from casing differences)
                var normalizedEmail = canonicalEmail.ToLower();
                var otpOlds = await _context.EmailOtps
                    .Where(x => x.Email.ToLower() == normalizedEmail)
                    .ToListAsync();

                _logger.LogInformation(
                    "ResendOtp: Found {Count} old OTP(s) for {Email}",
                    otpOlds.Count,
                    canonicalEmail
                );

                _context.EmailOtps.RemoveRange(otpOlds);

                // Generate new OTP
                var otp = new Random()
                    .Next(100000, 999999)
                    .ToString();

                _logger.LogInformation(
                    "ResendOtp: New OTP generated for {Email}",
                    canonicalEmail
                );

                // Save new OTP (store canonical email so ConfirmEmail matches)
                await _context.EmailOtps.AddAsync(new EmailOtp
                {
                    Email = canonicalEmail,
                    Code = otp,
                    ExpiryTime = Luqma.Domain.Helpers.EgyptTime.Now.AddMinutes(10)
                });

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "ResendOtp: OTP saved successfully for {Email}",
                    canonicalEmail
                );

                // Create email body
                string emailBody = GetEmailTemplate(
                    "إعادة إرسال رمز التأكيد",
                    "لقد طلبت إعادة إرسال رمز تأكيد البريد الإلكتروني لحسابك في تطبيق Luqma. يُرجى استخدام الرمز التالي:",
                    otp
                );

                // Send email
                try
                {
                    _logger.LogInformation(
                        "ResendOtp: Sending OTP email to {Email}",
                        canonicalEmail
                    );

                    await _emailService.SendEmailAsync(
                        canonicalEmail,
                        "تطبيق Luqma - رمز تأكيد جديد",
                        emailBody
                    );

                    _logger.LogInformation(
                        "ResendOtp: OTP email sent successfully to {Email}",
                        canonicalEmail
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "ResendOtp: OTP email failed for {Email}",
                        canonicalEmail
                    );

                    // Remove OTP if email sending failed
                    var createdOtp = await _context.EmailOtps
                        .Where(x => x.Email.ToLower() == normalizedEmail && x.Code == otp)
                        .ToListAsync();

                    _context.EmailOtps.RemoveRange(createdOtp);

                    await _context.SaveChangesAsync();

                    _logger.LogWarning(
                        "ResendOtp: OTP removed after email sending failure for {Email}",
                        canonicalEmail
                    );

                    return "فشل إرسال رمز التحقق إلى بريدك الإلكتروني، تحقق من إعدادات البريد وحاول مرة أخرى";
                }

                _logger.LogInformation(
                    "ResendOtp completed successfully for {Email}",
                    canonicalEmail
                );

                return "تم إعادة إرسال رمز التحقق بنجاح";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ResendOtp unexpected error for {Email}",
                    email
                );

                return "حدث خطأ غير متوقع أثناء إعادة إرسال الرمز";
            }
        }
        public async Task<bool> ConfirmEmailAsync(string email, string otp)
        {
            email = (email ?? string.Empty).Trim();
            otp = (otp ?? string.Empty).Trim();
            var normalizedEmail = email.ToLower();
            var record = _context.EmailOtps.FirstOrDefault(x => x.Email.ToLower() == normalizedEmail && x.Code == otp);

            if (record == null || record.ExpiryTime < Luqma.Domain.Helpers.EgyptTime.Now)
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            _context.EmailOtps.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var otp = new Random().Next(100000, 999999).ToString();

            var oldOtps = await _context.EmailOtps.Where(x => x.Email == email).ToListAsync();
            _context.EmailOtps.RemoveRange(oldOtps);

            await _context.EmailOtps.AddAsync(new EmailOtp
            {
                Email = email,
                Code = otp,
                ExpiryTime = Luqma.Domain.Helpers.EgyptTime.Now.AddMinutes(10)
            });

            await _context.SaveChangesAsync();

           string emailBody = GetEmailTemplate("إعادة تعيين كلمة المرور", "لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك في تطبيق Luqma. يُرجى استخدام الرمز التالي لإكمال العملية:", otp);
            try
            {
                await _emailService.SendEmailAsync(email, "تطبيق Luqma - إعادة تعيين كلمة المرور", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RequestPasswordReset: OTP email failed for {Email}.", email);
                var createdOtp = await _context.EmailOtps.Where(x => x.Email == email && x.Code == otp).ToListAsync();
                _context.EmailOtps.RemoveRange(createdOtp);
                await _context.SaveChangesAsync();
                return false;
            }

            return true;
        }
        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var record = _context.EmailOtps
                .FirstOrDefault(x => x.Email == email && x.Code == otp);
            if (record == null || record.ExpiryTime < Luqma.Domain.Helpers.EgyptTime.Now)
                return false;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                _context.EmailOtps.Remove(record);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteRole(string Id)
        {
            if (!Guid.TryParse(Id, out Guid roleId))
                return false;

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }
        public async Task<List<UserReadDto>> GetAllUsers()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            var result = new List<UserReadDto>();

            foreach (var a in users)
            {
                var roles = await _userManager.GetRolesAsync(a);
                result.Add(new UserReadDto
                {
                    Id = a.Id.ToString(),
                    Name = a.FullName ?? a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    Address = a.Address,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    EmailConfirmed = a.EmailConfirmed,
                    IsArchived = a.IsArchived ?? false,
                    CreateAt = a.CreateAt,
                    UpdateAt = a.UpdateAt
                });
            }

            return result;
        }

        public async Task<UserReadDto?> GetUserByIdAsync(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserReadDto
            {
                Id = user.Id.ToString(),
                Name = user.FullName ?? user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = roles.FirstOrDefault() ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                IsArchived = user.IsArchived ?? false,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt
            };
        }

        public async Task<bool> UpdateAsync(UpdateAccountDto updateAccountDto)
        {
            var user = await _userManager.FindByIdAsync(updateAccountDto.Id);

            if (user == null)
                return false;

            var existingUserByPhone = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == updateAccountDto.PhoneNumber && u.Id.ToString() != updateAccountDto.Id);
            if (existingUserByPhone != null)
                return false;

            user.Email = updateAccountDto.Email;
            user.UserName = updateAccountDto.Email;
            user.FullName = updateAccountDto.UserName;
            user.Address = updateAccountDto.Address;
            user.PhoneNumber = updateAccountDto.PhoneNumber;
            
            var isUpdated = await _userManager.UpdateAsync(user);

            if (isUpdated.Succeeded)
                return true;

            return false;
        }

        public async Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(updatePasswordDto.Id);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordDto.OldPassword, updatePasswordDto.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> ArchiveAsync(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
                return false;

            user.IsArchived = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnarchiveAsync(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
                return false;

            user.IsArchived = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<UserReadDto?> GetProfileAsync(string userId)
        {
            if (!Guid.TryParse(userId, out Guid id))
                return null;

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserReadDto
            {
                Id = user.Id.ToString(),
                Name = user.FullName ?? user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = roles.FirstOrDefault() ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                IsArchived = user.IsArchived ?? false,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateAccountDto updateAccountDto)
        {
            if (!Guid.TryParse(userId, out Guid id))
                return false;

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return false;

            var existingUserByPhone = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == updateAccountDto.PhoneNumber && u.Id != id);
            if (existingUserByPhone != null)
                return false;

            user.FullName = updateAccountDto.UserName;
            user.Address = updateAccountDto.Address;
            user.PhoneNumber = updateAccountDto.PhoneNumber;
            user.UpdateAt = Luqma.Domain.Helpers.EgyptTime.Now;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public Task<bool> AssignRoleToUser(AssignRoleDto assignRoleDto)
        {
            // مُنفَّذ عبر AssignRoleToUserCommand في RolesController
            return Task.FromResult(true);
        }

        public Task<bool> CreateRole(CreateRoleDto createRoleDto)
        {
            // مُنفَّذ عبر CreateRoleCommand في RolesController
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            var isDeleted = await _userManager.DeleteAsync(user);

            if (isDeleted.Succeeded)
                return true;

            return false;
        }

        
        public Task<IEnumerable<RoleReadDto>> GetAllRoles()
        {
            // مُنفَّذ عبر GetAllRolesQuery في RolesController
            return Task.FromResult<IEnumerable<RoleReadDto>>(new List<RoleReadDto>());
        }

        
        public Task<bool> UpdateRole(UpdateRoleDto updateRoleDto)
        {
            // مُنفَّذ عبر UpdateRoleCommand في RolesController
            return Task.FromResult(true);
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return GenerateToken(claims);
        }

        public async Task<AuthUserResponseDto?> ExternalLoginAsync(string provider, string providerUserId, string? email, string? fullName)
        {
            var existingLogin = await _context.ExternalLogins
                .FirstOrDefaultAsync(el => el.Provider == provider && el.ProviderUserId == providerUserId);

            ApplicationUser user;

            if (existingLogin != null)
            {
                user = await _userManager.FindByIdAsync(existingLogin.UserId.ToString());
                if (user == null)
                    return null;
            }
            else
            {
                user = null!;

                if (!string.IsNullOrEmpty(email))
                {
                    user = await _userManager.FindByEmailAsync(email);
                }

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email ?? $"{provider}_{providerUserId}",
                        FullName = fullName ?? email ?? $"{provider} User",
                        EmailConfirmed = true,
                        CreateAt = Luqma.Domain.Helpers.EgyptTime.Now
                    };

                    string password = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));
                    var createResult = await _userManager.CreateAsync(user, password);
                    if (!createResult.Succeeded)
                        return null;

                    string defaultRole = "User";
                    if (!await _roleManager.RoleExistsAsync(defaultRole))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(defaultRole));
                    }
                    await _userManager.AddToRoleAsync(user, defaultRole);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? "User")
                    };
                    await _userManager.AddClaimsAsync(user, claims);
                }

                _context.ExternalLogins.Add(new ExternalLogin
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Provider = provider,
                    ProviderUserId = providerUserId,
                    CreatedAt = Luqma.Domain.Helpers.EgyptTime.Now
                });
                await _context.SaveChangesAsync();
            }

            var userClaims = (await _userManager.GetClaimsAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            userClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(userClaims);

            return new AuthUserResponseDto
            {
                UserId = user.Id.ToString(),
                UserName = user.FullName ?? user.UserName,
                Role = roles.FirstOrDefault(),
                Token = token,
                ExpiresAt = Luqma.Domain.Helpers.EgyptTime.Now.AddMonths(10)
            };
        }

        public async Task SeedDefaultAdminAsync()
        {
            string adminEmail = "admin@gmail.com";
            string adminPassword = "in";

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
                return;

            string adminRole = "Admin";
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
            }

            string superAdminRole = "SuperAdmin";
            if (!await _roleManager.RoleExistsAsync(superAdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(superAdminRole));
            }

            ApplicationUser admin = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FullName = "Admin",
                EmailConfirmed = true,
                CreateAt = Luqma.Domain.Helpers.EgyptTime.Now
            };

            var result = await _userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, adminRole);
                await _userManager.AddToRoleAsync(admin, superAdminRole);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                    new Claim(ClaimTypes.Name, "Admin")
                };
                await _userManager.AddClaimsAsync(admin, claims);
            }
        }

        private string GenerateToken(List<Claim> claims)
        {
            string secutirykey = _configuration.GetSection("JWT:SecurityKey").Value!;
            string issuer = _configuration.GetSection("JWT:IssuerIP").Value!;
            string audience = _configuration.GetSection("JWT:AudienceIP").Value!;

            var securityKeyByte = Encoding.ASCII.GetBytes(secutirykey!);

            SecurityKey securityKey = new SymmetricSecurityKey(securityKeyByte);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: Luqma.Domain.Helpers.EgyptTime.Now.AddMonths(10),
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }

        private string GetEmailTemplate(string title, string message, string otp)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='ar' dir='rtl'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>{title}</title>
        <style>
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa; margin: 0; padding: 0; }}
            .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }}
            .header {{ background-color: #2c3e50; color: #ffffff; padding: 20px; text-align: center; }}
            .header h1 {{ margin: 0; font-size: 24px; }}
            .content {{ padding: 30px; text-align: center; color: #333333; }}
            .content p {{ font-size: 16px; line-height: 1.6; margin-bottom: 20px; }}
            .otp-box {{ background-color: #f8f9fa; border: 2px dashed #3498db; color: #2c3e50; font-size: 32px; font-weight: bold; padding: 15px 30px; margin: 20px auto; display: inline-block; border-radius: 5px; letter-spacing: 5px; }}
            .footer {{ background-color: #f1f1f1; color: #777777; text-align: center; padding: 15px; font-size: 14px; border-top: 1px solid #eeeeee; }}
            .footer p {{ margin: 5px 0; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>تطبيق Luqma</h1>
            </div>
            <div class='content'>
                <h2>{title}</h2>
                <p>{message}</p>
                <div class='otp-box'>{otp}</div>
                <p style='color: #e74c3c; font-weight: bold;'>هذا الرمز صالح لمدة 10 دقائق فقط. يُرجى عدم مشاركته مع أي شخص.</p>
            </div>
            <div class='footer'>
                <p>تم إرسال هذه الرسالة من تطبيق Luqma. إذا لم تقم بطلب هذا الرمز، يُرجى تجاهل هذه الرسالة.</p>
                <p>&copy; {Luqma.Domain.Helpers.EgyptTime.NowLocal.Year} تطبيق Luqma. جميع الحقوق محفوظة.</p>
            </div>
        </div>
    </body>
    </html>";
        }
    }
}
