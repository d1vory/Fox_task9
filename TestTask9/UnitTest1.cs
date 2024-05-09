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


    [TestMethod]
    public void TestCheckInput()
    {
        string[] invalidInputs = [
            "", "abc", "20.02.2024 USD", "er aa.dd.2214 USD",  "er !20.02.2024 USD",
            "er 20.02.2024 21", "er 20.02.2024 ab", "er 20:02:2024 USD", "er 20\\02\\2024 USD"
        ];
        string[] validInputs = [
            "er 20.02.2024 USD", "ER 1.2.2024 USD", "Er 1.02.2024 USD", "eR 01.2.2024 USD", "er 01/2/2024 USD",
            "  er          20.02.2024               USD", "er 20.02.2024 USD", "er 20.02.2024 USD", 
            "asdader 20.02.2024 USD",
        ];
        
        foreach (var input in invalidInputs)
        {
            Assert.IsFalse(ConvertService.CheckInput(input, out var tempDate, out var tempCurrency), $"exception for {input}");
        }
        foreach (var input in validInputs)
        {
            Assert.IsTrue(ConvertService.CheckInput(input, out var tempDate, out var tempCurrency), $"exception for {input}");
        }
    }


    [TestMethod]
    public void TestCheckDate()
    {
        string[] validDates = ["12.01.2024", "30.3.2022", "1.2.2024", "04/09/2020"];
        foreach (var input in validDates)
        {
            ConvertService.CheckDate(input);
        }
        string[] invalidDates = ["20\\02\\2024", "30.02.2022", "40.02.2024", "99.99.9999"];
        foreach (var input in invalidDates)
        {
            Assert.ThrowsException<ApplicationException>(() => ConvertService.CheckDate(input), $"No exception for {input}");
        }
    }


    [TestMethod]
    public void TestCheckCurrency()
    {
        foreach (var input in ConvertService.AllowedCurrencies)
        {
            ConvertService.CheckCurrency(input.ToUpper());
            ConvertService.CheckCurrency(input.ToLower());
        }

        string[] invalidInputs = ["abc", "", "qwe", "USDD", "TEsT"];
        foreach (var input in invalidInputs)
        {
            Assert.ThrowsException<ApplicationException>(() => ConvertService.CheckCurrency(input), $"No exception for {input}");
        }
    }
}