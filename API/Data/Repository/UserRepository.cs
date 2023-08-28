using API.Models;
using Microsoft.EntityFrameworkCore;
using API.LIB.INTERFACES;
using API.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using API.LIB.HELPERS;

namespace API.Data.REPOSITORY;

public class UserRepository: IUserRepository{

    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public UserRepository(DataContext context, IMapper mapper)
    {
            _mapper = mapper;
            _context = context;
    }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();
            query = query.Where(user => user.UserName != userParams.CurrentUsername); //Do not get logedIn user
            query = query.Where(user => user.Gender == userParams.Gender); //Filter by gender

            var minDayOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDayOfBirth = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(user => user.DateOfBirth >= minDayOfBirth && user.DateOfBirth <= maxDayOfBirth);
            query = userParams.OrderBy switch{
                "created" => query.OrderByDescending(user => user.Created),
                _ => query.OrderByDescending(user => user.LastActive)
            };

            return await PagedList<MemberDTO>.CreateAsync(
                query.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).AsNoTracking(), 
                userParams.PageNumber, userParams.PageSize);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users.Where(user => user.UserName == username).Select(user => user.Gender).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
}
