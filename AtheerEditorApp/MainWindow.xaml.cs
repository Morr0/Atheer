using AtheerEditorApp.Constants;
using AtheerEditorApp.Services;
using System.Windows;
using System.Windows.Controls;
using AtheerEditorApp.Exceptions;

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

        private CheckoutRepository _checkoutRepo;

        public MainWindow()
        {
            InitializeComponent();

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
                
                FindName("_secret") as PasswordBox
                );
        }

        // Select button click for operation type
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as ComboBoxItem;
            SetNewSelection(ref item);
            _checkoutRepo?.ChangeStrategy(_currentSelectedOp);
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
                case "new":
                    _currentSelectedOp = OperationType.New;
                    break;
            }
        }
        
        // Checkout button
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                await _checkoutRepo.Checkout();
                MessageBox.Show("Successful checkout");
            }
            catch (IncorrectSecretException)
            {
                MessageBox.Show("Please provide the correct secret");
            }
            // TODO handle double sends
        }
    }
}
