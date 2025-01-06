using Microsoft.EntityFrameworkCore;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Static;
using Newtonsoft.Json;
using System.Net;

namespace MyPaySlipLive.BLL.Service
{
    public class BlogService : IBlog
    {
        private readonly PayslipDbContext _db;
        public BlogService(PayslipDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<Response> Add(BlogDto blogDetail)
        {
            try
            {
                
                var newBlog = new TblBlog()
                {
                    Id = Guid.NewGuid(),
                    Details = blogDetail.Details,
                    Date = GetTime.GetIndianTimeZone()
                };
                _db.TblBlogs.Add(newBlog);
                await _db.SaveChangesAsync();
                return new Response(HttpStatusCode.OK.ToString(), $"{Message.AddSuccessfully}");
            }
            catch { return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.AddFail}"); }

        }
        public async Task<Response> Update(BlogDto blogDetail)
        {
            try
            {
                var blog = _db.TblBlogs.FirstOrDefault(e => e.Id == blogDetail.Id);
                if (blog == null)
                {
                    return new Response(HttpStatusCode.BadRequest.ToString(), $"Blog {Message.NotFound}");
                }
                blog.Details = string.IsNullOrEmpty(blogDetail.Details!.Trim()) ? blog.Details : blogDetail.Details;
                _db.TblBlogs.Update(blog);
                await _db.SaveChangesAsync();
                return new Response(HttpStatusCode.OK.ToString(), $"{Message.UpdateSuccessfully}");
            }
            catch
            {
                return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.UpdateFail}");
            }
        }

        public async Task<Response> AddUpdateBlog(BlogDto blogDetail)
        {
            if (blogDetail.Id == Guid.Empty) { return await Add(blogDetail); }
            return await Update(blogDetail);
        }

        public async Task<Response> GetAllBlogsForSuperAdmin(Pagination pagination)
        {
            pagination.PageNumber = pagination.PageNumber > 0 ? pagination.PageNumber : 1;
            pagination.PageSize = pagination.PageSize > 0 ? pagination.PageSize : 10;
            int skip = (pagination.PageNumber - 1) * pagination.PageSize;
            var blogList = await _db.TblBlogs.OrderByDescending(e => e.Date).ToListAsync();
            var blogListAfterPagination = blogList.Skip(skip).Take(pagination.PageSize);
            return new Response(HttpStatusCode.OK.ToString(), $"{blogList.Count}{Message.Found}", blogList.Count(), JsonConvert.SerializeObject(blogListAfterPagination));
        }

        public async Task<Response> GetAllBlogsForHome()
        {
            
            var blogList = await _db.TblBlogs.OrderByDescending(e => e.Date).ToListAsync();
            if (blogList.Count == 0) { return new Response(HttpStatusCode.BadRequest.ToString(), $"Blog {Message.NotFound}"); }
             return new Response(HttpStatusCode.OK.ToString(), $"{blogList.Count}{Message.Found}", blogList.Count(), JsonConvert.SerializeObject(blogList));
        }

        public async Task<Response> GetBlogById(Guid id)
        {
            var blogDetail = await _db.TblBlogs.FirstOrDefaultAsync(e => e.Id == id);
            if (blogDetail == null) { return new Response(HttpStatusCode.BadRequest.ToString(), $"Blog {Message.NotFound}"); }
            return new Response(HttpStatusCode.OK.ToString(), $"Company {Message.Found}", JsonConvert.SerializeObject(blogDetail));
        }

        public async Task<Response> DeleteBlog(Guid blogId)
        {
            try
            {
                var blog = _db.TblBlogs.FirstOrDefault(e => e.Id == blogId);
                if (blog == null)
                {
                    return new Response(HttpStatusCode.BadRequest.ToString(), $"Blog {Message.NotFound}");
                }

                _db.TblBlogs.Remove(blog);
                await _db.SaveChangesAsync();

                return new Response(HttpStatusCode.OK.ToString(), $"{Message.DeleteSuccessfully}");
            }
            catch
            {
                return new Response(HttpStatusCode.BadRequest.ToString(), $"{Message.DeleteFail}");
            }
        }

    }
}
