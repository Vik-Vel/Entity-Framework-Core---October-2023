using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MusicHub
{
    using System;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
            
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumInfo = context.Producers
                .Include(x => x.Albums)
                .ThenInclude(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .First(x => x.Id == producerId)
                .Albums.Select(x => new
                {
                   AlbumName = x.Name,
                    RealeseDate = x.ReleaseDate,
                    ProducerName = x.Producer.Name,
                    AlbumSongs = x.Songs.Select(x => new
                        {
                            SongName = x.Name,
                            SongPrice = x.Price,
                            SongWritetName = x.Writer.Name
                        }).OrderByDescending(x => x.SongName)
                        .ThenBy(x => x.SongWritetName),
                    TotalAlbumPrice = x.Price
                }).OrderByDescending(x => x.TotalAlbumPrice).AsEnumerable();

            StringBuilder sb = new StringBuilder();

            foreach (var album in albumInfo)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}")
                    .AppendLine($"-ReleaseDate: {album.RealeseDate.ToString("MM/dd/yyyy")}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");

              
                    int count = 1;
                    foreach (var song in album.AlbumSongs)
                    {
                        sb
                            .AppendLine($"---#{count++}")
                            .AppendLine($"---SongName: {song.SongName}")
                            .AppendLine($"---Price: {song.SongPrice:f2}")
                            .AppendLine($"---Writer: {song.SongWritetName}");
                    }
                    
                sb
                    .AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:f2}");
            }


            return sb.ToString().TrimEnd();


            //Дава грешка нълл, за това го правим по горният начин
            //var albumInfo = context.Producers
            //    .FirstOrDefault(x => x.Id == producerId)
            //    .Albums.Select(x => new
            //    {
            //        x.Name,
            //        RealeseDate = x.ReleaseDate,
            //        ProducerName = x.Producer.Name,
            //        AlbumSongs = x.Songs.Select(x => new
            //        {
            //            x.Name,
            //            x.Price,
            //            SongWritetName = x.Writer.Name
            //        }).OrderByDescending(x => x.Name).ThenBy(x => x.SongWritetName),
            //        TotalAlbumPrice = x.Price
            //    }).OrderByDescending(x => x.TotalAlbumPrice);





        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var span = new TimeSpan(0, 0, duration);

            var songsAboveDuration = context
                .Songs
                .Where(s => s.Duration > span)
                .Select(s => new
                {
                    SongName = s.Name,
                    PerformerFullName = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .OrderBy(name => name)
                        .ToList(),
                    WriterName = s.Writer.Name,
                    AlbumProducerName = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (var s in songsAboveDuration)
            {
                sb
                    .AppendLine($"-Song #{counter++}")
                    .AppendLine($"---SongName: {s.SongName}")
                    .AppendLine($"---Writer: {s.WriterName}");

                if (s.PerformerFullName.Any())
                {
                    sb.AppendLine(string
                        .Join(Environment.NewLine, s.PerformerFullName
                            .Select(p => $"---Performer: {p}")));
                }

                sb
                    .AppendLine($"---AlbumProducer: {s.AlbumProducerName}")
                    .AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().TrimEnd();

            //var songs = context.Songs
            //    .Include(s => s.SongPerformers)
            //    .ThenInclude(sp => sp.Performer)
            //    .Include(s => s.Writer)
            //    .Include(s => s.Album)
            //    .ThenInclude(a => a.Producer)
            //    .ToList() //AsEnumerable()//ToArray //Fetch/izvlicha/ all songs to memory, otherwise filtering will fall
            //    .Where(s => s.Duration.TotalSeconds > duration)
            //    .Select(s => new
            //    {
            //        s.Name,
            //        Performers = s.SongPerformers
            //            .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
            //            .ToList(),
            //        WriterName = s.Writer.Name,
            //        AlbumProducer = s.Album.Producer.Name,
            //        Duration = s.Duration.ToString("c")
            //    })
            //    .OrderBy(s => s.Name)
            //    .ThenBy(s => s.WriterName)
            //    .ToList();



            //StringBuilder sb = new StringBuilder();

            //int count = 1;
            //foreach (var song in songs)
            //{

            //    sb.AppendLine($"-Song #{count++}")
            //        .AppendLine($"---SongName: {song.Name}")
            //        .AppendLine($"---Writer: {song.WriterName}")
            //        .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
            //        .AppendLine($"---Duration: {song.Duration}");

            //    if (song.Performers.Any())
            //    {
            //        sb.AppendLine($"---Performer: {string.Join(", ", song.Performers)}");
            //    }
            //}

            //return sb.ToString().Trim();


        }
    }
}
