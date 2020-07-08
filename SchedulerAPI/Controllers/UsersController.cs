﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationPlugin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchedulerAPI.Data;
using SchedulerAPI.Models;

namespace SchedulerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        #region Fields
        private readonly SchedulerContext _context;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        #endregion

        public UsersController(SchedulerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _context.Users.Include(r=>r.Role);

            Console.WriteLine(users.First().Role);

            if (users != null)
            {
                return Ok(users);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _context.Users.Include(r=>r.Role).Single(i=>i.Id == id);

            if (user == null) return NotFound("No record found with this id");

            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User user)
        {
            //If model state does not match
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = _context.Users.Include(r => r.Role).SingleOrDefault(i => i.Id == id);

            if (dbUser == null)
            {
                return BadRequest("No record found against this id");
            }

            //Update values
            dbUser.Name = user.Name;
            dbUser.Email = user.Email;
            dbUser.Password = SecurePasswordHasherHelper.Hash(user.Password);
            dbUser.RoleId = user.RoleId;

            //Save changes
            _context.SaveChanges();

            return Ok("Record updated successfully");

        }

        [HttpPost("[action]")]
        public IActionResult Register([FromBody]User user)
        {
            //Ensures unique email
            var userWithSameEmail = _context.Users.Where(u => u.Email == user.Email).SingleOrDefault();
            if (userWithSameEmail != null)
            {
                return BadRequest("User with same email already exists");
            }

            //Ensures unique name
            var userWithSameName = _context.Users.Where(u => u.Name == user.Name).SingleOrDefault();
            if (userWithSameName != null)
            {
                return BadRequest("User with the same name already exists");
            }

            var userObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                RoleId = 0, //defaults to new user
            };

            _context.Users.Add(userObj);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody]User user)
        {
            var userEmail = _context.Users.Include(r=>r.Role).FirstOrDefault(u => u.Email == user.Email);

            if (userEmail == null)
            {
                return NotFound();
            }

            if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
            {
                return Unauthorized();
            }

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Role, userEmail.Role.AuthorizationLevel.ToString())
             };

            var token = _auth.GenerateAccessToken(claims);

            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                userId = userEmail.Id
            });
        }


    }
}