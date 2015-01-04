using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public class VoteManager
    {
        public static int NewVote(string name)
        {
            cms_vote vote = new cms_vote { name = name, can_vote = true, enabled = true, start_time = DateTime.Now };
            LinqHelper.CMS.cms_vote.InsertOnSubmit(vote);
            LinqHelper.CMS.SubmitChanges();
            return LinqHelper.CMS.cms_vote.Max(x => x.id);
        }

        public static cms_vote GetDbRecord(int voteId)
        {
            return LinqHelper.CMS.cms_vote.Single(x => x.id == voteId);
        }

        public static IEnumerable<cms_vote> GetAllVotes()
        {
            return LinqHelper.CMS.cms_vote.Where(x => x.enabled);
        }

        public static void SaveChanges()
        {
            LinqHelper.CMS.SubmitChanges();
        }

        public static void SetVoteOptions(int voteId, params string[] options)
        {
            LinqHelper.CMS.cms_vote_option.DeleteAllOnSubmit(LinqHelper.CMS.cms_vote_option.Where(x => x.fk_vote_id == voteId));
            LinqHelper.CMS.cms_vote_option.InsertAllOnSubmit(options.Select(x => new cms_vote_option { name = x, fk_vote_id = voteId, votes = 0 }));
            LinqHelper.CMS.SubmitChanges();
        }

        public static IEnumerable<cms_vote_option> GetVoteOptions(int voteId)
        {
            return LinqHelper.CMS.cms_vote_option.Where(x => x.fk_vote_id == voteId);
        }
    }
}
