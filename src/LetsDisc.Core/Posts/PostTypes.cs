using System.ComponentModel.DataAnnotations;

namespace LetsDisc.Posts
{
    public class PostTypes
    {
        [Key]
        public string Name { get; set; }

        public PostTypes(string name)
        {
            Name = name;
        }
    }
}
