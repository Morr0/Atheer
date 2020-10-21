using Microsoft.Extensions.Configuration;

namespace AtheerCore
{
    public class ConstantsLoader
    {
        private static string ATHEER_BLOGPOST_TABLE_NAME = "Atheer-Post-TableName";
        
        public ConstantsLoader(IConfiguration configuration)
        {
            if (configuration == null)
            {
                BlogPostTableName = CommonConstants.BLOGPOST_TABLE;
                return;
            }
            
            string postTableName = configuration[ATHEER_BLOGPOST_TABLE_NAME];
            if (postTableName == null)
                postTableName = CommonConstants.BLOGPOST_TABLE;
            BlogPostTableName = postTableName;

        }
        
        public string BlogPostTableName { get; }

        public string BlogPostTableTTLAttribute => CommonConstants.BLOGPOST_TABLE_TTL_ATTRIBUTE;

        public string BlogPostEditSecretName => CommonConstants.ATHEER_BLOG_EDIT_SECRET;
    }
}