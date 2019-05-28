using System.Collections.Generic;
using LetsDisc.Posts;
using System.Linq;

namespace LetsDisc.EntityFrameworkCore.Seed.Host
{
    class DefaultPostTypesCreator
    {
        public static List<PostTypes> InitialPostTypes => GetInitialPostType();

        private readonly LetsDiscDbContext _context;

        private static List<PostTypes> GetInitialPostType()
        {
            return new List<PostTypes>
            {
                new PostTypes("Question"),
                new PostTypes("Answer"),
                new PostTypes("Tag Wiki")
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

        private void AddPostTypeIfNotExists(PostTypes postType)
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
