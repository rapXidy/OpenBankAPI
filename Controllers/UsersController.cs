﻿using AutoMapper;
using BankAPI.DTOs;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IJWTManagerRepository _jWTManager;
        public UsersController(IUserRepository _userRepo, IMapper _mapper, IJWTManagerRepository jWTManager)
        {
            this._userRepo = _userRepo;
            this._mapper = _mapper;
            _jWTManager = jWTManager;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AuthenticateJWT")]
        public IActionResult AuthenticateJWT(AuthenticateModel usersdata)
        {
            var token = _jWTManager.Authenticate(usersdata);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        // GET api/<UsersController>/5
        [HttpGet]
        [Route("Get_Auth_User")]
        public IActionResult GetAuthUser(string username, string password)
        {
            if (username == null || password == null)
                return BadRequest();
            var user = _userRepo.GetUser(username,password);
            var cleanedUser = _mapper.Map<UserReadDTO>(user);
            return Ok(cleanedUser);
        }

        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> GetUsers()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<UsersController>
        [HttpPost]
        public void CreateUser([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
