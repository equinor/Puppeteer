
# ASP.NET Core / NodeServices / Puppeteer / Imagemagick / Ghostscript

* Convert html to pdf
* Convert pdf to png


## To run

```
cd src
dotnet run
```

## To run in Docker

```
cd src
docker build -f Dockerfile .
```

Then run the image as a container using

```
docker run -d -p 8000:80 madpdf
```
# Examples

## curl pdf2png
**Optional**
page (default = 0)
dpi (default 180, range between 1 - 300)
```bash
curl -F 'file=@test.pdf' https://example.azurewebsites.net/api/v1/pdf/pdf2png?page=0&dpi=180 > test.png && open test.png
```


## curl html2pdf
```bash
curl -d "{ html: '<h1>header </h1> <p>123</p>', config: { format: "A4", landscape: false } }" -H "Content-Type: application/json" -X POST https://example.azurewebsites.net/api/v1/pdf/html2pdf > test.pdf && open test.pdf
```



## .NET c# html2pdf

```csharp
static void Main(string[] args)
{
    var pdfArray = CreatePdf().Result;
    System.IO.File.WriteAllBytes("myPdf.pdf", pdfArray);
}

// why static? https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client#create-and-initialize-httpclient
private static HttpClient client = new HttpClient();

public static async Task<byte[]> CreatePdf()
{

    var payload = new
    {
        html = "<p>test</p>",
        config = new
        {
            format = "A4",
            // If width or height is used format will be ignored
            // width = "100",
            // height = 100",
            landscape = false,
            margin = new { top = "10", bottom = "50" }, // top, right, bottom, left
            printBackground = true,
            preferCSSPageSize = true,
            scale = 0.6,

            displayHeaderFooter = true,

            /*
              Should be valid HTML markup with following classes used to inject printing values into them:
                date formatted print date
                title document title
                url document location
                pageNumber current page number
            */
            headerTemplate = "<div style='font-size: 10px'><span class='pageNumber'></span>My document</div>",
            footerTemplate = "<div style='font-size: 10px'><span class='url'>My document</div>"
        }
    };

    var json = JsonConvert.SerializeObject(payload);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", "token..");
    var result = await client.PostAsync("http://localhost:5000/api/v1/pdf/html2pdf", content);

    return await result.Content.ReadAsByteArrayAsync();
}
```