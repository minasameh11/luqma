using Luqma.Domain.AccountsDtos;
using Luqma.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luqma.Domain.Interfaces
{
    public interface IAccountManager
    {
        Task<AuthUserResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthUserResponseDto?> LoginAdminAsync(LoginDto loginDto);
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> RegisterAdminAsync(string email, string password);
        Task<bool> ConfirmEmailAsync(string email, string otp);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<string> ResendOtpAsync(string email);
        Task<bool> DeleteAsync(Guid Id);
        Task<bool> UpdateAsync(UpdateAccountDto updateAccountDto);
        Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
        Task<bool> ArchiveAsync(Guid Id);
        Task<bool> UnarchiveAsync(Guid Id);
        Task<UserReadDto?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateAccountDto updateAccountDto);
        Task<List<UserReadDto>> GetAllUsers();
        Task<UserReadDto?> GetUserByIdAsync(Guid Id);

        Task<bool> CreateRole(CreateRoleDto createRoleDto);
        Task<bool> AssignRoleToUser(AssignRoleDto assignRoleDto);
        Task<bool> UpdateRole(UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRole(string Id);
        Task<IEnumerable<RoleReadDto>> GetAllRoles();
        Task<string> GenerateTokenAsync(ApplicationUser user);
        Task<AuthUserResponseDto?> ExternalLoginAsync(string provider, string providerUserId, string? email, string? fullName);
        Task SeedDefaultAdminAsync();
    }
}
