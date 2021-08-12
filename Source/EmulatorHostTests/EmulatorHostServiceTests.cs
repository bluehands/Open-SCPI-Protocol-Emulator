// using EmulatorHost;
// using EmulatorHost.NetworkInterface;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using FluentAssertions;
// using Microsoft.Extensions.Logging.Abstractions;
//
// namespace EmulatorHostTests
// {
//     [TestClass]
//     public class EmulatorHostServiceTests
//     {
//         [TestMethod]
//         public async Task StartAsync_StateUnderTest_ExpectedBehavior()
//         {
//             // Arrange
//
//             var socketServer = new TcpServer(new NullLogger<TcpServer>());
//             var service = new EmulatorHostService(new NullLogger<EmulatorHostService>(), socketServer);
//             var cancellationToken = default(global::System.Threading.CancellationToken);
//             var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111);
//
//             // Act
//             await service.StartAsync(
//                 cancellationToken).ConfigureAwait(false);
//
//
//             var client = new TcpClient();
//             client.Connect(endPoint);
//
//             client.Connected.Should().BeTrue();
//             var networkStream = client.GetStream();
//             networkStream.CanWrite.Should().BeTrue();
//             networkStream.CanRead.Should().BeTrue();
//             networkStream.Write(Encoding.ASCII.GetBytes("READ?\n"));
//             var buffer = new byte[100];
//             var bytes = networkStream.Read(buffer);
//
//
//             // Assert
//             Encoding.ASCII.GetString(buffer, 0, bytes).Should().Be("424242");
//             await service.StopAsync(cancellationToken).ConfigureAwait(false);
//         }
//
//         [TestMethod]
//         public async Task StopAsync_StateUnderTest_ExpectedBehavior()
//         {
//             // Arrange
//
//             var socketServer = new TcpServer(new NullLogger<TcpServer>());
//             var service = new EmulatorHostService(new NullLogger<EmulatorHostService>(), socketServer);
//             CancellationToken cancellationToken = default(global::System.Threading.CancellationToken);
//
//             // Act
//             await service.StopAsync(
//                 cancellationToken).ConfigureAwait(false);
//
//             // Assert
//         }
//         [TestMethod]
//         public void Test()
//         {
//         }
//     }
// }
