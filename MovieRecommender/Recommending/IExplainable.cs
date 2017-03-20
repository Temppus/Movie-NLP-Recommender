using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Recommending
{
    public interface IExplainable
    {
        IEnumerable<ExplanationTuple> ExplainBySentiment(IEnumerable<string> imdbIds);
    }

    public class ExplanationTuple
    {
        public ExplanationTuple(string imdbId, Explanation explanation)
        {
            ImdbId = imdbId;
            Explanation = explanation;
        }

        public string ImdbId { get; set; }
        public Explanation Explanation { get; set; }
    }
}
