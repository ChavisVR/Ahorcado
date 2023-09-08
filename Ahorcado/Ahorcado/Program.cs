using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new HttpClient();
        bool JugarDeNuevo = true;

        while (JugarDeNuevo)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://pokeapi.co/api/v2/pokemon?limit=100000&offset=0")
            };

            HttpResponseMessage response;
            using (response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(body);

                JArray resultsArray = (JArray)jsonObject["results"];
                Random random = new Random();
                int randomIndex = random.Next(resultsArray.Count);
                string pokemonName = (string)resultsArray[randomIndex]["name"];

                Console.WriteLine("¡Bienvenido al juego del ahorcado con Pokémon!");
                Console.WriteLine("Adivina el nombre del Pokémon:");
                string guessedName = new string('_', pokemonName.Length);

                int attempts = 6; // Número de intentos permitidos
                char[] incorrectGuesses = new char[attempts];

                while (attempts > 0)
                {
                    Console.WriteLine($"Palabra: {guessedName}");
                    Console.WriteLine($"Intentos restantes: {attempts}");
                    Console.WriteLine("Letras incorrectas: " + new string(incorrectGuesses.Where(c => c != '\0').ToArray()));

                    Console.Write("Ingresa una letra: ");
                    char guess = char.ToLower(Console.ReadKey().KeyChar);

                    if (pokemonName.Contains(guess))
                    {
                        for (int i = 0; i < pokemonName.Length; i++)
                        {
                            if (pokemonName[i] == guess)
                            {
                                guessedName = guessedName.Remove(i, 1).Insert(i, guess.ToString());
                            }
                        }

                        if (guessedName == pokemonName)
                        {
                            Console.WriteLine("\n¡Felicidades, acertaste el Pokémon correctamente!");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nLetra incorrecta. Intenta de nuevo.");
                        attempts--;
                        if (attempts > 0)
                        {
                            Array.Resize(ref incorrectGuesses, attempts);
                            incorrectGuesses[incorrectGuesses.Length - 1] = guess;
                        }
                    }
                }

                if (attempts == 0)
                {
                    Console.WriteLine("\n¡Agotaste tus intentos! El Pokémon era: " + pokemonName);
                }

                Console.Write("¿Quieres seguir jugando? (s/n): ");
                char responseChar = char.ToLower(Console.ReadKey().KeyChar);

                if (responseChar != 's')
                {
                    JugarDeNuevo = false;
                    Console.WriteLine("\n¡Gracias por jugar!");
                }
            }
        }
    }
}

