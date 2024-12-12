using System.Drawing;
using System.Formats.Asn1;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new JsonStringEnumConverter() }
};

string basePath = @"C:\Users\Lenovo 5i Pro\TaskManager";
string dirPath = Path.Combine(basePath, "Tasks");
string jsonPath = Path.Combine(dirPath, "tasks.json");
string logPath = Path.Combine(basePath, "log.txt");

string pattern = @"[a-zA-Z0-9\s]+";

if(!File.Exists(logPath))
    File.Create(logPath).Close();

try
{
    if(!Directory.Exists(dirPath))
        Directory.CreateDirectory(dirPath);
    if(!File.Exists(jsonPath))
        File.Create(jsonPath).Close();
}
catch (Exception ex)
{
    WriteLog(logPath, ex.Message);
}

List<Task> tasks = ReadJsonFile(jsonPath);


while(true)
{
    Console.WriteLine("\t\tTask manager\n");
    Console.Write($"{"Vazifa qo'shish", -35} - 1\n{"Barcha vazifalarni ko'rish", - 35} - 2\n" + 
    $"{"Kalit so'z bo'yicha qidiruv", -35} - 3\n{"Vazifalarni bajarildi deb belgilash", -35} - 4\n{"Ilovadan chiqish", -35} - 5\n" + "--> ");
    string input = Console.ReadLine();
    try
    {
        switch(input)
        {
            case "1":
                AddTask(ref tasks);
                break;
            case "2":
                ShowTask(tasks);
                break;
            case "3":
                SearchTasksByKey(tasks);
                break;
            case "4":
                MarkAsCompleted(ref tasks);
                break;
            case "5":
                Console.WriteLine("\n\tDastur yakunlandi :)\n");
                return;
            default:
                Console.WriteLine("\n\tDasturdan chiqish uchun 5 ni bosing!\n");
                break;
        }
    }
    catch(Exception ex)
    {
        WriteLog(logPath, ex.Message);
        Console.WriteLine("\n\tXatolik yuz berdi, xatolik log.txt yozildi!");
    }

}

void AddTask(ref List<Task> tasks)
{
    Task newTask = new Task();
    newTask.Id = SequenceId(tasks);
    Console.Write("Task nomini kiriting -> ");
    newTask.Name = Console.ReadLine();
    string description = "";
    do
    {
        Console.Write("Task tavsifini kiriting - > ");
        description = Console.ReadLine();

    }while(!IsValidDescription(description, pattern));
    newTask.Description = description;

    try
    {
        Console.WriteLine("Muddat Namuma: 2024-12-31\tYYYY-MM-DD");
        Console.Write("Vazifani bajarish muddatini kiriting - > ");
        newTask.DueDate = DateTime.Parse(Console.ReadLine());
        Console.WriteLine("\n\tTak muvaffaqiyatli qo'shildi :)\n");
    }
    catch(Exception)
    {
        throw;
    }
    finally
    {
        tasks.Add(newTask);
        WriteJsonFile(tasks, jsonPath);
    }


}

void ShowTask(List<Task> tasks)
{
    if(tasks.Count > 0)
    {
        foreach(Task task in tasks)
            Console.WriteLine(task.ToString());
    }
    else
        Console.WriteLine("\nTasklar mavjud emas!\n");
}
void SearchTasksByKey(List<Task> tasks)
{
    Console.Write($"{"Name bilan qidiruv", -23} - 1\n{"DueDate bilan qidiruv", -23} - 2\n{"Status bilan qidiruv", -23} - 3\n{"Exit", - 23} - 4\n--> ");
    string input = Console.ReadLine();
    Console.WriteLine();
    
    switch(input)
    {
        case "1":
            Console.Write("Name kiriting -> ");
            string taskName = Console.ReadLine();
            List<Task> tasksSeachByName = tasks.FindAll(x => x.Name == taskName);
            if(tasksSeachByName.Count > 0)
            {
                foreach(var task in tasksSeachByName)
                    Console.WriteLine(task.ToString());
            }
            else
                Console.WriteLine("\n\tName bo'yicha Task topilmadi!\n");
            break;
        case "2":
            try
            {
                Console.Write("DueDate kiriting - > ");
                DateTime taskDueDate = DateTime.Parse(Console.ReadLine());
                List<Task> tasksSeachByDueDate = tasks.FindAll(x => x.DueDate.Date == taskDueDate.Date);
                if(tasksSeachByDueDate.Count > 0)
                {
                    foreach(var task in tasksSeachByDueDate)
                        Console.WriteLine(task.ToString());
                }
                else
                    Console.WriteLine("\n\tDueDate bo'yicha Task topiladi!\n");

            }
            catch(Exception)
            {
                throw;
            }
            break;
        case "3":
            Console.Write($"{"Pending", -10} - 1\n{"Done", -10} - 2\n--> ");
            string status = Console.ReadLine();

            switch(status)
            {
                case "1":
                List<Task> tasksSearchByStatusPending = tasks.FindAll(x => (int)x.Status == (int)Status.Pending);
                if(tasksSearchByStatusPending.Count > 0)
                {
                    foreach(var task in tasksSearchByStatusPending)
                        Console.WriteLine(task.ToString());
                }
                else
                    Console.WriteLine("\n\tStatus.Pending bo'yicha task topilmadi!\n");
                    break;
                case "2":
                List<Task> tasksSearchByStatusDone = tasks.FindAll(x => (int)x.Status == (int)Status.Pending);
                if(tasksSearchByStatusDone.Count > 0)
                {
                    foreach(var task in tasksSearchByStatusDone)
                        Console.WriteLine(task.ToString());
                }
                else
                    Console.WriteLine("\n\tStatus.Done bo'yicha task topilmadi!\n");
                    break;
                default:
                    break;
            }
            break;
        default:
            break;
    }
}

void MarkAsCompleted(ref List<Task> tasks)
{
    ShowTask(tasks);
    
    try
    {
        Console.Write("\nTask id kiriting -> ");
        int taskId = int.Parse(Console.ReadLine());
        Task taskById = tasks.Find(x => x.Id == taskId);
        if(taskById != null)
        {
            Console.WriteLine(taskById.ToString());
            if(taskById.Status != Status.Done)
            {
                taskById.Status = Status.Done;
                WriteJsonFile(tasks, jsonPath);
                Console.WriteLine("\tTask Status.Pending dan Status.Done ga o'tqazildi.");
            }
            else
                Console.WriteLine("\n\tTask allaqochon Yakunlangan!\n");
        }
        else
            Console.WriteLine("\n\tId bo'yicha task topilmadi!\n");
    }
    catch(Exception)
    {
        throw;
    }

}

int SequenceId(List<Task> tasks)
{
    int maxId = 0;
    if(tasks.Count > 0)
        maxId = tasks.Max(x => x.Id);

    return ++maxId; 
}


void WriteLog(string logPath, string log)
{
    using(StreamWriter streamWriter = new StreamWriter(logPath))
    {
        streamWriter.WriteLine(log);
    }
}

bool IsValidDescription(string description, string pattern)
{
    return Regex.IsMatch(description, pattern);
}


void WriteJsonFile(List<Task> tasks, string jsonPath)
{
    using(StreamWriter streamWriter = new StreamWriter(jsonPath))
    {
        string content = JsonSerializer.Serialize(tasks, jsonOptions);
        streamWriter.Write(content);
    }
}

List<Task> ReadJsonFile(string jsonPath)
{
    using(StreamReader streamReader = new StreamReader(jsonPath))
    {
        return JsonSerializer.Deserialize<List<Task>>(streamReader.ReadToEnd(), jsonOptions) ?? new List<Task>();
    }
}