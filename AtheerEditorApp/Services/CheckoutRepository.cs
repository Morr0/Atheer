using AtheerEditorApp.Constants;
using AtheerEditorApp.Services.Strategies;
using System.Threading.Tasks;

namespace AtheerEditorApp.Services
{
    internal class CheckoutRepository
    {
        public async Task Checkout(OperationType operationType, UIDataMapper uiDataMapper)
        {
            if (operationType == OperationType.New)
                await new NewPostCheckoutStrategy().Checkout(uiDataMapper.Post());
        }
    }
}
