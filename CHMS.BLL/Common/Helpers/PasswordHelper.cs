using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CHMS.BLL.Common.Helpers
{
    public static class PasswordHelper
    {
        // Hàm mã hóa password thành chuỗi ký tự loằng ngoằng
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                // Chuyển string thành byte rồi băm (Hash)
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Chuyển byte ngược lại thành string (Hex)
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Hàm kiểm tra pass (Dùng cho chức năng Login sau này)
        public static bool Verify(string password, string hashedPassword)
        {
            var hashOfInput = Hash(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hashedPassword) == 0;
        }
    }
}
