using AutoMapper;
using BankAPI.DTOs;
using BankAPI.Models;
using BankAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BankAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        //now, lets inject the AccountService
        //and also bring the Automapper
        public AccountsController(IAccountService accountService,IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register_new_account")]
        public IActionResult RegisterNewAccount([FromBody] RegisterNewAccountModel newAccount)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(newAccount);

                //map RegisterNewAccountModel to Account
                var account = _mapper.Map<Account>(newAccount);
                return Ok(_accountService.Create(account, newAccount.Pin, newAccount.ConfirmPin));
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                   ex.Message);
            }
        }

        [HttpGet]
        [Route("get_all_accounts")]
        public IActionResult GetAllAccounts()
        {
            //we want to map to GetAccountModel as defined in Our AutoMapperProfile

            var accounts = _accountService.GetAllAccounts();
            var cleanedAccounts = _mapper.Map<IList<GetAccountModel>>(accounts);
            return Ok(cleanedAccounts);
        }

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] AuthencateModel authmodel)
        {
            if(!ModelState.IsValid)
                return BadRequest(authmodel);
            //now lets map
            return Ok(_accountService.Authenticate(authmodel.AccountNumber, authmodel.Pin));
            //it returns an account... let's see when we run before we know whether to map or not
        }

        [HttpGet]
        [Route("get_by_account_number")]
        public IActionResult GetByAccountNumber(string AccountNumber)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account Number must be 10-digits");
            var account = _accountService.GetByAccountNumber(AccountNumber);
            var cleanedAccount = _mapper.Map<GetAccountModel>(account);
            return Ok(cleanedAccount);
        }

        [HttpGet]
        [Route("get_account_by_id")]
        public IActionResult GetAccountById(int id)
        {
            var account = _accountService.GetById(id);
            var cleanedAccount = _mapper.Map<GetAccountModel>(account);
            return Ok(cleanedAccount);
        }

        [HttpPut, Route("update_account")]
        public IActionResult UpdateAccount([FromBody] UpdateAccountModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(model);
            var account = _mapper.Map<Account>(model);
            _accountService.Update(account, model.Pin);
            return NoContent();
        }
    }
}
