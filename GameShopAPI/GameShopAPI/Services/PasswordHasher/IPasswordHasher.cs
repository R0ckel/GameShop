using GameShopAPI.Models.Domain;

namespace GameShopAPI.Services.PasswordHasher;

public interface IPasswordHasher
{
    void SetUserPasswordHash(User user, string password);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
}
