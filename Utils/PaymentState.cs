using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace sipetok_api.Utilis
{
    public enum PaymentState
    {
        Pending,    
        Processing, 
        Success,    
        Failed,     
        Cancelled   
    }
}
