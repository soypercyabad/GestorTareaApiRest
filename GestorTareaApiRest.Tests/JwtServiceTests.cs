using GestorTareaApiRest.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorTareaApiRest.Tests
{
    [TestClass]
    public class JwtServiceTests
    {
        [TestMethod]
        public void GenerarToken_DeberiaRetornarTokenValido()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "K3ys3cr3t!JWT-W3bC0ntr0l2025#S3gUr4@API" },
                    { "Jwt:Issuer", "GestorTareasAPI" },
                    { "Jwt:Audience", "GestorTareasUsuario" },
                    { "Jwt:ExpireMinutes", "60" }
                })
                .Build();

            var jwtService = new JwtService(config);
            var token = jwtService.GenerarToken(1, "percy");

            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
            Console.WriteLine("Token generado: " + token);
        }
    }
}
