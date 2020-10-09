using AtheerEditorApp.Constants;
using AtheerEditorApp.Services.Strategies;
using System.Threading.Tasks;
using AtheerCore.Models;
using AtheerEditorApp.Exceptions;
using AtheerEditorApp.Services.AuthorizationService;

namespace AtheerEditorApp.Services
{
    internal sealed class CheckoutRepository
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
        
        public async Task Checkout()
        {
            if (!await _authorizationService.Allowed(_uiDataMapper.Secret))
                throw new IncorrectSecretException();
                
            await _currentStrategy.Checkout(_uiDataMapper.Post());
        }

        public async Task<BlogPost> Get(int year, string titleShrinked)
        {
            // The strategy used does not matter
            return await _currentStrategy.GetPostElseNull(year, titleShrinked);
        }
    }
}
