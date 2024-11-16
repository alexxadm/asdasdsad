
using System.Xml.Serialization;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public decimal? Price { get; set; } // Цена может быть null для групп

    public List<Service> Children { get; set; } = new List<Service>();

    // Метод для добавления дочерней услуги
    public void AddChild(Service child)
    {
        child.ParentId = this.Id;
        this.Children.Add(child);
    }
}

public class Report
{
    public static void GenerateReport(List<Service> services)
    {
        decimal totalPrice = 0;

        // Рекурсивный метод для обхода и вывода информации по услугам
        void ProcessService(Service service, int level = 0)
        {
            // Печать информации об услуге
            string indent = new string(' ', level * 4);  // Отступ для иерархии
            Console.WriteLine($"{indent}ID: {service.Id}, Name: {service.Name}, Price: {service.Price ?? 0:C}");

            // Если есть цена, добавляем её к общей сумме
            if (service.Price.HasValue)
            {
                totalPrice += service.Price.Value;
            }

            // Обрабатываем дочерние элементы
            foreach (var child in service.Children)
            {
                ProcessService(child, level + 1);
            }
        }

        // Процессинг всех услуг (начинаем с корневых)
        foreach (var service in services.Where(s => s.ParentId == null))
        {
            ProcessService(service);
        }

        // Вывод общей суммы
        Console.WriteLine($"\nTotal Price: {totalPrice:C}");
    }

    public static void SaveToXml(List<Service> services, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Service>));
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, services);
        }
    }

    public static List<Service> LoadFromXml(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Service>));
        using (StreamReader reader = new StreamReader(filePath))
        {
            return (List<Service>)serializer.Deserialize(reader);
        }
    }
}

public class Program
{
    public static void Main()
    {
        string filePath = "services.xml";

        // Загрузка из XML
        List<Service> services = Report.LoadFromXml(filePath);

        // Образец: добавление новой услуги
        var child = new Service { Id = 8, Name = "aaa"};
        services.Add(child);

        // Сохранение обновленного списка в XML
      //  Report.SaveToXml(services, filePath);

        // Создание отчета
        Report.GenerateReport(services);


    }
}
