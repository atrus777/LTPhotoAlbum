﻿using LTPhotoAlbum.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTPhotoAlbum
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var services = StartUp.ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            string consoleFeedback;

            ShowLaunchDisplay(); 

            consoleFeedback = Console.ReadLine().ToLower().Trim();

            while (consoleFeedback != "exit")
            {
                switch (consoleFeedback)
                {
                    case "home":
                        ShowLaunchDisplay();

                        break;
                    case "album":
                        var albumService = serviceProvider.GetService<IPhotoRepository>();

                        if (albumService == null)
                        {
                            throw new Exception("Couldn't find application service to run");
                        }

                        IEnumerable<int> albums = await albumService.GetAlbumIdsAsync();

                        albums.ToList().ForEach(r =>
                        {
                            Console.WriteLine($"{r}");
                        });

                        ShowOptions();

                        break;
                    case "photo":
                        var photoService = serviceProvider.GetService<IPhotoRepository>();

                        if (photoService == null)
                        {
                            throw new Exception("Couldn't find application service to run");
                        }

                        Console.WriteLine("Please enter an album id to search for and hit enter to continue.");

                        var albumId = Console.ReadLine();
                        var albumValidation = IsValidAlbumId(albumId, (await photoService.GetAlbumIdsAsync()).ToList());

                        if (string.IsNullOrEmpty(albumValidation))
                        {
                            List<PhotoDto> photos = (await photoService.GetPhotosAsync(int.Parse(albumId))).ToList();

                            photos.ForEach(r =>
                            {
                                Console.WriteLine(r.ToString());
                            });

                            Program.ShowOptions();
                        }
                        else
                        {
                            Console.WriteLine(albumValidation);
                            Console.WriteLine("Press enter to continue.");
                            Console.ReadLine();

                            ShowLaunchDisplay();
                        }

                        break;
                    case "help":
                        ShowOptions();

                        break;
                    case "exit":
                        break;
                    default:
                        Console.WriteLine("Invalid option entered.  Please see list and enter a new option.");
                        ShowOptions();

                        break;
                }

                consoleFeedback = Console.ReadLine().ToLower().Trim();
            }
        }

        public static void ShowLaunchDisplay()
        {
            Console.Clear();

            Console.WriteLine("Welcome to your photo album!");

            ShowOptions();

            Console.WriteLine();
            Console.WriteLine("Please type in option above and click 'Enter'.");
        }

        public static void ShowOptions()
        {
            Console.WriteLine();
            Console.WriteLine("Please submit one of the following options:");
            Console.WriteLine();
            Console.WriteLine("home - Navigate back to launch display");
            Console.WriteLine("album - Display photo albums");
            Console.WriteLine("photo - Search for photos in selected album");
            Console.WriteLine("help - Display these options at any time");
            Console.WriteLine("exit - Closes application");
        }

        public static string IsValidAlbumId(string albumId, List<int> allAlbums)
        {
            string message = string.Empty;

            if (int.TryParse(albumId, out int _albumId))
            {
                if (!allAlbums.Contains(_albumId))
                {
                    message = $"Album {_albumId} does not exist.  Please search for albums between {allAlbums.Min()} and {allAlbums.Max()}.";
                }
            }
            else
            {
                message = $"Search option entered, '{albumId}', is not a valid whole number.";
            }

            return message;
        }

    }
}
