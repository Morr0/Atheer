using AtheerBackend.Models;
using Microsoft.Extensions.Configuration;

namespace AtheerBackend.Utilities
{
    public class ConstantsLoader
    {
        // Config keys
        private const string ATHEER_BLOGPOST_TABLE_NAME = "Atheer-Post-TableName";
        private const string ATHEER_CONTACT_TABLE_NAME = "Atheer-Contact-TableName";
        
        public ConstantsLoader(IConfiguration configuration)
        {
            if (configuration == null)
            {
                BlogPostTableName = CommonConstants.BLOGPOST_TABLE;
                return;
            }
            
            string postTableName = configuration[ATHEER_BLOGPOST_TABLE_NAME];
            BlogPostTableName = postTableName ?? CommonConstants.BLOGPOST_TABLE;

            string contactTableName = configuration[ATHEER_CONTACT_TABLE_NAME];
            ContactTableName = contactTableName ?? CommonConstants.CONTACT_TABLE;
        }
        
        public string BlogPostTableName { get; }

        public string BlogPostTableTTLAttribute => CommonConstants.BLOGPOST_TABLE_TTL_ATTRIBUTE;

        public string BlogPostEditSecretName => CommonConstants.ATHEER_BLOG_EDIT_SECRET;
        
        public string ContactTableName { get; }
        public string ContactTablePrimaryKey => nameof(Contact.Id);
    }
}