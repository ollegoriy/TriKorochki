using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

[Serializable]
[XmlRoot("Cars")]
public class Car
{
    [XmlElement("Brand")]
    public string Brand { get; set; }

    [XmlElement("Year")]
    public int Year { get; set; }

    [XmlElement("Price")]
    public double Price { get; set; }

    public Car(string brand, int year, double price)
    {
        Brand = brand;
        Year = year;
        Price = price;
    }

    public Car()
    {
    }
}

public class FileManager
{
    private string filePath;

    public FileManager(string path)
    {
        filePath = path;
    }

    public List<Car> LoadFile()
    {
        List<Car> cars = new List<Car>();

        try
        {
            if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath);

                if (extension == ".txt")
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        while (!sr.EndOfStream)
                        {
                            Car car = new Car
                            {
                                Brand = sr.ReadLine()
                            };
                            if (int.TryParse(sr.ReadLine(), out int year))
                            {
                                car.Year = year;
                            }
                            if (double.TryParse(sr.ReadLine(), out double price))
                            {
                                car.Price = price;
                            }
                            cars.Add(car);
                        }
                    }
                }
                else if (extension == ".json")
                {
                    string json = File.ReadAllText(filePath);
                    cars = JsonConvert.DeserializeObject<List<Car>>(json);
                }
                else if (extension == ".xml")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        cars = (List<Car>)serializer.Deserialize(fs);
                    }
                }
                else
                {
                    Console.WriteLine("Неподдерживаемый формат файла.");
                }
            }
            else
            {
                Console.WriteLine("Файл не существует.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }

        return cars;
    }

    public void SaveFile(List<Car> cars)
    {
        try
        {
            Console.WriteLine("Введите путь для сохранения файла:");
            string savePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(savePath))
            {
                Console.WriteLine("Путь не указан. Файл не сохранен.");
                return;
            }

            string extension = Path.GetExtension(savePath);

            if (extension == ".xml")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    serializer.Serialize(fs, cars);
                    Console.WriteLine("Данные успешно сохранены в формате XML.");
                }
            }
            else if (extension == ".json")
            {
                string json = JsonConvert.SerializeObject(cars);
                File.WriteAllText(savePath, json);
                Console.WriteLine("Данные успешно сохранены в формате JSON.");
            }
            else if (extension == ".txt")
            {
                using (StreamWriter sw = new StreamWriter(savePath))
                {
                    foreach (var car in cars)
                    {
                        sw.WriteLine("Машина");
                        sw.WriteLine(car.Brand);
                        sw.WriteLine(car.Year);
                        sw.WriteLine(car.Price);
                    }
                    Console.WriteLine("Данные успешно сохранены в текстовом формате.");
                }
            }
            else
            {
                Console.WriteLine("Неподдерживаемый формат файла.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }


}

public class ConsoleEditor
{
    public void DisplayCarInfo(Car car)
    {
        Console.WriteLine("Информация об автомобиле:");
        Console.WriteLine("Марка: " + car.Brand);
        Console.WriteLine("Год: " + car.Year);
        Console.WriteLine("Цена: " + car.Price);
    }

    public void EditCarInfo(Car car)
    {
        Console.WriteLine("Выберите свойство для редактирования (1: Марка, 2: Год, 3: Цена, 0: Выход):");
        int choice = 0;
        if (int.TryParse(Console.ReadLine(), out choice))
        {
            switch (choice)
            {
                case 1:
                    Console.Write("Введите новую марку: ");
                    car.Brand = Console.ReadLine();
                    break;
                case 2:
                    Console.Write("Введите новый год: ");
                    int новыйГод;
                    if (int.TryParse(Console.ReadLine(), out новыйГод))
                    {
                        car.Year = новыйГод;
                    }
                    else
                    {
                        Console.WriteLine("Неверный ввод.");
                    }
                    break;
                case 3:
                    Console.Write("Введите новую цену: ");
                    double новаяЦена;
                    if (double.TryParse(Console.ReadLine(), out новаяЦена))
                    {
                        car.Price = новаяЦена;
                    }
                    else
                    {
                        Console.WriteLine("Неверный ввод.");
                    }
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к файлу: ");
        string filePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath);
        List<Car> cars = fileManager.LoadFile();

        ConsoleEditor consoleEditor = new ConsoleEditor();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Информация о загруженных машинах:");

            for (int i = 0; i < cars.Count; i++)
            {
                Console.WriteLine($"Машина {i + 1}:");
                consoleEditor.DisplayCarInfo(cars[i]);
                Console.WriteLine();
            }

            Console.WriteLine("Нажмите F1 для сохранения, Escape для выхода, или любую клавишу для редактирования.");
            ConsoleKey key = Console.ReadKey().Key;

            if (key == ConsoleKey.F1)
            {
                fileManager.SaveFile(cars);
                Console.WriteLine("Файл сохранен.");
            }
            else if (key == ConsoleKey.Escape)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Выберите запись для редактирования (1, 2, 3):");

                for (int i = 0; i < cars.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {cars[i].Brand}");
                }

                if (int.TryParse(Console.ReadLine(), out int selection) && selection >= 1 && selection <= cars.Count)
                {
                    consoleEditor.EditCarInfo(cars[selection - 1]);
                }
                else
                {
                    Console.WriteLine("Неверный выбор.");
                }
            }
        }
    }
}
