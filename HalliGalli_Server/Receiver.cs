using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    class Receiver
    {
        public static Receiver Instance { get; } = new Receiver();

        public T ReceiveJson<T>(StreamReader reader)
        {
            string str = reader.ReadLine();
            Console.WriteLine("받은 json: " + str);
            try
            {
                var options = new JsonSerializerOptions
                {
                    IncludeFields = true,
                    PropertyNameCaseInsensitive = true
                };

                T data = JsonSerializer.Deserialize<T>(str, options);
                Console.WriteLine("=== 수신된 원본 JSON ===");
                Console.WriteLine(data.ToString());
                Console.WriteLine("========================");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("파싱 실패: " + ex.Message);
                return default(T); // Ensure all code paths return a value
            }
        }
    }
}
