using Task9;

namespace TestTask9;

[TestClass]
public class TestConvertService
{
    [TestMethod]
    public void TestInputValue()
    {
        Assert.ThrowsException<ArgumentException>(() => new ConvertService("abc"));
        Assert.ThrowsException<ArgumentException>(() => new ConvertService(""));
        Assert.ThrowsException<ArgumentException>(() => new ConvertService("20.02.2024 USD"));
        Assert.ThrowsException<ArgumentException>(() => new ConvertService("er 20.02.2024"));
        Assert.ThrowsException<ArgumentException>(() => new ConvertService("er 20nmj.02.2024 USD"));
        Assert.ThrowsException<ApplicationException>(() => new ConvertService("er 40.02.2024 USD"));
        Assert.ThrowsException<ApplicationException>(() => new ConvertService("er 99.99.9999 USD"));
        Assert.ThrowsException<ApplicationException>(() => new ConvertService("er 20.02.2024 ABC"));

        string[] validStrings =
        [
            "er 20.02.2024 USD", "ER 1.2.2024 EUR", "Er 1.02.2024 CHF", "eR 01.2.2024 GBP", "er 01/2/2024 PLZ",
            "  er          20.02.2024               SEK", "er 20.02.2024 XAU", "er 20.02.2024 CAD",
        ];
        foreach (var str in validStrings)
        {
            var convertService = new ConvertService(str);
        }

    }

    [TestMethod]
    public async Task TestGetUahExchangeValue()
    {
        var cs = new ConvertService("er 01.01.2024 USD");
        var rate = await cs.GetUahExchangeValue();
        Assert.AreEqual(38.2m, rate.PurchaseRate);
        Assert.AreEqual(38.8m, rate.SaleRate);

        string[] noRateValues = ["er 01.01.2034 USD", "er 01.01.1980 USD", "er 01.01.1980 USD"];
        foreach (var input in noRateValues)
        {
            cs = new ConvertService(input);
            await Assert.ThrowsExceptionAsync<ApplicationException>(() => cs.GetUahExchangeValue(), $"No exception for {input}");
        }

    }
}