using AtheerCore.Models;
using System;
using System.Windows.Controls;

namespace AtheerEditorApp
{
    /// <summary>
    /// Maps UI data into a blog post object
    /// </summary>
    public class UIDataMapper
    {
        internal TextBox _yearBox;
        internal TextBox _shrinkedTitleBox;
        internal TextBox _titleBox;
        internal TextBox _topicBox;
        internal TextBox _descriptionBox;
        internal TextBox _contentBox;
        private CheckBox _draftCheckbox;
        private CheckBox _unlistedCheckbox;
        private CheckBox _likeableCheckbox;
        private CheckBox _shareableCheckbox;

        internal PasswordBox _secretBox;

        /// <summary>
        /// Only to be used when not adding a new article
        /// </summary>
        private BlogPost _post;

        public UIDataMapper(TextBox yearBox, TextBox shrinkedTitleBox, TextBox titleBox,
            TextBox topicBox, TextBox descriptionBox, TextBox contentBox, 
            CheckBox draftCheckbox, CheckBox unlistedCheckbox, PasswordBox secretBox, 
            CheckBox likeableCheckbox, CheckBox shareableCheckbox)
        {
            _yearBox = yearBox;
            _shrinkedTitleBox = shrinkedTitleBox;
            _titleBox = titleBox;
            _topicBox = topicBox;
            _descriptionBox = descriptionBox;
            _contentBox = contentBox;
            _draftCheckbox = draftCheckbox;
            _unlistedCheckbox = unlistedCheckbox;
            _likeableCheckbox = likeableCheckbox;
            _shareableCheckbox = shareableCheckbox;
            
            _secretBox = secretBox;
        }
        
        public string Secret => _secretBox.Password;

        public void Fill(BlogPost post)
        {
            _post = post;
            
            _yearBox.Text = post.CreatedYear.ToString();
            _shrinkedTitleBox.Text = post.TitleShrinked;
            _titleBox.Text = post.Title;
            _topicBox.Text = post.Topic;
            _descriptionBox.Text = post.Description;
            _contentBox.Text = post.Content;
            _draftCheckbox.IsChecked = post.Draft;
            _unlistedCheckbox.IsChecked = post.Unlisted;
            _likeableCheckbox.IsChecked = post.Likeable;
            _shareableCheckbox.IsChecked = post.Shareable;
        }

        public void Clear()
        {
            _post = null;
            
            _yearBox.Text = "";
            _shrinkedTitleBox.Text = "";
            _titleBox.Text = "";
            _topicBox.Text = "";
            _descriptionBox.Text = "";
            _contentBox.Text = "";
            _draftCheckbox.IsChecked = true;
            _unlistedCheckbox.IsChecked = false;
            _shareableCheckbox.IsChecked = true;
            _likeableCheckbox.IsChecked = true;
            
            _secretBox.Clear();
        }

        public BlogPost Post(bool @new = true)
        {
            BlogPost post = @new ? new BlogPost() : (_post ?? new BlogPost());
            post.Content = _contentBox.Text;
            post.Description = _descriptionBox.Text;
            post.Title = _titleBox.Text;
            post.Topic = _topicBox.Text;
            post.Unlisted = _unlistedCheckbox.IsChecked ?? true;
            post.Draft = _draftCheckbox.IsChecked ?? true;
            post.Likeable = _likeableCheckbox.IsChecked ?? true;
            post.Shareable = _shareableCheckbox.IsChecked ?? true;

            // Create first time metadata
            if (@new)
            {
                post.TitleShrinked = post.Title.TrimStart().TrimEnd()
                    .ToLower().Replace(" ", "-");
            }

            DateTime currDate = DateTime.UtcNow;
            // Update dates
            if (@new)
                post.CreationDate = post.LastUpdatedDate = currDate.ToString();
            else
                post.LastUpdatedDate = currDate.ToString();
            
            return post;
        }
    }
}
