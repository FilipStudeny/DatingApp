using API.Models;
using Microsoft.EntityFrameworkCore;
using API.LIB.INTERFACES;
using API.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace API.Data.REPOSITORY;

public class UserRepository: IUserRepository{

    private readonly DataContext context;
    private readonly IMapper mapper;

    public UserRepository(DataContext context, IMapper mapper){
        this.context = context;
        this.mapper = mapper;
    }

    async Task<MemberDTO> IUserRepository.GetMemberAsync(string username)
    {
        return await context.Users
            .Where(u => u.Username == username)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    async Task<IEnumerable<MemberDTO>> IUserRepository.GetMembersAsync()
    {
        return await context.Users
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    async Task<User> IUserRepository.GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    async Task<User> IUserRepository.GetUserByUsernameAsync(string username)
    {
        return await context.Users.Include(photos => photos.Photos).SingleOrDefaultAsync(u => u.Username == username);
    }

    async Task<IEnumerable<User>> IUserRepository.GetUsersAsync()
    {
        return await context.Users.Include(photos => photos.Photos).ToListAsync();
    }

    async Task<bool> IUserRepository.SaveAllAsync()
    {
        var isChanged = await context.SaveChangesAsync() > 0;
        return isChanged;
    }

    void IUserRepository.Update(User user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}