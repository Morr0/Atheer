using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using AtheerCore;

namespace AtheerEditorApp.Services.AuthorizationService
{
    public class AuthorizationRepository
    {
        private AmazonSimpleSystemsManagementClient _systemsClient;

        public AuthorizationRepository()
        {
            _systemsClient = new AmazonSimpleSystemsManagementClient();
        }
        
        public async Task<bool> Allowed(string secret)
        {
            var getParameterRequest = new GetParameterRequest
            {
                Name = CommonConstants.ATHEER_BLOG_EDIT_SECRET
            };

            var getParameterResponse = await _systemsClient.GetParameterAsync(getParameterRequest);

            return getParameterResponse.Parameter.Value == secret;
        }
    }
}