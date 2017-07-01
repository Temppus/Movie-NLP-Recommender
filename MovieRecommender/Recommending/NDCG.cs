using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Recommending
{
    public class NDCG
    {
        private NDCG() { }

        public static double Compute(IList<int> items)
        {
            double dcg = 0.0;

            for (int i = 0; i < items.Count; i++)
            {
                double item = items[i];
                dcg += item / Math.Log(i + 2, 2);
            }

            var ordereditems = items.OrderByDescending(x => x).ToList();

            double idcg = 0.0;

            for (int i = 0; i < ordereditems.Count; i++)
            {
                double item = ordereditems[i];
                idcg += item / Math.Log(i + 2, 2);
            }

            if (idcg == 0.0)
                return 0.0;

            double ndcg = dcg / idcg;

            return ndcg;
        }
    }
}