using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Utils
{
    public class MathUtil
    {
        public static double Normalize(double rawScore, double minScore, double maxScore) 
        {
            double normalizedScore = (rawScore - minScore) / (maxScore - minScore);
            return normalizedScore;
        }
    }
}