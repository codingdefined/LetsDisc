using System.Collections.Generic;
using System.Linq;
using LetsDisc.Votes;

namespace LetsDisc.EntityFrameworkCore.Seed.Host
{
    class DefaultVoteTypesCreator
    {
        public static List<VoteType> InitialVoteTypes => GetInitialVoteType();

        private readonly LetsDiscDbContext _context;

        private static List<VoteType> GetInitialVoteType()
        {
            return new List<VoteType>
            {
                new VoteType("AcceptedByOriginator"),
                new VoteType("Upvote"),
                new VoteType("Downvote"),
                new VoteType("Favorite"),
                new VoteType("Spam"),
                new VoteType("ModeratorReview")
            };
        }

        public DefaultVoteTypesCreator(LetsDiscDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateVoteTypes();
        }

        private void CreateVoteTypes()
        {
            foreach (var voteType in InitialVoteTypes)
            {
                AddVoteTypeIfNotExists(voteType);
            }
        }

        private void AddVoteTypeIfNotExists(VoteType voteType)
        {
            if(_context.VoteTypes.Any(p => p.Name == voteType.Name))
            {
                return;
            }

            _context.VoteTypes.Add(voteType);
            _context.SaveChanges();
        }
    }
}
