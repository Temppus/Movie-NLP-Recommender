using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Extensions
{
    public static class EnumerableExtensions
    {
        private static Random _random = new Random();

        /// <summary>
        /// Taken from http://stackoverflow.com/questions/9141594/whats-the-most-concise-way-to-pick-a-random-element-by-weight-in-c
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="weightSelector"></param>
        /// <returns></returns>
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, int> weightSelector)
        {
            var items = sequence.ToList();

            int totalWeight = items.Sum(x => weightSelector(x));
            int randomWeightedIndex = _random.Next(totalWeight);
            int itemWeightedIndex = 0;
            foreach (var item in items)
            {
                itemWeightedIndex += weightSelector(item);
                if (randomWeightedIndex < itemWeightedIndex)
                    return item;
            }
            throw new ArgumentException("Collection count and weights must be greater than 0");
        }
    }
}