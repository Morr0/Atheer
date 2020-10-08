using AtheerEditorApp.Constants;
using AtheerEditorApp.Services.Strategies;
using System.Threading.Tasks;
using AtheerEditorApp.Exceptions;
using AtheerEditorApp.Services.AuthorizationService;

namespace AtheerEditorApp.Services
{
    internal class CheckoutRepository
    {
        private UIDataMapper _uiDataMapper;
        private CheckoutStrategy _currentStrategy;

        private AuthorizationRepository _authorizationService;
        
        public CheckoutRepository(UIDataMapper uiDataMapper)
        {
            _uiDataMapper = uiDataMapper;
            _currentStrategy = new NewPostCheckoutStrategy();
            
            _authorizationService = new AuthorizationRepository();
        }

        public void ChangeStrategy(OperationType operationType)
        {
            if (operationType == OperationType.New)
                _currentStrategy = new NewPostCheckoutStrategy();
        }
        
        public async Task Checkout()
        {
            if (!await _authorizationService.Allowed(_uiDataMapper.Secret))
                throw new IncorrectSecretException();
                
            await _currentStrategy.Checkout(_uiDataMapper.Post());
        }
    }
}
