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
                new VoteType(1, "AcceptedByOriginator"),
                new VoteType(2, "Upvote"),
                new VoteType(3, "Downvote"),
                new VoteType(4, "Favorite"),
                new VoteType(5, "Spam"),
                new VoteType(6, "ModeratorReview")
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
