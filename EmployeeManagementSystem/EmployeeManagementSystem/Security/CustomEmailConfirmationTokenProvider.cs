using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace EmployeeManagementSystem.Security
{
    public class CustomEmailConfirmationTokenProvider<TUser> :
        DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<DataProtectionTokenProviderOptions> options, 
            ILogger<DataProtectorTokenProvider<TUser>> logger) : 
            base(dataProtectionProvider, options, logger)
        {

        }
    }
}
