using Microsoft.Playwright;

// Install Playwright
var exitCode = Microsoft.Playwright.Program.Main(new[] {"install"});
if (exitCode != 0)
{
    throw new Exception($"Playwright exited with code {exitCode}");
}

// Run the Playwright test
using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync();
var page = await browser.NewPageAsync();
await page.GotoAsync("https://tw.stock.yahoo.com/class-quote?sectorId=26&exchange=TAI");

// 使用 css selector 找到所有股票資訊
var nodes = //await page.QuerySelectorAllAsync("li.List(n)");
    await page.QuerySelectorAllAsync("li[class='List(n)']");

var stocks = nodes.Select(async a => new Stock
{
    //使用 XPath 找到股票名稱、代號、價格、漲跌、漲跌幅、開盤、昨收、最高、最低、成交量
    Name = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[1]/div[2]/div/div[1]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    Symbol = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[1]/div[2]/div/div[2]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    Price = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[2]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    Change = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[3]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    PriceChange = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[4]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    Open = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[5]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    LastClose = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[6]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    High = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[7]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    Low = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[8]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    Turnover = (await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[9]',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().InnerTextAsync()).Trim(),
    UpDown = UpDownCheck((await (await a.EvaluateHandleAsync("el => document.evaluate('./div/div[3]/span',el,null,XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue")).AsElement().GetAttributeAsync("class")).Trim()),
}).Select(b=>b.GetAwaiter().GetResult());

foreach(var stock in stocks) 
{
    Console.WriteLine($"股票名稱: {stock.Name.PadRight(12)}\t 股票代號: {stock.Symbol}\t 股價: {stock.Price.PadRight(5)}\t 漲跌: {stock.UpDown} {stock.PriceChange.PadRight(8)}\t 漲跌幅: {stock.UpDown} {stock.Change.PadRight(8)}\t 開盤: {stock.Open}\t 昨收: {stock.LastClose}\t 最高: {stock.High}\t 最低: {stock.Low}\t 成交量(張): {stock.Turnover}");
} 

string UpDownCheck(string value)
{
    if (value.Contains("up"))
    {
        return "上漲";
    }
    if (value.Contains("down"))
    {
        return "下跌";
    }
    return string.Empty;
}

class Stock
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Price { get; set; }
    public string Change { get; set; }
    public string PriceChange { get; set; }
    public string Open { get; set; }
    public string LastClose { get; set; }
    public string High { get; set; }
    public string Low { get; set; }
    public string Turnover { get; set; }
    public string UpDown { get; set; }
} 