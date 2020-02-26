using DatingApp.API.Helpers;
using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppAPI.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _dataContext;

        public DatingRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public void Add<T>(T entity) where T : class
        {
            _dataContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _dataContext.Remove(entity);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            //var query = _dataContext.Users.AsQueryable();

            //if (isCurrentUser)
            //    query = query.IgnoreQueryFilters();

            //var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            var user = await _dataContext.Users.Include(x => x.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //var users = await _dataContext.Users.Include(x => x.Photos).ToListAsync();
            var users = _dataContext.Users.Include(x => x.Photos).OrderByDescending(x => x.LastActive).AsQueryable();
            users = users.Where(x => x.Id != userParams.UserId);
            users = users.Where(x => x.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(x => userLikers.Contains(x.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(x => userLikees.Contains(x.Id));
            }

            if (userParams.MinAge !=18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(x => x.DateOfBirth <= maxDob && x.DateOfBirth >= minDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created": 
                        users = users.OrderByDescending(x => x.Created);
                        break;
                    default:
                        users = users.OrderByDescending(x => x.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<IEnumerable<int>> GetUserLikes(int id, bool liker) 
        {
            var user = await _dataContext.Users
                .Include(x => x.Likees)
                .Include(x => x.Likers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (liker)
            {
                return user.Likers.Where(x => x.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(x => x.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _dataContext.Photos.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _dataContext.Photos.Where(x => x.UserId == userId).FirstOrDefaultAsync(x => x.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _dataContext.Likes.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recipientId);
        }
    }
}
