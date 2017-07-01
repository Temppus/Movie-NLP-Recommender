using MovieRecommender.Database.CollectionAPI;
using MovieRecommender.Models;
using MovieRecommender.Recommending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieRecommender.Controllers
{
    [Authorize]
    public class ExperimentController : Controller
    {
        private readonly IUserExperimentRepository _experimentStore;
        private readonly IUserRepository _userStore;

        public ExperimentController(IUserRepository userStore, IUserExperimentRepository experimentStore)
        {
            _userStore = userStore;
            _experimentStore = experimentStore;
        }

        public ActionResult Index()
        {
            var userName = User.Identity.Name;

            ExperimentResultViewModel model = _userStore.FillExperimentData();

            int minClicks = int.MaxValue;

            foreach (var user in model.Users)
            {
                var clickInfos = _experimentStore.GetClickProgresssForUser(user.UserName, user.ExperimentResult).ToList();
                int clickCount = clickInfos.Count();

                if (clickCount < minClicks)
                {
                    minClicks = clickCount;
                }
            }

            if (minClicks == int.MaxValue)
                minClicks = 0;

            for (int i = 0; i < minClicks; i++)
            {
                model.SentimentWouldWatchList.Add(0);
                model.KeyWordsWouldWatchList.Add(0);
                model.SawPrecisionAt.Add(0.0);
                model.SentimentPrecisionAt.Add(0.0);
                model.KeywordPrecisionAt.Add(0.0);
            }

            foreach (var user in model.Users)
            {
                var clickInfos = _experimentStore.GetClickProgresssForUser(user.UserName, user.ExperimentResult).Take(minClicks).ToList();
                model.UsersClicksByTime.Add(clickInfos);

                model.SawNDCG += NDCG.Compute(clickInfos.Select(x => x.ClickType == ClickType.Saw ? 1 : 0).ToList());
                model.SentimentSingleNDCG += NDCG.Compute(clickInfos.Where(x => x.IsSentimentClick && x.ClickType != ClickType.Saw).Select(x => x.ClickType == ClickType.WouldWatch ? 1 : 0).ToList());
                model.KeywordSingleNDCG += NDCG.Compute(clickInfos.Where(x => !x.IsSentimentClick && x.ClickType != ClickType.Saw).Select(x => x.ClickType == ClickType.WouldWatch ? 1 : 0).ToList());

                model.SentimentNDCGAll += NDCG.Compute(clickInfos.Where(x => x.IsSentimentClick).Select(x => x.ClickType == ClickType.WouldWatch || x.ClickType == ClickType.Saw ? 1 : 0).ToList());
                model.KeywordNDCGAll += NDCG.Compute(clickInfos.Where(x => !x.IsSentimentClick).Select(x => x.ClickType == ClickType.WouldWatch || x.ClickType == ClickType.Saw ? 1 : 0).ToList());

                for (int i = 0; i < clickInfos.Count; i++)
                {
                    var clickInfo = clickInfos[i];

                    if (clickInfo.ClickType != ClickType.WouldWatch)
                        continue;

                    if (clickInfo.IsSentimentClick)
                    {
                        model.SentimentWouldWatchList[i] += 1;
                    }
                    else
                    {
                        model.KeyWordsWouldWatchList[i] += 1;
                    }
                }
            }

            model.SawNDCG /= (double)model.Users.Count;
            model.SentimentSingleNDCG /= (double)model.Users.Count;
            model.KeywordSingleNDCG /= (double)model.Users.Count;

            model.SentimentNDCGAll /= (double)model.Users.Count;
            model.KeywordNDCGAll /= (double)model.Users.Count;


            int minSentimentClicks = int.MaxValue;
            int minKWClicks = int.MaxValue;

            foreach (var user in model.Users)
            {
                var clickInfos = _experimentStore.GetClickProgresssForUser(user.UserName, user.ExperimentResult).Take(minClicks).ToList();

                // SAW PRECISION
                int sawTruePositives = 0;

                for (int k = 0; k < clickInfos.Count(); k++)
                {
                    var clickInfo = clickInfos[k];

                    if (clickInfo.ClickType == ClickType.Saw)
                    {
                        sawTruePositives++;
                        model.SawPrecisionAt[k] += (double)sawTruePositives / (double)(k + 1);
                    }
                }

                // SENTIMENT PRECISION
                var sentimentClicks = clickInfos.Where(x => x.IsSentimentClick).ToList();

                if (sentimentClicks.Count < minSentimentClicks)
                {
                    minSentimentClicks = sentimentClicks.Count;
                }

                int sentimentTruePositives = 0;

                for (int k = 0; k < sentimentClicks.Count(); k++)
                {
                    var clickInfo = sentimentClicks[k];

                    if (clickInfo.ClickType == ClickType.WouldWatch)
                    {
                        sentimentTruePositives++;
                        model.SentimentPrecisionAt[k] += (double)sentimentTruePositives / (double)(k + 1);
                    }
                }

                // KW PRECISION
                var keywordClicks = clickInfos.Where(x => !x.IsSentimentClick).ToList();

                if (keywordClicks.Count < minKWClicks)
                {
                    minKWClicks = keywordClicks.Count;
                }

                int keywordTruePositives = 0;

                for (int k = 0; k < keywordClicks.Count(); k++)
                {
                    var clickInfo = keywordClicks[k];

                    if (clickInfo.ClickType == ClickType.WouldWatch)
                    {
                        keywordTruePositives++;
                        model.KeywordPrecisionAt[k] += (double)keywordTruePositives / (double)(k + 1);
                    }
                }
            }

            model.KeywordPrecisionAt = model.KeywordPrecisionAt.Take(minKWClicks).ToList();
            model.SentimentPrecisionAt = model.SentimentPrecisionAt.Take(minSentimentClicks).ToList();
            model.SawPrecisionAt = model.SawPrecisionAt.Take(10).ToList();

            for (int i = 0; i < model.KeywordPrecisionAt.Count; i++)
            {
                model.KeywordPrecisionAt[i] /= (double)model.KeywordPrecisionAt.Count;
            }

            for (int i = 0; i < model.SentimentPrecisionAt.Count; i++)
            {
                model.SentimentPrecisionAt[i] /= (double)model.SentimentPrecisionAt.Count;
            }

            for (int i = 0; i < model.SawPrecisionAt.Count; i++)
            {
                model.SawPrecisionAt[i] /= (double)minClicks;
            }

            return View(model);
        }
    }
}
