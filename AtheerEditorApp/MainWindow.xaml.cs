using System.Threading.Tasks;
using AtheerEditorApp.Constants;
using AtheerEditorApp.Services;
using System.Windows;
using System.Windows.Controls;
using AtheerEditorApp.Exceptions;
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

        private ComboBox _operationCombobox;
        private readonly Button _getPostButton;

        // To not let some methods get called at startup due to WPF
        // ALSO USED with changing the selection of combobox programatically so it does not double
        // fire the event
        private static bool _firstTimeComboboxSelect = true;

        public MainWindow()
        {
            InitializeComponent();

            _operationCombobox = FindName("_combobox") as ComboBox;
            _getPostButton = FindName("_get") as Button;

            InitUIDatamapper();

            _checkoutRepo = new CheckoutRepository(_uiDataMapper);
        }

        private void InitUIDatamapper()
        {
            _uiDataMapper = new UIDataMapper(
                FindName("_year") as TextBox,
                FindName("_titleShrinked") as TextBox,
                FindName("_title") as TextBox,
                FindName("_topic") as TextBox,
                FindName("_description") as TextBox,
                FindName("_content") as TextBox,
                FindName("_draft") as CheckBox,
                FindName("_unlisted") as CheckBox,
                FindName("_secret") as PasswordBox,
                FindName("_likeable") as CheckBox,
                FindName("_shareable") as CheckBox
                );
        }

        // Select button click for operation type
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
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
            _uiDataMapper.Clear();
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
            if (_getPostButton == null)
                return;
            
            _getPostButton.Visibility = _currentSelectedOp == OperationType.New 
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

                // 
                await _checkoutRepo.Checkout();

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
