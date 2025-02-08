using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface IAccountService
    {
        public Account GetAccount(int id);
    }
}