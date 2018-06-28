using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Questions.Dto
{
    public class VoteChangeOutput
    {
        public int VoteCount { get; set; }

        public VoteChangeOutput()
        {

        }

        public VoteChangeOutput(int voteCount)
        {
            VoteCount = voteCount;
        }
    }
}
