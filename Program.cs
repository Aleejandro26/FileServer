using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileServer {
    class Program {
        static void Main(string[] args) {
            StartServer();
        }

        static void StartServer() {
            TcpListener server = null;
            try
            {
                // Establece la dirección IP y el puerto para el servidor
                int port = 8888;
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

                // Crea un objeto TcpListener
                server = new TcpListener(ipAddress, port);

                // Inicia el servidor
                server.Start();

                Console.WriteLine("Servidor de archivos iniciado. Esperando conexiones...");

                // Bucle de escucha para aceptar clientes
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    Console.WriteLine("Cliente conectado. Esperando solicitud...");

                    // Procesa la solicitud del cliente en un hilo separado
                    System.Threading.ThreadPool.QueueUserWorkItem(ProcessClientRequest, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Detiene el servidor
                server.Stop();
            }
        }

        static void ProcessClientRequest(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = null;

            try
            {
                // Obtiene la referencia al flujo de red del cliente
                stream = client.GetStream();

                // Buffer para almacenar los datos recibidos del cliente
                byte[] buffer = new byte[1024];
                int bytesRead;

                // Lee la solicitud del cliente
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string request = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Solicitud recibida del cliente: " + request);

                // Procesa la solicitud (por ejemplo, lee un archivo del sistema de archivos)
                string filePath = "ruta/del/archivo";
                byte[] fileData = File.ReadAllBytes(filePath);

                // Envía los datos del archivo al cliente
                stream.Write(fileData, 0, fileData.Length);
                Console.WriteLine("Archivo enviado al cliente.");

                // Cierra la conexión
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar la solicitud del cliente: " + ex.Message);
            }
            finally
            {
                // Asegúrate de cerrar el flujo de red
                if (stream != null)
                {
                    stream.Close();
                }           
            }
        }
    }
}
