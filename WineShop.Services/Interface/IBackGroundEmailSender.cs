using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineShop.Services.Interface
{
    public interface IBackGroundEmailSender
    {
        Task DoWork();
    }
}
