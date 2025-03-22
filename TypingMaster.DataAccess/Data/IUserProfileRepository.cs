using TypingMaster.DataAccess.Dao;
using TypingMaster.DataAccess.Utility;

namespace TypingMaster.DataAccess.Data;

public interface IUserProfileRepository
{
    Task<UserProfileDao?> GetUserProfileByIdAsync(int id);

    Task<UserProfileDao> CreateUserProfileAsync(UserProfileDao userProfile);

    Task<UserProfileDao?> UpdateUserProfileAsync(UserProfileDao userProfile);

    Task<bool> CanDeleteUserProfileAsync(int id);

    Task<bool> DeleteUserProfileAsync(int id); 

    ProcessResult ProcessResult { get; protected set; }
}