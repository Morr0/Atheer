﻿using System;
using Atheer.Extensions;
using Atheer.Models;

namespace Atheer.Services.TagService
{
    public class TagFactory
    {
        /// <summary>
        /// This should be called when creating a tag that does not exist linked to an article
        /// </summary>
        /// <returns></returns>
        public Tag CreateTag(string title)
        {
            string id = GetId(title);
            var date = DateTime.UtcNow;
            return new Tag
            {
                Id = id,
                Title = title,
                DateCreated = date.GetString(),
                DateLastAddedTo = date.GetString()
            };
        }

        public void UpdateTag(Tag tag)
        {
            tag.DateLastAddedTo = DateTime.UtcNow.GetString();
        }

        public string GetId(string title)
        {
            return title.Trim().ToLower().Replace(' ', '-');
        }
    }
}