using Abp.AutoMapper;
using LetsDisc.PostDetails;
using LetsDisc.Votes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Posts.Dto
{
    public class PostWithVoteInfo
    {
        public PostDto Post { get; set; }
        public bool Upvote { get; set; }
        public bool Downvote { get; set; }
    }
}
