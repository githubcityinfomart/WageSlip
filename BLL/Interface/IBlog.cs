using MyPaySlipLive.Models;

namespace MyPaySlipLive.BLL.Interface
{
    public interface IBlog
    {
        Task<Response> AddUpdateBlog(BlogDto blogDetail);
        Task<Response> GetAllBlogsForSuperAdmin(Pagination pagination);
        Task<Response> GetAllBlogsForHome();
        Task<Response> GetBlogById(Guid id);

        Task<Response> DeleteBlog(Guid id);
    }
}
