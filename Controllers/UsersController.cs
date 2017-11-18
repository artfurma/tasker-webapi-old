using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskerWebAPI.DTO;
using TaskerWebAPI.Helpers;
using TaskerWebAPI.Models;
using TaskerWebAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskerWebAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class UsersController : Controller
	{
		private IUserService _userService;
		private IMapper _mapper;
		private readonly AppSettings _appSettings;

		public UsersController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
		{
			_userService = userService;
			_mapper = mapper;
			_appSettings = appSettings.Value;
		}

		[AllowAnonymous]
		[HttpPost("authenticate")]
		public IActionResult Authenticate([FromBody] UserDTO UserDTO)
		{
			var user = _userService.Authenticate(UserDTO.Username, UserDTO.Password);

			if (user == null)
				return Unauthorized();

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.Id.ToString())
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			// return basic user info (without password) and token to store client side
			return Ok(new
			{
				Id = user.Id,
				Username = user.Username,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Token = tokenString
			});
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult Register([FromBody] UserDTO UserDTO)
		{
			// map dto to entity
			var user = _mapper.Map<User>(UserDTO);

			try
			{
				// save 
				_userService.Create(user, UserDTO.Password);
				return Ok();
			}
			catch (AppException ex)
			{
				// return error message if there was an exception
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var users = _userService.GetAll();
			var UserDTOs = _mapper.Map<IList<UserDTO>>(users);
			return Ok(UserDTOs);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var user = _userService.GetById(id);
			var UserDTO = _mapper.Map<UserDTO>(user);
			return Ok(UserDTO);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] UserDTO UserDTO)
		{
			// map dto to entity and set id
			var user = _mapper.Map<User>(UserDTO);
			user.Id = id;

			try
			{
				// save 
				_userService.Update(user, UserDTO.Password);
				return Ok();
			}
			catch (AppException ex)
			{
				// return error message if there was an exception
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			_userService.Delete(id);
			return Ok();
		}
	}
}