using SmartTaskScheduler.Library.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartTaskScheduler.Library.Core.Services
{
    public class TaskDataService
    {
        private readonly string _filePath;

        public TaskDataService(string filePath = "tasks.txt")
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Сохраняет список задач в текстовый файл
        /// </summary>
        public void SaveTasks(IEnumerable<TaskItem> tasks)
        {
            try
            {
                var lines = new List<string>();

                foreach (var task in tasks)
                {
                    // Формируем строку для каждой задачи
                    var line = $"{task.Title}|{task.Description}|{task.Deadline:yyyy-MM-dd HH:mm:ss}|{task.Priority}|{task.IsCompleted}|{task.CreatedDate:yyyy-MM-dd HH:mm:ss}";
                    lines.Add(line);
                }

                File.WriteAllLines(_filePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении задач: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Загружает список задач из текстового файла
        /// </summary>
        public List<TaskItem> LoadTasks()
        {
            var tasks = new List<TaskItem>();

            if (!File.Exists(_filePath))
                return tasks;

            try
            {
                var lines = File.ReadAllLines(_filePath);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split('|');
                    if (parts.Length >= 6)
                    {
                        var task = new TaskItem
                        {
                            Title = parts[0],
                            Description = parts[1],
                            Deadline = DateTime.Parse(parts[2]),
                            Priority = int.Parse(parts[3]),
                            IsCompleted = bool.Parse(parts[4]),
                            CreatedDate = DateTime.Parse(parts[5])
                        };
                        tasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке задач: {ex.Message}", ex);
            }

            return tasks;
        }

        /// <summary>
        /// Проверяет существование файла с задачами
        /// </summary>
        public bool DataFileExists()
        {
            return File.Exists(_filePath);
        }

        /// <summary>
        /// Удаляет файл с задачами
        /// </summary>
        public void DeleteDataFile()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }
    }
}