using System;
using Moq;
using Xunit;

namespace TestProject1;

public class FailOpen : Exception
{
    public override string Message { get; } = "FailOpen";
}

public class FailCommitTrans : Exception
{
    public override string Message { get; } = "FailCommitTrans";
}

public interface ISes
{
    void Open();
    void OpenTrans();
    void Save<T>(T v);
    void CommitTrans();
    void RollbackTrans();
    void Close();
}

public interface IDbLog
{
    void Log(string s);
}

public class GenRepo
{
    private readonly IDbLog _logger;

    public GenRepo(IDbLog logger)
    {
        _logger = logger;
    }

    public void Save<T>(ISes ses, T t)
    {
        ses.Open();

        try
        {
            ses.OpenTrans();
            ses.Save(t);
            ses.CommitTrans();
        }
        catch (Exception e)
        {
            _logger.Log(e.Message);
            ses.RollbackTrans();
        }

        ses.Close();
    }
}

public class UnitTest2
{
    [Fact]
    public void Test1()
    {
        var loggerMock = new Mock<IDbLog>();
        var sesMock = new Mock<ISes>();

        loggerMock.Setup(c => c.Log(It.IsAny<string>()));
        sesMock.Setup(s => s.Open()).Throws<FailOpen>();


        var repo = new GenRepo(loggerMock.Object);


        Assert.Throws<FailOpen>(() => repo.Save(sesMock.Object, 1));
    }

    [Fact]
    public void Test2()
    {
        var loggerMock = new Mock<IDbLog>();
        var sesMock = new Mock<ISes>();

        loggerMock.Setup(c => c.Log(It.IsAny<string>()))
                  .Callback((string s) => { Assert.Equal("FailCommitTrans", s); });

        sesMock.Setup(s => s.Open());
        sesMock.Setup(s => s.OpenTrans());
        sesMock.Setup(s => s.Save(1));
        sesMock.Setup(s => s.CommitTrans()).Throws<FailCommitTrans>();
        sesMock.Setup(s => s.RollbackTrans());


        var repo = new GenRepo(loggerMock.Object);

        repo.Save(sesMock.Object, 1);

        var isLogOnce = Record.Exception(() => loggerMock.Verify(s => s.Log(It.IsAny<string>()), Times.Once));
        Assert.Null(isLogOnce);

        var isOpenOnce = Record.Exception(() => sesMock.Verify(s => s.Open(), Times.Once));
        Assert.Null(isOpenOnce);

        var isOpenTransOnce = Record.Exception(() => sesMock.Verify(s => s.OpenTrans(), Times.Once));
        Assert.Null(isOpenTransOnce);

        var isSaveOnce = Record.Exception(() => sesMock.Verify(s => s.Save(1), Times.Once));
        Assert.Null(isSaveOnce);

        var isRollbackOnce = Record.Exception(() => sesMock.Verify(s => s.RollbackTrans(), Times.Once));
        Assert.Null(isRollbackOnce);
    }


    [Fact]
    public void Test3()
    {
        var loggerMock = new Mock<IDbLog>();
        var sesMock = new Mock<ISes>();

        loggerMock.Setup(c => c.Log(It.IsAny<string>()));

        sesMock.Setup(s => s.Open());
        sesMock.Setup(s => s.OpenTrans());
        sesMock.Setup(s => s.Save(1));
        sesMock.Setup(s => s.CommitTrans());
        sesMock.Setup(s => s.RollbackTrans());

        var repo = new GenRepo(loggerMock.Object);


        repo.Save(sesMock.Object, 1);

        var isLogNever = Record.Exception(() => loggerMock.Verify(s => s.Log(It.IsAny<string>()), Times.Never));
        Assert.Null(isLogNever);

        var isOpenOnce = Record.Exception(() => sesMock.Verify(s => s.Open(), Times.Once));
        Assert.Null(isOpenOnce);

        var isOpenTransOnce = Record.Exception(() => sesMock.Verify(s => s.OpenTrans(), Times.Once));
        Assert.Null(isOpenTransOnce);

        var isSaveOnce = Record.Exception(() => sesMock.Verify(s => s.Save(1), Times.Once));
        Assert.Null(isSaveOnce);

        var isRollbackNever = Record.Exception(() => sesMock.Verify(s => s.RollbackTrans(), Times.Never));
        Assert.Null(isRollbackNever);
    }
}
