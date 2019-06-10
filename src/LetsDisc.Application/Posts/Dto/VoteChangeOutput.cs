using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Posts.Dto
{
    public class VoteChangeOutput
    {
        public int VoteCount { get; set; }
        public bool UpVote { get; set; }
        public bool DownVote { get; set; }

        public VoteChangeOutput(int voteCount, bool upVote, bool downVote)
        {
            VoteCount = voteCount;
            UpVote = upVote;
            DownVote = downVote;
        }
    }
}
