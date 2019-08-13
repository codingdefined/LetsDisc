using System.ComponentModel.DataAnnotations;

namespace LetsDisc.PostDetails
{
    public class PostType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public PostType(string name)
        {
            Name = name;
        }
    }
}
