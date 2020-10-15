using System;
using AtheerEditorApp.Constants;
using System.Windows;
using System.Windows.Controls;
using AtheerEditorApp.Exceptions;
using AtheerEditorApp.Services.CheckoutService;
using AtheerEditorApp.Services.CheckoutService.Inputs;
using AtheerEditorApp.Services.UIDataValidationService;

namespace AtheerEditorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // 
        private OperationType _currentSelectedOp = OperationType.New;

        private UIDataMapper _uiDataMapper;

        private readonly CheckoutRepository _checkoutRepo;

        // To not let some methods get called at startup due to WPF
        // ALSO USED with changing the selection of combobox programatically so it does not double
        // fire the event
        private static bool _firstTimeComboboxSelect = true;

        public MainWindow()
        {
            InitializeComponent();

            InitUIDatamapper();

            _checkoutRepo = new CheckoutRepository(_uiDataMapper);
        }

        private void InitUIDatamapper()
        {
            _uiDataMapper = new UIDataMapper(
                _year, _titleShrinked, _title, _topic, _description, _content, _draft,
                _unlisted, _secret, _likeable, _shareable,
                _useScheduledDate, _scheduledDate, _scheduledTime);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SO IT IS NOT CALLED ON INITIALIZATION
            if (_firstTimeComboboxSelect)
            {
                _firstTimeComboboxSelect = false;
                return;
            }
            
            // Before changing, check the user is alright with it since will clear fields
            MessageBoxResult result = MessageBox.Show
                ("Changing this selection will result in clearing all fields, are you sure to proceed"
                , "CHECK", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No || result == MessageBoxResult.None
                                              || result == MessageBoxResult.Cancel)
            {
                ResetComboBox();
                return;
            }
            
            var item = e.AddedItems[0] as ComboBoxItem;
            SetNewSelection(ref item);
            _checkoutRepo?.ChangeStrategy(_currentSelectedOp);
            SetGetPostButtonVisibility();
            SetScheduledControlsVisibility();
            _uiDataMapper.Clear();
        }

        private void SetScheduledControlsVisibility()
        {
            _useScheduledDate.Visibility =
                _currentSelectedOp == OperationType.New ? Visibility.Visible : Visibility.Hidden;
            _scheduledDate.Visibility =
                _currentSelectedOp == OperationType.New ? Visibility.Visible : Visibility.Hidden;
            _scheduledTime.Visibility = 
                _currentSelectedOp == OperationType.New ? Visibility.Visible : Visibility.Hidden;
            _scheduleLabel.Visibility = 
                _currentSelectedOp == OperationType.New ? Visibility.Visible : Visibility.Hidden;
        }

        // To reverse the effect of uncompleted event in case of user not wanting to change the operation
        private void ResetComboBox()
        {
            _combobox.SelectedIndex = (int) _currentSelectedOp;
        }

        private void SetNewSelection(ref ComboBoxItem item)
        {
            switch (item.Name)
            {
                case "edit":
                    _currentSelectedOp = OperationType.Edit;
                    break;
                case "remove":
                    _currentSelectedOp = OperationType.Remove;
                    break;
                default:
                    
                    _currentSelectedOp = OperationType.New;
                    break;
            }
        }

        private void SetGetPostButtonVisibility()
        {
            // FOR FIRST TIME ONLY DUE TO WPF
            if (_get == null)
                return;
            
            _get.Visibility = _currentSelectedOp == OperationType.New 
                ? Visibility.Hidden : Visibility.Visible;
        }
        
        // Checkout button
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validations
                string invalidField = UIValidationRepository.IsValid(_currentSelectedOp, _uiDataMapper);
                if (invalidField != null)
                {
                    MessageBox.Show($"Please provide the {invalidField} field");
                    return;
                }

                CheckoutInput input = null;
                try
                {
                    input = _currentSelectedOp == OperationType.New
                        ? _uiDataMapper.GetNewArticleCheckoutInput()
                        : null;
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please provide the correct time of the day in 24hr format as 00:00 format");
                    return;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Please provide the time of day within the bounds of 24 and 0 hours");
                    return;
                }

                // Execution
                
                await _checkoutRepo.Checkout(input);

                // Success
                MessageBox.Show("Successful checkout");
                _uiDataMapper.Clear();
            }
            catch (IncorrectSecretException)
            {
                MessageBox.Show("Please provide the correct secret");
            }
            catch (APostExistsWithSamePrimaryKeyException)
            {
                MessageBox.Show("Another post exists with the same title in this year, please change it");
            }
        }

        // Get post button, visible only when not posting a new post
        private async void _get_Click(object sender, RoutedEventArgs e)
        {
            if (!UIValidationRepository.IsValidForFetchingAPost(_uiDataMapper))
            {
                MessageBox.Show("Please provide a key (combination of year and titleShrinked) to fetch");
                return;
            }
                
            // Get the post
            _uiDataMapper.Fill(await _checkoutRepo.Get(int.Parse(_uiDataMapper._yearBox.Text)
                , _uiDataMapper._shrinkedTitleBox.Text));
        }
    }
}
