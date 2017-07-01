using MovieRecommender.Database.Models;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Database.CollectionAPI
{
    public interface IUserExperimentRepository
    {
        void LogUserProgress(UserExperimentProgress model);

        IEnumerable<ClickInfo> GetClickProgresssForUser(string userName, Experiment experimentResult);
    }
}
