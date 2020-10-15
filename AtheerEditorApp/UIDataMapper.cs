using AtheerCore.Models;
using System;
using System.Windows.Controls;
using AtheerEditorApp.Services.CheckoutService.Inputs;

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
        
        private CheckBox _useScheduledDateCheckBox;
        private DatePicker _scheduledDatePicker;

        internal PasswordBox _secretBox;

        /// <summary>
        /// Only to be used when not adding a new article
        /// </summary>
        private BlogPost _post;

        public UIDataMapper(TextBox yearBox, TextBox shrinkedTitleBox, TextBox titleBox,
            TextBox topicBox, TextBox descriptionBox, TextBox contentBox, 
            CheckBox draftCheckbox, CheckBox unlistedCheckbox, PasswordBox secretBox, 
            CheckBox likeableCheckbox, CheckBox shareableCheckbox, 
            CheckBox useScheduledDateCheckBox, DatePicker scheduledDatePicker)
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

            _useScheduledDateCheckBox = useScheduledDateCheckBox;
            _scheduledDatePicker = scheduledDatePicker;
            
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
                // Customizablity of the created year if applicable i.e. modified the year text box
                if (!string.IsNullOrEmpty(_yearBox.Text))
                {
                    // If can parse to number then can customise
                    try
                    {
                        int year = int.Parse(_yearBox.Text);
                        post.CreatedYear = year;
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }
                
                // Customizablity of the shrinked title if applicable i.e. modified the shrinked title
                // Else fallback to shrinking the original
                string titleShrinkedCandidate = string.IsNullOrEmpty(_shrinkedTitleBox.Text) ? post.Title : _shrinkedTitleBox.Text;
                post.TitleShrinked = titleShrinkedCandidate.TrimStart().TrimEnd()
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

        public CheckoutInput GetNewArticleCheckoutInput()
        {
            if (!_useScheduledDateCheckBox.IsChecked.Value)
                return null;

            if (_scheduledDatePicker.SelectedDate == null)
                return null;
            
            return new NewArticleCheckoutSchedulingInput(_scheduledDatePicker.SelectedDate.Value);
        }
    }
}
