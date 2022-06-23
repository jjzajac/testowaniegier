using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Moq;
using Xunit;

namespace TestProject1;

internal static class Func
{
    public static T Id<T>(T t) => t;
}

public class Car
{
    public Car(int id, IEnumerable<(long, long)> year)
    {
        _id = id;
        Year = year;
    }

    private int _id;
    public IEnumerable<(long, long)> Year { get; }
}

public interface ICarRepo
{
    public Option<Car> GetCarById(int id);
}

internal class CarService
{
    private readonly ICarRepo _repo;

    public CarService(ICarRepo repo)
    {
        _repo = repo;
    }

    public long FindMileageBetweenYears(int id, int b, int e)
    {
        return _repo
               .GetCarById(id)
               .Bind(c => c.Year)
               .Filter(c => c.Item1 >= b && c.Item1 <= e)
               .Select(cc => cc.Item2)
               .Sum(Func.Id);
    }
}

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var car = new Car(1, new List<(long, long)> { (2019, 10), (2020, 10), (2021, 10) });
        var mock = new Mock<ICarRepo>();
        mock.Setup(c => c.GetCarById(1))
            .Returns(Option<Car>.Some(car));


        var service = new CarService(mock.Object);

        Assert.Equal(10, service.FindMileageBetweenYears(1, 2020, 2020));
        Assert.Equal(30, service.FindMileageBetweenYears(1, 2019, 2022));
        Assert.Equal(20, service.FindMileageBetweenYears(1, 2019, 2020));
        Assert.Equal(0, service.FindMileageBetweenYears(1, 2000, 2010));
        Assert.Equal(0, service.FindMileageBetweenYears(1, 2022, 2030));
    }

    [Fact]
    public void TestNull()
    {
        var mock = new Mock<ICarRepo>();
        mock.Setup(c => c.GetCarById(2))
            .Returns(Option<Car>.None);

        var service = new CarService(mock.Object);

        Assert.Equal(0, service.FindMileageBetweenYears(2, 2000, 2020));
    }
}