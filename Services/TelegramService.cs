using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yerevan_Housing_API.Helpers;
using Yerevan_Housing_API.Models;

namespace Yerevan_Housing_API.Services
{
    public class TelegramService
    {
        private readonly string _token;
        private readonly string _chatId;
        private readonly TelegramBotClient Bot;

        public TelegramService()
        {
            _token = Startup.BotApiKey;
            _chatId = Startup.ChatId;
            Bot = new TelegramBotClient(_token);
        }
        public async Task SendCaption(House house)
        {
            try
            {
                var price = (house.Rent / 0.9m);
                var value2 = Math.Round((decimal)price / 10000) * 10000;
                var notarPrice = String.Format("{0:n0}", value2);
                var actualPrice = String.Format("{0:n0}", house.Rent);
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"<b>Адрес: </b> {house.Address}");
                stringBuilder.AppendLine($"<b>Количество комнат:</b> {house.RoomCount}");
                stringBuilder.AppendLine($"<b>Площадь: </b>{house.Area} м²");
                stringBuilder.AppendLine($"<b>Домашние животные: </b>{house.Animals}");
                stringBuilder.AppendLine($"<b>Минимальный период: </b>{house.MinimalPeriod}");
                stringBuilder.AppendLine($"<b>Этаж: </b>{house.Floor}");
                stringBuilder.AppendLine($"<b>Аренда: </b>{actualPrice} драм/Месяц ({notarPrice} драм/месяц с нотариальным договором) ");
                stringBuilder.AppendLine($"<b>Контакты: </b>{house.Name}");
                stringBuilder.AppendLine($"<b>Код:</b> YB#{house.Id}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"<i> Оплата за коммунальные производится отдельно </i>");
                stringBuilder.AppendLine($"<i> Комиссия риэлтора - {house.Percent}% от стоимости месячного проживания </i>");

                var message = stringBuilder.ToString();
                try
                {
                    List<IAlbumInputMedia> allMedia = new List<IAlbumInputMedia>();
                    var i = 1;

                    foreach (var file in house.Media.Photos)
                    {
                        file.Position = 0;
                        if (i != 1)
                        {
                            IAlbumInputMedia media = new InputMediaPhoto(new InputMedia(file, $"image_{i}.jpg"))
                            {
                                ParseMode = ParseMode.Html
                            };
                            allMedia.Add(media);
                        }
                        else
                        {
                            IAlbumInputMedia media = new InputMediaPhoto(new InputMedia(file, $"image_{i}.jpg"))
                            {
                                Caption = message,
                                ParseMode = ParseMode.Html
                            };
                            allMedia.Add(media);
                        }
                        i++;

                    }
                    var fileStreams = new List<FileStream>();
                    var j = 0;
                    foreach (var video in house.Media.Videos)
                    {
                        var fileName = $"video_{j}.mp4";
                        var fsStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        fileStreams.Add(fsStream);
                        if (i != 1)
                        {
                            IAlbumInputMedia media = new InputMediaVideo(new InputMedia(fsStream, fileName));
                            allMedia.Add(media);
                            j++;
                        }
                        else
                        {
                            IAlbumInputMedia media = new InputMediaVideo(new InputMedia(fsStream, fileName))
                            {
                                ParseMode = ParseMode.Html,
                                Caption = message
                            };
                            allMedia.Add(media);
                            j++;
                        }
                        i++;

                    }
                    if (allMedia.Count > 0)
                    {
                        var response = Bot.SendMediaGroupAsync(_chatId, allMedia);
                        response.Wait(10000);
                        var k = 0;
                        foreach (var item in fileStreams)
                        {
                            var fileName = $"video_{k}.mp4";
                            item.Close();
                            System.IO.File.Delete(fileName);
                            k++;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //Bot.StartReceiving();

                    // Bot.StopReceiving();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
