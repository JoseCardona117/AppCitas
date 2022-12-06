using AppCitas.Service.DTOs;
using AppCitas.UnitTests.Helpers;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppCitas.UnitTests.Tests
{
    public class UsersControllerTest
    {
        private string apiRoute = "api/users";
        private readonly HttpClient _client;
        private HttpResponseMessage? httpResponse;
        public string requestUrl = String.Empty;
        private string registerObject = String.Empty;
        private string loginObject = String.Empty;
        private HttpContent? httpContent;

        public UsersControllerTest()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("OK")]
        public async Task GetUsers_ShouldBeOK(string statusCode)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}";

            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //Obteniendo usuario
        [Theory]
        [InlineData("OK", "lisa")]
        public async Task GetAUser_ShouldBeOK(string statusCode, string user)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/{user}";

            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //Usuario inexistente
        [Theory]
        [InlineData("NoContent", "lis")]
        public async Task GetAUser_ShouldBeNoContent(string statusCode, string user)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/{user}";

            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("NoContent", "Hi I like read", "Be Happy", "Games", "Aguascalientes", "Mexico")]
        public async Task UpdateUser_ShouldBeNoContent(string statusCode, string introduccion, string lokfor, string intereses, string cuidad, string pais)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}";
            var updateDto = new MemberUpdateDto
            {
                Introduction = introduccion,
                LookingFor = lokfor,
                Interests = intereses,
                City = cuidad,
                Country = pais
            };

            registerObject = GetMemberUpdateObject(updateDto);
            httpContent = GetHttpContent(registerObject);
            //Act
            httpResponse = await _client.PutAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("Created", @"D:\Imagenes\Saved Pictures\pintura miguel coco.jpg")]
        public async Task AddPhoto_ShouldBeOk(string statusCode, string FileRoute)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/add-photo";
            var filename = Path.GetFileName(FileRoute);

            using var requestContent = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(FileRoute);
            requestContent.Add(new StreamContent(fileStream), "file", filename);

            //Action
            httpResponse = await _client.PostAsync(requestUrl, requestContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("BadRequest", @"D:\Documentos\Libros\El Principito.pdf")]
        public async Task AddPhoto_ShouldBeBadRequest(string statusCode, string FileRoute)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/add-photo";
            var filename = Path.GetFileName(FileRoute);

            using var requestContent = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(FileRoute);
            requestContent.Add(new StreamContent(fileStream), "file", filename);

            //Action
            httpResponse = await _client.PostAsync(requestUrl, requestContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());


        }


        [Theory]
        [InlineData("NotFound", "13")]
        public async Task SetMainPhoto_ShouldBeNotFound(string statusCode, string newMainPhotoId)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/set-main-photo/{newMainPhotoId}";
            httpContent = null;

            //Action
            httpResponse = await _client.PutAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("MethodNotAllowed", "")]
        public async Task SetMainPhoto_ShouldBeNotAllowed(string statusCode, string newMainPhotoId)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/set-main-photo/{newMainPhotoId}";
            httpContent = null;

            //Action
            httpResponse = await _client.PutAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("MethodNotAllowed", "")]
        public async Task DelatePhoto_ShouldBeNotAllowed(string statusCode, string DelatePhotoId)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/delete-photo/{DelatePhotoId}";

            //Action
            httpResponse = await _client.DeleteAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("NotFound", "13")]
        public async Task DelatePhoto_ShouldBeNotFound(string statusCode, string DelatePhotoId)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/delete-photo/{DelatePhotoId}";

            //Action
            httpResponse = await _client.DeleteAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("BadRequest", "1")] 
        public async Task DelatePhoto_ShouldBeBadRequest(string statusCode, string DelatePhotoId)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/delete-photo/{DelatePhotoId}";

            //Action
            httpResponse = await _client.DeleteAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "11")] //Fotos de lisa
        public async Task DelatePhoto_ShouldBeOK(string statusCode, string DelatePhotoId)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/delete-photo/{DelatePhotoId}";

            //Action
            httpResponse = await _client.DeleteAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        #region Privated methods        
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

        private static string GetMemberUpdateObject(MemberUpdateDto updateDto)
        {
            var entityObj = new JObject()
            {
                {nameof (updateDto.Introduction), updateDto.Introduction },
                {nameof (updateDto.LookingFor), updateDto.LookingFor },
                {nameof (updateDto.Interests), updateDto.Interests },
                {nameof (updateDto.City), updateDto.City },
                {nameof (updateDto.Country), updateDto.Country }
            };
            return entityObj.ToString();
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
