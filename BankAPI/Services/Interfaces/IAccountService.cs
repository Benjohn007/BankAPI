using BankAPI.Models;
using System.Collections.Generic;

namespace BankAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Account Authenticate(string AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        void Delete(int Id);
        Account GetById(int Id);
        void Update(Account account, string Pin = null);
        Account GetByAccountNumber(string AccountNumber);
    }
}
