using System;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.Utilities.TimeService;

namespace Atheer.Services.TagService
{
    public class TagFactory
    {
        private readonly ITimeService _timeService;

        public TagFactory(ITimeService timeService)
        {
            _timeService = timeService;
        }
        
        /// <summary>
        /// This should be called when creating a tag that does not exist linked to an article
        /// </summary>
        /// <returns></returns>
        public Tag CreateTag(string title)
        {
            string id = GetId(title);
            var date = _timeService.Get();
            return new Tag
            {
                Id = id,
                Title = title,
                CreatedAt = date,
                LastUpdatedAt = date
            };
        }

        public void UpdateTag(Tag tag)
        {
            tag.LastUpdatedAt = _timeService.Get();
        }

        public string GetId(string title)
        {
            return title.Trim().ToLower().Replace(' ', '-');
        }
    }
}