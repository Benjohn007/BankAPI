﻿using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace BankAPI.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private BankDbContext _dbContext;
        public AccountService(BankDbContext bankDbContext)
        {
            _dbContext = bankDbContext;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if(account == null)
                return null;

            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                return null;
            return account;

        }


        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");
            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i =0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            //Create a new Acccount
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An Account already exist with this email");
            //Pin validation
            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pin does not match", "Pin");

            //hashing and encrypting pin
            byte[] pinHash, pinSalt;
            CreatedPinHash(Pin, out pinHash, out pinSalt);//crypto method

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;  

            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            return account;
        }
        private static void CreatedPinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinHash = hmac.Key;
                pinSalt = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));    
            }
        }

        public void Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if(account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAllAccounts()
        {
             return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if(account == null) return null;

            return account;
        }

        public Account GetById(int Id)
        {
            var account = _dbContext.Accounts.Where(x => x.Id == Id).FirstOrDefault();
            if(account == null) return null;

            return account;
        }

        public void Update(Account account, string Pin = null)
        {
            var accountToBeUpdate = _dbContext.Accounts.Where(x => x.Email == account.Email).FirstOrDefault();
            if (accountToBeUpdate == null) throw new ApplicationException("Account does not exist");
            if (string.IsNullOrEmpty(account.Email))
            {
                if(_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("This Email " +
                    account.Email + " already exist");

                accountToBeUpdate.Email = account.Email;
            }

            if (string.IsNullOrEmpty(account.PhoneNumber))
            {
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber)) throw new ApplicationException("This Email " +
                    account.PhoneNumber + " already exist");

                accountToBeUpdate.PhoneNumber = account.PhoneNumber;
            }

            if (string.IsNullOrEmpty(Pin))
            {
                byte[] pinHash, pinSalt;
                CreatedPinHash(Pin, out pinHash, out pinSalt);

                accountToBeUpdate.PinHash = pinHash;
                accountToBeUpdate.PinSalt = pinSalt;
            }
            accountToBeUpdate.DateLastUpdated = DateTime.Now;

            _dbContext.Accounts.Update(accountToBeUpdate);
            _dbContext.SaveChanges();
        }
    }
}
