using AtheerEditorApp.Constants;

namespace AtheerEditorApp.Services.UIDataValidationService
{
    public class UIValidationRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns null if valid, otherwise will return the first non-valid field name</returns>
        public static string IsValid(OperationType operationType, UIDataMapper uiDataMapper)
        {
            // Because a new post doesn't have titleShrinked neither a year, are auto generated as ID
            if (operationType != OperationType.New)
            {
                if (!Valid(uiDataMapper._shrinkedTitleBox.Text))
                    return nameof(uiDataMapper._titleBox);
                if (!Valid(uiDataMapper._yearBox.Text))
                    return nameof(uiDataMapper._contentBox);
            }
            
            if (!Valid(uiDataMapper._titleBox.Text))
                return nameof(uiDataMapper._titleBox);
            if (!Valid(uiDataMapper._contentBox.Text))
                return nameof(uiDataMapper._contentBox);
            if (!Valid(uiDataMapper._descriptionBox.Text))
                return nameof(uiDataMapper._descriptionBox);
            if (!Valid(uiDataMapper._topicBox.Text))
                return nameof(uiDataMapper._topicBox);
            
            // Ignore boolean checkboxes as they are choices

            return null;
        }

        private static bool Valid(string text)
        {
            // ADD ANY CHECKING HERE
            return !string.IsNullOrEmpty(text);
        }
    }
}