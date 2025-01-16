using Microsoft.EntityFrameworkCore;
using QTechNote.Data.Models;

namespace QTechNote.Api.Logic;

public class UserLogic : BaseLogic<User>
{
    public UserLogic(QTechNoteContext context) : base(context) { }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    public override async Task<User> CreateAsync(User user)
    {
        // Password is already hashed in the controller
        return await base.CreateAsync(user);

    }

}








