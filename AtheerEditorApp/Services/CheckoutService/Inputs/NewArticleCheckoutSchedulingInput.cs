using System;
using AtheerEditorApp.Constants;

namespace AtheerEditorApp.Services.CheckoutService.Inputs
{
    public class NewArticleCheckoutSchedulingInput : CheckoutInput
    {
        public NewArticleCheckoutSchedulingInput(DateTime dateTime) : base(OperationType.New)
        {
            ScheduleDate = dateTime;
        }
        
        public DateTime ScheduleDate { get; }
    }
}