using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TweetApplication.Models;
using TweetApplication.Services;

namespace TweetApplication.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserServices _userServices;
        private ILogger<UserController> _logger;
        public UserController(IUserServices userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            try
            {
                bool? _bool = _userServices.AddUser(user);
                if (_bool == true)
                {
                    return Ok();
                }
                else if(_bool == false)
                {
                    return StatusCode(400);
                }
                else
                {
                    return Ok("Please use a different username. This one has already been taken.");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500); 
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginUser([FromBody] User user)
        {
            try
            {
                if (_userServices.Login(user))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(401);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("{username}/forgot")]
        public IActionResult ForgotPassword([FromBody] User user)
        {
            try
            {
                _userServices.Forgot(user);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500); 
            }
        }

        [HttpGet]
        [Route("users/all")]
        public IActionResult GetAllUsers()
        {
            try
            {
                List<string> users = _userServices.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500); 
            }
        }
    }
}
