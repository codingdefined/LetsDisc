using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Posts.Dto
{
    public class PostWithAnswers
    {
        public PostWithVoteInfo Post { get; set; }
        public List<PostWithVoteInfo> Answers { get; set; }
    }
}
