using AtheerEditorApp.Constants;

namespace AtheerEditorApp.Services.CheckoutService.Inputs
{
    public abstract class CheckoutInput
    {
        private OperationType _operationType;
        
        public CheckoutInput(OperationType operationType)
        {
            _operationType = operationType;
        }
    }
}