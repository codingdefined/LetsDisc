using System.ComponentModel.DataAnnotations;

namespace LetsDisc.Votes
{
    public class VoteType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public VoteType(string name)
        {
            Name = name;
        }
    }
}