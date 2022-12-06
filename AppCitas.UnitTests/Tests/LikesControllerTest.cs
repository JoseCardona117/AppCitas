using AppCitas.Service.DTOs;
using AppCitas.UnitTests.Helpers;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppCitas.UnitTests.Tests
{
    public class LikesControllerTest
    {
        private string apiRoute = "api/likes";
        private readonly HttpClient _client;
        private HttpResponseMessage? httpResponse;
        public string requestUrl = String.Empty;
        private string registerObject = String.Empty;
        private string loginObject = String.Empty;
        private HttpContent? httpContent;

        public LikesControllerTest()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("NotFound", "Goku")]
        public async Task AddLike_ShoulBeNotFound(string statusCode, string userliked)
        {

            //Arrange

            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/{userliked}";
            httpContent = null;
            //Act
            httpResponse = await _client.PostAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //like a un usuario 
        [Theory]
        [InlineData("OK")]
        public async Task AddLike_ShouldBe1(string statusCode) 
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/berry";
            httpContent = null;
            //Act
            httpResponse = await _client.PostAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("BadRequest", "lisa")]
        public async Task AddLike_ShouldBeBadRequest(string statusCode, string userliked)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/{userliked}";
            httpContent = null;
            //Act
            httpResponse = await _client.PostAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }


        //liked y likedby
        [Theory]
        [InlineData("OK", "liked")]
        [InlineData("OK", "likedby")]
        public async Task GetLikedUsers_ShouldBeOk(string statusCode, string predicateValue)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}?predicate={predicateValue}";
            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }



        #region Privated methods

        private async Task<string> GetNewUserToLike()
        {
            requestUrl = "api/account/register";
            string finalName = "arturo" + Guid.NewGuid().ToString();
            DateTime date = Convert.ToDateTime("2000 - 01 - 01");
            var registerDto = new RegisterDto
            {
                Username = finalName,
                KnownAs = "Arturo",
                Gender = "male",
                DateOfBirth = date,
                City = "Aguascalientes",
                Country = "Mexico",
                Password = "Pa$$w0rd"
            };

            registerObject = GetRegisterObject(registerDto);
            httpContent = GetHttpContent(registerObject);


            httpResponse = await _client.PostAsync(requestUrl, httpContent);
            return finalName;
        }

        private async Task<AuthenticationHeaderValue> GetAuthoritation()
        {
            requestUrl = "api/account/login";
            var loginDto = new LoginDto
            {
                Username = "lisa",
                Password = "Pa$$w0rd"
            };
            loginObject = GetLoginObject(loginDto);
            httpContent = GetHttpContent(loginObject);

            httpResponse = await _client.PostAsync(requestUrl, httpContent);
            var reponse = await httpResponse.Content.ReadAsStringAsync();
            var userDto = JsonSerializer.Deserialize<UserDto>(reponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new AuthenticationHeaderValue("Bearer", userDto.Token);
        }

        private static string GetRegisterObject(RegisterDto registerDto)
        {
            var entityObject = new JObject()
            {
                { nameof(registerDto.Username), registerDto.Username },
                { nameof(registerDto.KnownAs), registerDto.KnownAs },
                { nameof(registerDto.Gender), registerDto.Gender },
                { nameof(registerDto.DateOfBirth), registerDto.DateOfBirth },
                { nameof(registerDto.City), registerDto.City },
                { nameof(registerDto.Country), registerDto.Country },
                { nameof(registerDto.Password), registerDto.Password }
            };

            return entityObject.ToString();
        }


        private static string GetLoginObject(LoginDto loginDto)
        {
            var entityObj = new JObject()
            {
                {nameof (loginDto.Username), loginDto.Username },
                {nameof (loginDto.Password), loginDto.Password }
            };
            return entityObj.ToString();

        }

        private StringContent GetHttpContent(string objectToEncode)
        {
            return new StringContent(objectToEncode, Encoding.UTF8, "application/json");
        }

        #endregion
    }
}
