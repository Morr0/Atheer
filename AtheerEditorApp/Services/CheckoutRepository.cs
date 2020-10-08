using AtheerEditorApp.Constants;
using AtheerEditorApp.Services.Strategies;
using System.Threading.Tasks;

namespace AtheerEditorApp.Services
{
    internal class CheckoutRepository
    {
        private UIDataMapper _uiDataMapper;
        private CheckoutStrategy _currentStrategy;
        
        public CheckoutRepository(UIDataMapper uiDataMapper)
        {
            _uiDataMapper = uiDataMapper;
            _currentStrategy = new NewPostCheckoutStrategy();
        }

        public void ChangeStrategy(OperationType operationType)
        {
            if (operationType == OperationType.New)
                _currentStrategy = new NewPostCheckoutStrategy();
        }
        
        public async Task Checkout()
        {
            await _currentStrategy.Checkout(_uiDataMapper.Post());
        }
    }
}
