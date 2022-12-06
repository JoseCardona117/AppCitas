using AppCitas.Service.DTOs;
using AppCitas.UnitTests.Helpers;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppCitas.UnitTests.Tests
{
    public class MessageControllerTest
    {
        private string apiRoute = "api/messages";
        private readonly HttpClient _client;
        private HttpResponseMessage? httpResponse;
        public string requestUrl = String.Empty;
        private string registerObject = String.Empty;
        private string loginObject = String.Empty;
        private HttpContent? httpContent;

        public MessageControllerTest()
        {
            _client = TestHelper.Instance.Client;
        }

        //Mensaje a sí 
        [Theory]
        [InlineData("BadRequest", "lisa", "Hola, yo xd")]
        public async Task SendMessage_ShouldBeBadRequest(string statusCode, string sendTo, string content)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}";
            var message = new MessageCreateDto
            {
                RecipientUsername = sendTo,
                Content = content
            };
            registerObject = GetMessageCreate(message);
            httpContent = GetHttpContent(registerObject);
            //Act
            httpResponse = await _client.PostAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //Enviando a persona no registrada

        [Theory]
        [InlineData("NotFound", "Goku", "¡Ya basta, Freezer!")]
        public async Task SendMessage_ShouldBeNotFound(string statusCode, string sendTo, string content)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}";
            var message = new MessageCreateDto
            {
                RecipientUsername = sendTo,
                Content = content
            };
            registerObject = GetMessageCreate(message);
            httpContent = GetHttpContent(registerObject);
            //Act
            httpResponse = await _client.PostAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //Enviar mensaje a todd
        [Theory]
        [InlineData("OK", "todd", "Hola, Todd, saca el gallo")]
        public async Task SendMessage_ShouldBeOk(string statusCode, string sendTo, string content)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}";
            var message = new MessageCreateDto
            {
                RecipientUsername = sendTo,
                Content = content
            };
            registerObject = GetMessageCreate(message);
            httpContent = GetHttpContent(registerObject);
            //Act
            httpResponse = await _client.PostAsync(requestUrl, httpContent);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //Obteniendo los mensajes default
        [Theory]
        [InlineData("OK")]
        public async Task GetMessageDeafult_ShouldBeOk(string statusCode)
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

        [Theory]
        [InlineData("OK", "outbox")]
        [InlineData("OK", "Inbox")]
        public async Task GetMessageVariable_ShouldBeOk(string statusCode, string containerValue)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}?container={containerValue}";

            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        //todos los mensajes
        [Theory]
        [InlineData("OK", "todd")]
        public async Task GetMessageThread_ShouldBeOk(string statusCode, string otherPerson)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/thread/{otherPerson}";

            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("MethodNotAllowed", "")]
        public async Task GetMessageThread_ShouldBeNotAllowed(string statusCode, string error)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/thread/{error}";

            //Act
            httpResponse = await _client.GetAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }


        [Theory]
        [InlineData("OK", "3")]
        public async Task DelateMessage_ShouldBeOk(string statusCode, string idMessage)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/{idMessage}";

            //Act
            httpResponse = await _client.DeleteAsync(requestUrl);

            //Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }


        [Theory]
        [InlineData("MethodNotAllowed", "")]
        public async Task DelateMessage_ShouldBeNotAllowed(string statusCode, string idMessage)
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = await GetAuthoritation();
            requestUrl = $"{apiRoute}/{idMessage}";

            //Act
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

        private static string GetMessageCreate(MessageCreateDto messageCreateDto)
        {
            var entityObject = new JObject()
            {
                { nameof(messageCreateDto.RecipientUsername), messageCreateDto.RecipientUsername},
                { nameof(messageCreateDto.Content), messageCreateDto.Content },

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
