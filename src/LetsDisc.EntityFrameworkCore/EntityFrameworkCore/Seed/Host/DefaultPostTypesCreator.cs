using System.Collections.Generic;
using LetsDisc.PostDetails;
using System.Linq;

namespace LetsDisc.EntityFrameworkCore.Seed.Host
{
    class DefaultPostTypesCreator
    {
        public static List<PostType> InitialPostTypes => GetInitialPostType();

        private readonly LetsDiscDbContext _context;

        private static List<PostType> GetInitialPostType()
        {
            return new List<PostType>
            {
                new PostType("Question"),
                new PostType("Answer"),
                new PostType("Tag Wiki")
            };
        }

        public DefaultPostTypesCreator(LetsDiscDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreatePostTypes();
        }

        private void CreatePostTypes()
        {
            foreach (var postType in InitialPostTypes)
            {
                AddPostTypeIfNotExists(postType);
            }
        }

        private void AddPostTypeIfNotExists(PostType postType)
        {
            if(_context.PostTypes.Any(p => p.Name == postType.Name))
            {
                return;
            }

            _context.PostTypes.Add(postType);
            _context.SaveChanges();
        }
    }
}
