using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookClient.Data
{
    public class BookManager
    {
        const string Url = "http://bookserver25364.azurewebsites.net/api/books/";
        private string authorizationKey;

        private async Task<HttpClient> GetClient()
        {
            var client = new HttpClient();
            if(string.IsNullOrEmpty(this.authorizationKey))
            {
                authorizationKey = await client.GetStringAsync(Url + "login");
                authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
            }

            client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            var client = await this.GetClient();

            var books = await client.GetStringAsync(Url);
            return JsonConvert.DeserializeObject<IEnumerable<Book>>(books);
        }

        public async Task<Book> Add(string title, string author, string genre)
        {
            var authors = new List<string>
            {
                author
            };

            var newBook = new Book
            {
                ISBN = string.Empty,
                Title = title,
                Authors = authors,
                Genre = genre,
                PublishDate = DateTime.Now
            };

            var client = await this.GetClient();
            var response = await client.PostAsync(
                Url,
                new StringContent(JsonConvert.SerializeObject(newBook),
                Encoding.UTF8,
                "application/json")
            );

            return JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());
        }

        public async Task Update(Book bookToUpdate)
        {
            var client = await this.GetClient();

            await client.PutAsync(
                Url + "/" + bookToUpdate.ISBN,
                new StringContent(JsonConvert.SerializeObject(bookToUpdate),
                Encoding.UTF8,
                "application/json")
            );
        }

        public async Task Delete(string isbn)
        {
            var client = await GetClient();
            await client.DeleteAsync(Url + isbn);
        }
    }
}

