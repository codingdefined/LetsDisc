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
        public int PostTypeId { get; set; }
        public int PostId { get; set; }

        public VoteChangeOutput()
        {
        }
    }
}
