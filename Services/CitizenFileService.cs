using CitizensApi.Models;

namespace CitizensApi.Services;

public class CitizenFileService
{
    private readonly string _filePath;

    public CitizenFileService(IConfiguration configuration)
    {
        _filePath = configuration["AppSettings:CitizensFilePath"] ?? "Data/citizens.csv";

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Close();
        }
    }

    public List<Citizen> GetAll()
    {
        var citizens = new List<Citizen>();
        var lines = File.ReadAllLines(_filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = line.Split(',');

            if (values.Length < 5)
            {
                continue;
            }

            citizens.Add(new Citizen
            {
                FirstName = values[0],
                LastName = values[1],
                CI = values[2],
                BloodGroup = values[3],
                PersonalAsset = values[4]
            });
        }

        return citizens;
    }

    public void SaveAll(List<Citizen> citizens)
    {
        var lines = citizens.Select(c =>
            string.Join(",", c.FirstName, c.LastName, c.CI, c.BloodGroup, c.PersonalAsset));

        File.WriteAllLines(_filePath, lines);
    }
}