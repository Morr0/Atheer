using System.Threading.Tasks;
using AtheerCore.Models;
using AtheerEditorApp.Constants;
using AtheerEditorApp.Exceptions;
using AtheerEditorApp.Services.AuthorizationService;
using AtheerEditorApp.Services.CheckoutService.Inputs;
using AtheerEditorApp.Services.CheckoutService.Strategies;

namespace AtheerEditorApp.Services.CheckoutService
{
    internal sealed class CheckoutRepository
    {
        private UIDataMapper _uiDataMapper;
        private OperationType _operationType;
        private CheckoutStrategy _currentStrategy;

        private AuthorizationRepository _authorizationService;
        
        public CheckoutRepository(UIDataMapper uiDataMapper)
        {
            _uiDataMapper = uiDataMapper;
            _operationType = OperationType.New;
            _currentStrategy = new NewPostCheckoutStrategy();
            
            _authorizationService = new AuthorizationRepository();
        }

        public void ChangeStrategy(OperationType operationType)
        {
            _operationType = operationType;
            
            switch (operationType)
            {
                default:
                case OperationType.New:
                    _currentStrategy = new NewPostCheckoutStrategy();
                    break;
                case OperationType.Edit:
                    _currentStrategy = new EditPostCheckoutStrategy();
                    break;
                case OperationType.Remove:
                    _currentStrategy = new RemovePostCheckoutStrategy();
                    break;
            }
        }
        
        public async Task Checkout(CheckoutInput input = null)
        {
            if (!await _authorizationService.Allowed(_uiDataMapper.Secret))
                throw new IncorrectSecretException();
                
            await _currentStrategy.Checkout(_uiDataMapper.Post(_operationType == OperationType.New), input);
        }

        public async Task<BlogPost> Get(int year, string titleShrinked)
        {
            // The strategy used does not matter
            return await _currentStrategy.GetPostElseNull(year, titleShrinked);
        }
    }
}
